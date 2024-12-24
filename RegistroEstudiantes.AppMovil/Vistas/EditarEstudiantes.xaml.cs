using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil.Vistas;

public partial class EditarEstudiantes : ContentPage
{
    private FirebaseClient client = MauiProgram.FirebaseClient;
    private Estudiante estudianteActual;
    public List<Curso> Cursos { get; set; } = new();

    public EditarEstudiantes(Estudiante estudiante)
    {
        InitializeComponent();

        estudianteActual = estudiante;

        // Cargar datos del estudiante seleccionado
        NombreEntry.Text = estudiante.Nombre;
        ApellidoEntry.Text = estudiante.Apellido;
        CorreoEntry.Text = estudiante.CorreoElectronico;
        EdadEntry.Text = estudiante.Edad.ToString();

        // Cargar lista de cursos
        CargarCursos();
    }

    private async void CargarCursos()
    {
        try
        {
            // Obtener los cursos desde Firebase
            var cursosEnFirebase = await client.Child("Cursos").OnceAsync<Curso>();

            if (cursosEnFirebase != null)
            {
                Cursos = cursosEnFirebase.Select(c => c.Object).ToList();
                CursoPicker.ItemsSource = Cursos;

                // Seleccionar el curso actual del estudiante
                CursoPicker.SelectedItem = Cursos.FirstOrDefault(c => c.Nombre == estudianteActual.Curso.Nombre);
            }
            else
            {
                await DisplayAlert("Error", "No se encontraron cursos en la base de datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los cursos: {ex.Message}", "OK");
        }
    }

    private async void OnGuardarButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar datos
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(ApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(CorreoEntry.Text) ||
                string.IsNullOrWhiteSpace(EdadEntry.Text) ||
                CursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
                return;
            }

            // Actualizar los datos del estudiante
            estudianteActual.Nombre = NombreEntry.Text;
            estudianteActual.Apellido = ApellidoEntry.Text;
            estudianteActual.CorreoElectronico = CorreoEntry.Text;
            estudianteActual.Edad = int.Parse(EdadEntry.Text);
            estudianteActual.Curso = CursoPicker.SelectedItem as Curso;

            // Guardar cambios en Firebase
            if (!string.IsNullOrEmpty(estudianteActual.FirebaseKey))
            {
                await client
                    .Child("Estudiantes")
                    .Child(estudianteActual.FirebaseKey) // Utiliza la clave única para identificar el nodo
                    .PutAsync(estudianteActual);

                await DisplayAlert("Éxito", "El estudiante ha sido actualizado correctamente.", "OK");
                await Navigation.PopAsync(); // Regresar a la lista
            }
            else
            {
                await DisplayAlert("Error", "No se encontró la clave única del estudiante.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar los cambios: {ex.Message}", "OK");
        }
    }
}
