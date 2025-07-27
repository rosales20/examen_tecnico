using Microsoft.EntityFrameworkCore;
using Proyec_tecn.Data;
using Proyec_tecn.DTOs;
using Proyec_tecn.Moldels;

namespace Proyec_tecn.Services
{
    public class CompraService: ICompraService
    {
        private readonly ApplicationDbContext _context;

        public CompraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompraDto>> GetAllAsync()
        {
            var compras = await _context.CompraCabs
                .Include(c => c.CompraDets)
                    .ThenInclude(cd => cd.Producto)
                .ToListAsync();

            return compras.Select(MapToDto);
        }

        public async Task<CompraDto?> GetByIdAsync(int id)
        {
            var compra = await _context.CompraCabs
                .Include(c => c.CompraDets)
                    .ThenInclude(cd => cd.Producto)
                .FirstOrDefaultAsync(c => c.Id_CompraCab == id);

            return compra != null ? MapToDto(compra) : null;
        }

        public async Task<CompraDto> CreateAsync(CreateCompraDto createCompraDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var compraCab = new CompraCab
                {
                    FecRegistro = DateTime.Now,
                    SubTotal = 0,
                    Igv = 0,
                    Total = 0
                };

                _context.CompraCabs.Add(compraCab);
                await _context.SaveChangesAsync();

                decimal subTotal = 0;

                foreach (var detalle in createCompraDto.Detalles)
                {
                    var compraDet = new CompraDet
                    {
                        Id_CompraCab = compraCab.Id_CompraCab,
                        Id_producto = detalle.Id_producto,
                        Cantidad = detalle.Cantidad,
                        Precio = detalle.Precio,
                        Sub_Total = detalle.Cantidad * detalle.Precio,
                        Igv = (detalle.Cantidad * detalle.Precio) * 0.18m,
                        Total = (detalle.Cantidad * detalle.Precio) * 1.18m
                    };

                    subTotal += compraDet.Sub_Total;
                    _context.CompraDets.Add(compraDet);

                    // Actualizar producto
                    var producto = await _context.Productos.FindAsync(detalle.Id_producto);
                    if (producto != null)
                    {
                        producto.Costo = detalle.Precio;
                        producto.PrecioVenta = detalle.Precio * 1.35m;
                    }
                }

                // Actualizar totales de la cabecera
                compraCab.SubTotal = subTotal;
                compraCab.Igv = subTotal * 0.18m;
                compraCab.Total = subTotal * 1.18m;

                // Crear movimiento de entrada
                var movimientoCab = new MovimientoCab
                {
                    Fec_registro = DateTime.Now,
                    Id_TipoMovimiento = 1, // Entrada
                    Id_DocumentoOrigen = compraCab.Id_CompraCab
                };

                _context.MovimientoCabs.Add(movimientoCab);
                await _context.SaveChangesAsync();

                foreach (var detalle in createCompraDto.Detalles)
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

                return await GetByIdAsync(compraCab.Id_CompraCab) ?? throw new Exception("Error al crear la compra");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static CompraDto MapToDto(CompraCab compra)
        {
            return new CompraDto
            {
                Id_CompraCab = compra.Id_CompraCab,
                FecRegistro = compra.FecRegistro,
                SubTotal = compra.SubTotal,
                Igv = compra.Igv,
                Total = compra.Total,
                Detalles = compra.CompraDets.Select(cd => new CompraDetDto
                {
                    Id_producto = cd.Id_producto,
                    Nombre_producto = cd.Producto.Nombre_producto,
                    Cantidad = cd.Cantidad,
                    Precio = cd.Precio,
                    Sub_Total = cd.Sub_Total,
                    Igv = cd.Igv,
                    Total = cd.Total
                }).ToList()
            };
        }
    }
}
