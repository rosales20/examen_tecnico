using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyec_tecn.DTOs;
using Proyec_tecn.Services;

namespace Proyec_tecn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetAll()
        {
            var ventas = await _ventaService.GetAllAsync();
            return Ok(ventas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetById(int id)
        {
            var venta = await _ventaService.GetByIdAsync(id);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }

        [HttpGet("stock/{productoId}")]
        public async Task<ActionResult<int>> GetStock(int productoId)
        {
            var stock = await _ventaService.GetStockByProductoAsync(productoId);
            return Ok(stock);
        }

        [HttpPost]
        public async Task<ActionResult<VentaDto>> Create(CreateVentaDto createVentaDto)
        {
            try
            {
                var venta = await _ventaService.CreateAsync(createVentaDto);
                return CreatedAtAction(nameof(GetById), new { id = venta.Id_VentaCab }, venta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
