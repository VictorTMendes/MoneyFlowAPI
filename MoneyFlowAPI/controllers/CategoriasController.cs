using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyFlowAPI.Data;
using MoneyFlowAPI.Models;

namespace MoneyFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _context.Categorias
                .OrderBy(c => c.Nome)
                .ToListAsync();
            return Ok(categorias);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nome))
            {
                return BadRequest("Nome da categoria é obrigatório.");
            }

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }
    }
}