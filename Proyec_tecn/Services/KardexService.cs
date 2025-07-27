using Microsoft.EntityFrameworkCore;
using Proyec_tecn.Data;
using Proyec_tecn.DTOs;

namespace Proyec_tecn.Services
{
    public class KardexService : IKardexService
    {
        private readonly ApplicationDbContext _context;

        public KardexService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KardexDto>> GetKardexAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            var kardexList = new List<KardexDto>();

            foreach (var producto in productos)
            {
                var entradas = await _context.MovimientoDets
                    .Where(md => md.Id_Producto == producto.Id_producto && md.MovimientoCab.Id_TipoMovimiento == 1)
                    .SumAsync(md => md.Cantidad);

                var salidas = await _context.MovimientoDets
                    .Where(md => md.Id_Producto == producto.Id_producto && md.MovimientoCab.Id_TipoMovimiento == 2)
                    .SumAsync(md => md.Cantidad);

                var stock = entradas - salidas;

                kardexList.Add(new KardexDto
                {
                    Id_producto = producto.Id_producto,
                    Nombre_producto = producto.Nombre_producto,
                    Stock_actual = stock,
                    Costo = producto.Costo,
                    PrecioVenta = producto.PrecioVenta
                });
            }

            return kardexList;
        }

        public async Task<IEnumerable<MovimientoProductoDto>> GetMovimientosByProductoAsync(int productoId)
        {
            var movimientos = await _context.MovimientoDets
                .Include(md => md.MovimientoCab)
                .Where(md => md.Id_Producto == productoId)
                .OrderByDescending(md => md.MovimientoCab.Fec_registro)
                .ToListAsync();

            return movimientos.Select(md => new MovimientoProductoDto
            {
                Fecha_registro = md.MovimientoCab.Fec_registro,
                Tipo_Movimiento = md.MovimientoCab.Id_TipoMovimiento == 1 ? "Entrada" : "Salida",
                Cantidad = md.Cantidad
            });
        }

    }
}
