using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyFlowAPI.Data;
using MoneyFlowAPI.Models;
using MoneyFlowAPI.Services;

namespace MoneyFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public UsuariosController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                return BadRequest("Email já cadastrado.");

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(usuario);

            return Ok(new
            {
                message = "Usuário registrado com sucesso!",
                usuario = new
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email
                },
                token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario login)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(login.Senha, usuario.Senha))
                return Unauthorized("Email ou senha inválidos.");

            var token = _tokenService.GenerateToken(usuario);

            return Ok(new
            {
                message = "Login realizado com sucesso!",
                usuario = new
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email
                },
                token
            });
        }
    }
}
