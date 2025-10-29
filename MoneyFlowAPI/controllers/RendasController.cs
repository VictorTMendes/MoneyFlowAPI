using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyFlowAPI.Data;
using MoneyFlowAPI.Models;
using System.Security.Claims;

namespace MoneyFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RendasController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUsuarioId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            return usuario?.Id ?? 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = GetUsuarioId();
            var rendas = await _context.Rendas
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(rendas);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Renda renda)
        {
            var userId = GetUsuarioId();

            if (userId == 0)
                return Unauthorized("Usuário não autenticado.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            renda.UsuarioId = userId;
            _context.Rendas.Add(renda);
            await _context.SaveChangesAsync();

            return Ok(renda);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Renda renda)
        {
            var usuarioId = GetUsuarioId();
            var rendaExistente = await _context.Rendas
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

            if (rendaExistente == null)
                return NotFound("Renda não encontrada.");

            rendaExistente.Valor = renda.Valor;
            rendaExistente.Data = renda.Data;
            rendaExistente.CategoriaId = renda.CategoriaId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Renda atualizada com sucesso!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = GetUsuarioId();
            var renda = await _context.Rendas
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

            if (renda == null)
                return NotFound("Renda não encontrada.");

            _context.Rendas.Remove(renda);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Renda excluída com sucesso!" });
        }
    }
}
