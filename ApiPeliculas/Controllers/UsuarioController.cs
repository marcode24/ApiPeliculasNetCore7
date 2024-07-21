using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRespositorio;
        private readonly IMapper _mapper;
        protected RespuestaAPI _respuestaApi;

        public UsuarioController(IUsuarioRepositorio usuarioRespositorio, IMapper mapper)
        {
            _usuarioRespositorio = usuarioRespositorio;
            _mapper = mapper;
            _respuestaApi = new RespuestaAPI();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usuarioRespositorio.GetUsuarios();
            var listaUsuariosDTO = new List<UsuarioDTO>();
            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDTO.Add(_mapper.Map<UsuarioDTO>(lista));
            }
            return Ok(listaUsuariosDTO);
        }

        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int usuarioId)
        {
            var itemUsuario = _usuarioRespositorio.GetUsuario(usuarioId);
            if (itemUsuario == null) return NotFound();
            var itemUsuarioDTO = _mapper.Map<UsuarioDTO>(itemUsuario);
            return Ok(itemUsuarioDTO);
        }

        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDTO usuarioRegistroDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_usuarioRespositorio.IsUniqueUser(usuarioRegistroDTO.NombreUsuario))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages = new List<string>() { "El nombre de usuario ya existe" };
                return BadRequest(_respuestaApi);
            }

            var usuario = await _usuarioRespositorio.Registro(usuarioRegistroDTO);
            if(usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.InternalServerError;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages = new List<string>() { "Error al registrar el usuario" };
                return StatusCode(StatusCodes.Status500InternalServerError, _respuestaApi);
            }
            
            _respuestaApi.StatusCode = HttpStatusCode.Created;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO usuarioLoginDTO)
        {
            var usuario = await _usuarioRespositorio.Login(usuarioLoginDTO);
            if(usuario == null || string.IsNullOrEmpty(usuario.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.Unauthorized;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages = new List<string>() { "Usuario o contraseña incorrecta" };
                return Unauthorized(_respuestaApi);
            }
            return Ok(usuario);
        }
    }
}
