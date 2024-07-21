using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.DTOs
{
    public class CrearCategoriaDTO
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [MaxLength(60, ErrorMessage = "La longitud máxima del nombre es 60 caracteres")]
        public string Nombre { get; set; }
    }
}
