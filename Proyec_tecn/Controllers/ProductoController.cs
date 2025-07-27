using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyec_tecn.DTOs;
using Proyec_tecn.Services;

namespace Proyec_tecn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
        {
            var productos = await _productoService.GetAllAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetById(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Create(CreateProductoDto createProductoDto)
        {
            try
            {
                var producto = await _productoService.CreateAsync(createProductoDto);
                return CreatedAtAction(nameof(GetById), new { id = producto.Id_producto }, producto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> Update(int id, UpdateProductoDto updateProductoDto)
        {
            if (id != updateProductoDto.Id_producto)
                return BadRequest("ID mismatch");

            try
            {
                var producto = await _productoService.UpdateAsync(updateProductoDto);
                return Ok(producto);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productoService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
