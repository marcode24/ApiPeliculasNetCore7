﻿using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTOs
{
    public class UsuarioLoginDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }
}