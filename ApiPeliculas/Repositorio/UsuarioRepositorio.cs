using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio: IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        private string claveSecreta;
        public UsuarioRepositorio(ApplicationDbContext bd, IConfiguration configuration)
        {
            _bd = bd;
            claveSecreta = configuration.GetValue<string>("ApiSettings:SecretKey");
        }

        public Usuario GetUsuario(int usuarioId)
        {
            return _bd.Usuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(u => u.NombreUsuario).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuarioExiste = _bd.Usuario.Any(u => u.NombreUsuario.ToLower().Trim() == usuario.ToLower().Trim());
            return usuarioExiste;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            var passwordEncriptado = ObtenerMD5(usuarioRegistroDTO.Password);
            Usuario usuario = new Usuario
            {
                NombreUsuario = usuarioRegistroDTO.NombreUsuario,
                Nombre = usuarioRegistroDTO.Nombre,
                Password = passwordEncriptado,
                Role = usuarioRegistroDTO.Role
            };
            _bd.Usuario.Add(usuario);
            await _bd.SaveChangesAsync();
            usuario.Password = "";
            return usuario;
        }

        public async Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usuarioLoginDTO)
        {
            var passwordEncriptado = ObtenerMD5(usuarioLoginDTO.Password);
            var usuario = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario == usuarioLoginDTO.NombreUsuario && u.Password == passwordEncriptado);
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDTO()
                {
                    Token = "",
                    Usuario = null
                };
            }

            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new(ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            usuario.Password = "";
            var token = manejadorToken.CreateToken(tokenDescriptor);
            UsuarioLoginRespuestaDTO usuarioLoginRespuestaDTO = new()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = usuario
            };
            return usuarioLoginRespuestaDTO;
        }

        public static string ObtenerMD5(string valor)
        {
            MD5CryptoServiceProvider x = new();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}
