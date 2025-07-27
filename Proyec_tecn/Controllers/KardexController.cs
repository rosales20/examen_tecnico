using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyec_tecn.DTOs;
using Proyec_tecn.Services;

namespace Proyec_tecn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class KardexController : ControllerBase
    {
        private readonly IKardexService _kardexService;

        public KardexController(IKardexService kardexService)
        {
            _kardexService = kardexService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KardexDto>>> GetKardex()
        {
            var kardex = await _kardexService.GetKardexAsync();
            return Ok(kardex);
        }

        [HttpGet("movimientos/{productoId}")]
        public async Task<ActionResult<IEnumerable<MovimientoProductoDto>>> GetMovimientosByProducto(int productoId)
        {
            var movimientos = await _kardexService.GetMovimientosByProductoAsync(productoId);
            return Ok(movimientos);
        }
    }
}
