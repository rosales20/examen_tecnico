using Proyec_tecn.DTOs;

namespace Proyec_tecn.Services
{
    public interface ICompraService
    {
        Task<IEnumerable<CompraDto>> GetAllAsync();
        Task<CompraDto?> GetByIdAsync(int id);
        Task<CompraDto> CreateAsync(CreateCompraDto createCompraDto);
    }
}
