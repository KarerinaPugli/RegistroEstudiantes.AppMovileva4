﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RegistroEstudiantes.Modelos.Modelos
{
    public class Estudiante
    {
        public string? FirebaseKey { get; set; } // Clave única de Firebase
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? CorreoElectronico { get; set; }
        public int Edad { get; set; }
        public Curso Curso { get; set; } // Asociación con un curso
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}

