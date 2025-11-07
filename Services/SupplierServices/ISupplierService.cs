using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.SupplierDtos;
using EMS_Backend.Entities;

namespace EMS_Backend.Services.SupplierServices
{
    public interface ISupplierService
    {
        Task<ServiceResponse<PaginationResponse<SupplierResponse>>> GetAllSupplierAsync(PaginationParams @params);
        Task<ServiceResponse<SupplierResponse>> GetSupplierByIdAsync(string id);
        Task<ServiceResponse<SupplierResponse>> AddSupplierAsync(SupplierRequest request);
        Task<ServiceResponse<SupplierResponse>> UpdateSupplierAsync(SupplierRequest request);
        Task<ServiceResponse<SupplierResponse>> DeleteSupplierAsync(string id);
    }
}
