using Proyec_tecn.Data;
using Proyec_tecn.DTOs;
using Proyec_tecn.Moldels;
using Microsoft.EntityFrameworkCore;

namespace Proyec_tecn.Services
{
    public class ProductoService : IProductoService
    {
        
            private readonly ApplicationDbContext _context;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            return producto != null ? MapToDto(producto) : null;
        }

        public async Task<ProductoDto> CreateAsync(CreateProductoDto createProductoDto)
        {
            var producto = new Producto
            {
                Nombre_producto = createProductoDto.Nombre_producto,
                NroLote = createProductoDto.NroLote,
                Costo = createProductoDto.Costo,
                PrecioVenta = createProductoDto.Costo * 1.35m,
                Fec_registro = DateTime.Now
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return MapToDto(producto);
        }

        public async Task<ProductoDto> UpdateAsync(UpdateProductoDto updateProductoDto)
        {
            var producto = await _context.Productos.FindAsync(updateProductoDto.Id_producto);
            if (producto == null)
                throw new ArgumentException("Producto no encontrado");

            producto.Nombre_producto = updateProductoDto.Nombre_producto;
            producto.NroLote = updateProductoDto.NroLote;
            producto.Costo = updateProductoDto.Costo;
            producto.PrecioVenta = updateProductoDto.Costo * 1.35m;

            await _context.SaveChangesAsync();
            return MapToDto(producto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ProductoDto MapToDto(Producto producto)
        {
            return new ProductoDto
            {
                Id_producto = producto.Id_producto,
                Nombre_producto = producto.Nombre_producto,
                NroLote = producto.NroLote,
                Fec_registro = producto.Fec_registro,
                Costo = producto.Costo,
                PrecioVenta = producto.PrecioVenta
            };
        }
    }




}

    
