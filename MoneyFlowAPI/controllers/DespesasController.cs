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
    public class DespesasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DespesasController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUsuarioId()
        {
            // Pega o e-mail do token e busca o usuário
            var email = User.FindFirstValue(ClaimTypes.Email);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            return usuario?.Id ?? 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = GetUsuarioId();
            var despesas = await _context.Despesas
                .Where(d => d.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(despesas);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Despesa despesa)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            despesa.UsuarioId = userId;

            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();

            return Ok(despesa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Despesa despesa)
        {
            var usuarioId = GetUsuarioId();
            var despesaExistente = await _context.Despesas
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);

            if (despesaExistente == null)
                return NotFound("Despesa não encontrada.");

            despesaExistente.Valor = despesa.Valor;
            despesaExistente.DataDespesa = despesa.DataDespesa;
            despesaExistente.CategoriaId = despesa.CategoriaId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Despesa atualizada com sucesso!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = GetUsuarioId();
            var despesa = await _context.Despesas
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == usuarioId);

            if (despesa == null)
                return NotFound("Despesa não encontrada.");

            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Despesa excluída com sucesso!" });
        }
    }
}
