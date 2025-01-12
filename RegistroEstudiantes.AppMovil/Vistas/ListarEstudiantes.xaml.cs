using Firebase.Database;
using RegistroEstudiantes.Modelos.Modelos;
using System.Collections.ObjectModel;
using System.Linq;

namespace RegistroEstudiantes.AppMovil.Vistas;

public partial class ListarEstudiantes : ContentPage
{
    private FirebaseClient client = MauiProgram.FirebaseClient;
    public ObservableCollection<Estudiante> Lista { get; set; } = new ObservableCollection<Estudiante>();

    public ListarEstudiantes()
    {
        InitializeComponent();
        BindingContext = this;
        CargarLista();
    }

    private void CargarLista()
    {
        try
        {
            // Suscribirse a cambios en Firebase
            client.Child("Estudiantes").AsObservable<Estudiante>().Subscribe(estudiante =>
            {
                if (estudiante?.Object != null)
                {
                    estudiante.Object.FirebaseKey = estudiante.Key; // Guardar clave �nica

                    // Verificar si el estudiante ya existe en la lista
                    var estudianteExistente = Lista.FirstOrDefault(e => e.FirebaseKey == estudiante.Object.FirebaseKey);
                    if (estudianteExistente != null)
                    {
                        // Actualizar estudiante existente
                        var index = Lista.IndexOf(estudianteExistente);
                        Lista[index] = estudiante.Object;
                    }
                    else
                    {
                        // Agregar nuevo estudiante
                        Lista.Add(estudiante.Object);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"No se pudo cargar la lista de estudiantes: {ex.Message}", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Refrescar la lista manualmente al regresar de la edici�n
        RefrescarLista();
    }

    private async void RefrescarLista()
    {
        try
        {
            // Cargar todos los estudiantes de Firebase
            var estudiantesEnFirebase = await client.Child("Estudiantes").OnceAsync<Estudiante>();

            Lista.Clear();
            foreach (var estudiante in estudiantesEnFirebase)
            {
                estudiante.Object.FirebaseKey = estudiante.Key;
                Lista.Add(estudiante.Object);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"No se pudo refrescar la lista: {ex.Message}", "OK");
        }
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string filtro = filtroSearchBar.Text?.ToUpper() ?? string.Empty;

            if (!string.IsNullOrEmpty(filtro))
            {
                listaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToUpper().Contains(filtro));
            }
            else
            {
                listaCollection.ItemsSource = Lista;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al filtrar los estudiantes: {ex.Message}", "OK");
        }
    }

    private async void NuevoEstudianteBoton_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new CrearEstudiante());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al abrir la p�gina de creaci�n: {ex.Message}", "OK");
        }
    }

    private async void EditarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Obtener el estudiante desde el par�metro del bot�n
            if (sender is Button button && button.CommandParameter is Estudiante estudiante)
            {
                await Navigation.PushAsync(new EditarEstudiantes(estudiante));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al abrir la p�gina de edici�n: {ex.Message}", "OK");
        }
    }
}
