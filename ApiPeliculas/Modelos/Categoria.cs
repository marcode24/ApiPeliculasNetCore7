using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
