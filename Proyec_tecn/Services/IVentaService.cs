using Proyec_tecn.DTOs;

namespace Proyec_tecn.Services
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDto>> GetAllAsync();
        Task<VentaDto?> GetByIdAsync(int id);
        Task<VentaDto> CreateAsync(CreateVentaDto createVentaDto);
        Task<int> GetStockByProductoAsync(int productoId);
    }
}
