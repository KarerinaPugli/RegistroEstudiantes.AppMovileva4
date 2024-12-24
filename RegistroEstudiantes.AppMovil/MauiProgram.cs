using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil
{
    public static class MauiProgram
    {
        // Cliente Firebase accesible globalmente
        public static FirebaseClient FirebaseClient { get; private set; } =
            new FirebaseClient("https://registroestudiantes-eda53-default-rtdb.firebaseio.com/");

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            RegistrarCursos();
            return builder.Build();
        }

        // Método para registrar cursos iniciales
        public static void RegistrarCursos()
        {
            try
            {
                // Obtener los cursos existentes
                var cursos = FirebaseClient.Child("Cursos").OnceAsync<Curso>().Result;

                // Si no hay cursos registrados, los añadimos
                if (cursos.Count == 0)
                {
                    var cursosIniciales = new List<Curso>
                    {
                        new Curso { Nombre = "1ro Básico" },
                        new Curso { Nombre = "2do Básico" },
                        new Curso { Nombre = "3ro Básico" },
                        new Curso { Nombre = "4to Básico" },
                        new Curso { Nombre = "5to Básico" },
                        new Curso { Nombre = "6to Básico" },
                        new Curso { Nombre = "7mo Básico" },
                        new Curso { Nombre = "8vo Básico" },
                        new Curso { Nombre = "1ro Medio" },
                        new Curso { Nombre = "2do Medio" },
                        new Curso { Nombre = "3ro Medio" },
                        new Curso { Nombre = "4to Medio" }
                    };

                    foreach (var curso in cursosIniciales)
                    {
                        FirebaseClient.Child("Cursos").PostAsync(curso).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar cursos: {ex.Message}");
            }
        }

        // Método para obtener cursos como una lista reutilizable
        public static List<Curso> ObtenerCursos()
        {
            try
            {
                var cursos = FirebaseClient.Child("Cursos").OnceAsync<Curso>().Result;
                return cursos.Select(x => x.Object).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener cursos: {ex.Message}");
                return new List<Curso>();
            }
        }
    }
}
