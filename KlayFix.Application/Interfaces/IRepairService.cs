using KlayFix.Application.DTOs;

namespace KlayFix.Application.Interfaces;

public interface IRepairService
{
    Task<List<RepairDto>> GetAllAsync();
    Task<RepairDto?> GetByIdAsync(int id);
    Task<RepairDto> CreateAsync(CreateRepairDto dto);
    Task<RepairDto> UpdateStatusAsync(UpdateRepairStatusDto dto);
    Task<List<RepairDto>> GetByStatusAsync(string status);
    Task<List<RepairDto>> SearchAsync(string query);
}
