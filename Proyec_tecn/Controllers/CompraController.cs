using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyec_tecn.DTOs;
using Proyec_tecn.Services;

namespace Proyec_tecn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDto>>> GetAll()
        {
            var compras = await _compraService.GetAllAsync();
            return Ok(compras);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDto>> GetById(int id)
        {
            var compra = await _compraService.GetByIdAsync(id);
            if (compra == null)
                return NotFound();

            return Ok(compra);
        }

        [HttpPost]
        public async Task<ActionResult<CompraDto>> Create(CreateCompraDto createCompraDto)
        {
            try
            {
                var compra = await _compraService.CreateAsync(createCompraDto);
                return CreatedAtAction(nameof(GetById), new { id = compra.Id_CompraCab }, compra);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
