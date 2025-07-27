using Microsoft.EntityFrameworkCore;
using Proyec_tecn.Data;
using Proyec_tecn.DTOs;
using Proyec_tecn.Moldels;

namespace Proyec_tecn.Services
{
    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _context;

        public VentaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VentaDto>> GetAllAsync()
        {
            var ventas = await _context.VentaCabs
                .Include(v => v.VentaDets)
                    .ThenInclude(vd => vd.Producto)
                .ToListAsync();

            return ventas.Select(MapToDto);
        }

        public async Task<VentaDto?> GetByIdAsync(int id)
        {
            var venta = await _context.VentaCabs
                .Include(v => v.VentaDets)
                    .ThenInclude(vd => vd.Producto)
                .FirstOrDefaultAsync(v => v.Id_VentaCab == id);

            return venta != null ? MapToDto(venta) : null;
        }

        public async Task<int> GetStockByProductoAsync(int productoId)
        {
            var entradas = await _context.MovimientoDets
                .Where(md => md.Id_Producto == productoId && md.MovimientoCab.Id_TipoMovimiento == 1)
                .SumAsync(md => md.Cantidad);

            var salidas = await _context.MovimientoDets
                .Where(md => md.Id_Producto == productoId && md.MovimientoCab.Id_TipoMovimiento == 2)
                .SumAsync(md => md.Cantidad);

            return entradas - salidas;
        }

        public async Task<VentaDto> CreateAsync(CreateVentaDto createVentaDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validar stock
                foreach (var detalle in createVentaDto.Detalles)
                {
                    var stock = await GetStockByProductoAsync(detalle.Id_producto);
                    if (detalle.Cantidad > stock)
                    {
                        var producto = await _context.Productos.FindAsync(detalle.Id_producto);
                        throw new InvalidOperationException($"Stock insuficiente para el producto {producto?.Nombre_producto}. Stock disponible: {stock}");
                    }
                }

                var ventaCab = new VentaCab
                {
                    FecRegistro = DateTime.Now,
                    SubTotal = 0,
                    Igv = 0,
                    Total = 0
                };

                _context.VentaCabs.Add(ventaCab);
                await _context.SaveChangesAsync();

                decimal subTotal = 0;

                foreach (var detalle in createVentaDto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.Id_producto);
                    if (producto == null)
                        throw new ArgumentException($"Producto con ID {detalle.Id_producto} no encontrado");

                    var ventaDet = new VentaDet
                    {
                        Id_VentaCab = ventaCab.Id_VentaCab,
                        Id_producto = detalle.Id_producto,
                        Cantidad = detalle.Cantidad,
                        Precio = producto.PrecioVenta,
                        Sub_Total = detalle.Cantidad * producto.PrecioVenta,
                        Igv = (detalle.Cantidad * producto.PrecioVenta) * 0.18m,
                        Total = (detalle.Cantidad * producto.PrecioVenta) * 1.18m
                    };

                    subTotal += ventaDet.Sub_Total;
                    _context.VentaDets.Add(ventaDet);
                }

                // Actualizar totales de la cabecera
                ventaCab.SubTotal = subTotal;
                ventaCab.Igv = subTotal * 0.18m;
                ventaCab.Total = subTotal * 1.18m;

                // Crear movimiento de salida
                var movimientoCab = new MovimientoCab
                {
                    Fec_registro = DateTime.Now,
                    Id_TipoMovimiento = 2, // Salida
                    Id_DocumentoOrigen = ventaCab.Id_VentaCab
                };

                _context.MovimientoCabs.Add(movimientoCab);
                await _context.SaveChangesAsync();

                foreach (var detalle in createVentaDto.Detalles)
                {
                    var movimientoDet = new MovimientoDet
                    {
                        Id_movimientocab = movimientoCab.Id_MovimientoCab,
                        Id_Producto = detalle.Id_producto,
                        Cantidad = detalle.Cantidad
                    };

                    _context.MovimientoDets.Add(movimientoDet);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetByIdAsync(ventaCab.Id_VentaCab) ?? throw new Exception("Error al crear la venta");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static VentaDto MapToDto(VentaCab venta)
        {
            return new VentaDto
            {
                Id_VentaCab = venta.Id_VentaCab,
                FecRegistro = venta.FecRegistro,
                SubTotal = venta.SubTotal,
                Igv = venta.Igv,
                Total = venta.Total,
                Detalles = venta.VentaDets.Select(vd => new VentaDetDto
                {
                    Id_producto = vd.Id_producto,
                    Nombre_producto = vd.Producto.Nombre_producto,
                    Cantidad = vd.Cantidad,
                    Precio = vd.Precio,
                    Sub_Total = vd.Sub_Total,
                    Igv = vd.Igv,
                    Total = vd.Total
                }).ToList()
            };
        }
    }


}
