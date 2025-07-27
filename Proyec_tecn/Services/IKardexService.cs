using Proyec_tecn.DTOs;

namespace Proyec_tecn.Services
{
    public interface IKardexService
    {
        Task<IEnumerable<KardexDto>> GetKardexAsync();
        Task<IEnumerable<MovimientoProductoDto>> GetMovimientosByProductoAsync(int productoId);
    }
}
