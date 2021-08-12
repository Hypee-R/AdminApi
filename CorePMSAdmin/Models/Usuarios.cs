using System;
using System.Collections.Generic;

namespace CorePMSAdmin.Models
{
    public partial class Usuarios
    {
        public int Id { get; set; }
        public string Correo { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public bool Estatus { get; set; }
        public string Sexo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string Token { get; set; }
    }
}
