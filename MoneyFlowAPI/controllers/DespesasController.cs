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
            var email = User.FindFirstValue(ClaimTypes.Email);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            return usuario?.Id ?? 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = GetUsuarioId();
            var despesas = await _context.Despesas
                .Include(d => d.CategoriaId) // ← IMPORTANTE: Inclui categoria
                .Where(d => d.UsuarioId == usuarioId)
                .OrderByDescending(d => d.DataDespesa)
                .ToListAsync();

            return Ok(despesas);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Despesa despesa)
        {
            var usuarioId = GetUsuarioId();

            if (usuarioId == 0)
            {
                return Unauthorized("Usuário não identificado.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { errors });
            }

            if (string.IsNullOrWhiteSpace(despesa.Descricao))
            {
                return BadRequest("Descrição é obrigatória.");
            }

            if (despesa.Valor <= 0)
            {
                return BadRequest("Valor deve ser maior que zero.");
            }

            if (despesa.CategoriaId <= 0)
            {
                return BadRequest("Categoria inválida.");
            }

            despesa.UsuarioId = usuarioId;

            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();

            // Recarrega com a categoria incluída
            var despesaCriada = await _context.Despesas
                .Include(d => d.CategoriaId)
                .FirstOrDefaultAsync(d => d.Id == despesa.Id);

            return Ok(despesaCriada);
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
            despesaExistente.Descricao = despesa.Descricao;
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