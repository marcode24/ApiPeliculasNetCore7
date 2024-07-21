using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using AutoMapper;

namespace ApiPeliculas.PeliculasMapper
{
    public class PeliculasMapper: Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDTO>().ReverseMap();
            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
        }
    }
}
