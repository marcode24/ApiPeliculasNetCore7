using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _peliculaRepositorio;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio peliculaRepositorio, IMapper mapper)
        {
            _peliculaRepositorio = peliculaRepositorio;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var listaPeliculas = _peliculaRepositorio.GetPeliculas();
            var listaPeliculasDto = new List<CategoriaDTO>();
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<CategoriaDTO>(lista));
            }
            return Ok(listaPeliculasDto);
        }

        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _peliculaRepositorio.GetPelicula(peliculaId);
            if (itemPelicula == null) return NotFound();
            var itemPeliculaDTO = _mapper.Map<PeliculaDTO>(itemPelicula);

            return Ok(itemPeliculaDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PeliculaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearPelicula([FromBody] PeliculaDTO crearPeliculaDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (crearPeliculaDTO == null) return BadRequest(ModelState);
            if (_peliculaRepositorio.ExistePelicula(crearPeliculaDTO.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }
            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDTO);
            if (!_peliculaRepositorio.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        [HttpPatch("{peliculaId:int}", Name = "ActualizarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPelicula(int peliculaId, [FromBody] PeliculaDTO peliculaDTO)
        {
            if (peliculaDTO == null || peliculaId != peliculaDTO.Id) return BadRequest(ModelState);
            var pelicula = _mapper.Map<Pelicula>(peliculaDTO);
            if (!_peliculaRepositorio.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
