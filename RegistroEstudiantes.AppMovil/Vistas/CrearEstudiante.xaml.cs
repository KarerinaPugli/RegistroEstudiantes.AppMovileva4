using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil.Vistas;

public partial class CrearEstudiante : ContentPage
{
    public List<Curso> Cursos { get; set; } = new List<Curso>();

    public CrearEstudiante()
    {
        InitializeComponent();
        BindingContext = this;
        CargarCursos();
    }

    private void CargarCursos()
    {
        try
        {
            // Obtener cursos usando MauiProgram.ObtenerCursos
            Cursos = MauiProgram.ObtenerCursos();

            if (Cursos.Any())
            {
                cursoPicker.ItemsSource = Cursos; // Asignar la lista de cursos al Picker
            }
            else
            {
                DisplayAlert("Error", "No se encontraron cursos disponibles.", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al cargar los cursos: {ex.Message}", "OK");
        }
    }

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validar datos
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(ApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(CorreoElectronicoEntry.Text) ||
                string.IsNullOrWhiteSpace(EdadEntry.Text) ||
                cursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Por favor completa todos los campos antes de guardar.", "OK");
                return;
            }

            // Crear un nuevo estudiante
            Curso cursoSeleccionado = cursoPicker.SelectedItem as Curso;
            var nuevoEstudiante = new Estudiante
            {
                Nombre = NombreEntry.Text,
                Apellido = ApellidoEntry.Text,
                CorreoElectronico = CorreoElectronicoEntry.Text,
                Edad = int.Parse(EdadEntry.Text),
                Curso = cursoSeleccionado
            };

            // Guardar en Firebase
            await MauiProgram.FirebaseClient
                .Child("Estudiantes")
                .PostAsync(nuevoEstudiante);

            await DisplayAlert("Éxito", $"El estudiante {nuevoEstudiante.Nombre} {nuevoEstudiante.Apellido} fue guardado exitosamente.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar el estudiante: {ex.Message}", "OK");
        }
    }
}
