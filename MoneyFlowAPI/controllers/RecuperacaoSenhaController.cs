using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyFlowAPI.Data;
using MoneyFlowAPI.DTO;
using MoneyFlowAPI.Models;
using MoneyFlowAPI.services;
using MoneyFlowAPI.Services;

namespace MoneyFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecuperacaoSenhaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public RecuperacaoSenhaController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequest request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var token = Guid.NewGuid().ToString("N");
            var expiraEm = DateTime.UtcNow.AddHours(1);

            var resetToken = new PasswordResetToken
            {
                UsuarioId = usuario.Id,
                Token = token,
                ExpiraEm = expiraEm
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var link = $"https://projeto-integrado-multidisciplinar.vercel.app/src/pages/login/nova-senha.html?token={token}";
            var mensagem = $@"
                <h3>Recuperação de Senha - MoneyFlow</h3>
                <p>Olá, {usuario.Nome}!</p>
                <p>Clique para redefinir sua senha:</p>
                <a href='{link}'>{link}</a>";

            await _emailService.EnviarEmailAsync(usuario.Email, "Recuperação de Senha", mensagem);

            return Ok("E-mail de recuperação enviado com sucesso.");
        }


        [HttpPost("resetar-senha")]
        public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaRequest request)
        {
            var tokenValido = await _context.PasswordResetTokens
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Token == request.Token && t.ExpiraEm > DateTime.UtcNow);

            if (tokenValido == null)
                return BadRequest("Token inválido ou expirado.");

            tokenValido.Usuario.Senha = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            _context.PasswordResetTokens.Remove(tokenValido);
            await _context.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso!");
        }
    }

    public class ResetarSenhaRequest
    {
        public string Token { get; set; }
        public string NovaSenha { get; set; }
    }
}
