using Proyec_tecn.DTOs;

namespace Proyec_tecn.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetAllAsync();
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<ProductoDto> CreateAsync(CreateProductoDto createProductoDto);
        Task<ProductoDto> UpdateAsync(UpdateProductoDto updateProductoDto);
        Task<bool> DeleteAsync(int id);
    }
}
