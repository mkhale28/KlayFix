using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Application.Services;

public class RepairService : IRepairService
{
    private readonly IRepairRepository _repo;

    public RepairService(IRepairRepository repo) => _repo = repo;

    public async Task<List<RepairDto>> GetAllAsync()
    {
        var repairs = await _repo.GetAllAsync().ConfigureAwait(false);
        return repairs.Select(MapToDto).ToList();
    }

    public async Task<RepairDto?> GetByIdAsync(int id)
    {
        var repair = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        return repair is null ? null : MapToDto(repair);
    }

    public async Task<RepairDto> CreateAsync(CreateRepairDto dto)
    {
        var repair = new Repair
        {
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            DeviceModel = dto.DeviceModel,
            DeviceIMEI = dto.DeviceIMEI,
            IssueDescription = dto.IssueDescription,
            Status = RepairStatus.Received,
            Cost = dto.Cost,
            Charge = dto.Charge,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };
        repair = await _repo.AddAsync(repair).ConfigureAwait(false);
        return MapToDto(repair);
    }

    public async Task<RepairDto> UpdateStatusAsync(UpdateRepairStatusDto dto)
    {
        var repair = await _repo.GetByIdAsync(dto.Id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Repair not found");

        repair.Status = Enum.Parse<RepairStatus>(dto.Status, true);

        if (repair.Status == RepairStatus.Completed)
            repair.CompletedAt = DateTime.UtcNow;

        if (repair.Status == RepairStatus.Delivered)
            repair.DeliveredAt = DateTime.UtcNow;

        if (dto.Notes is not null)
            repair.Notes = dto.Notes;

        repair = await _repo.UpdateAsync(repair).ConfigureAwait(false);
        return MapToDto(repair);
    }

    public async Task<List<RepairDto>> GetByStatusAsync(string status)
    {
        var repairs = await _repo.GetByStatusAsync(status).ConfigureAwait(false);
        return repairs.Select(MapToDto).ToList();
    }

    public async Task<List<RepairDto>> SearchAsync(string query)
    {
        var repairs = await _repo.SearchAsync(query).ConfigureAwait(false);
        return repairs.Select(MapToDto).ToList();
    }

    private static RepairDto MapToDto(Repair r) => new()
    {
        Id = r.Id,
        CustomerName = r.CustomerName,
        CustomerPhone = r.CustomerPhone,
        DeviceModel = r.DeviceModel,
        DeviceIMEI = r.DeviceIMEI,
        IssueDescription = r.IssueDescription,
        Status = r.Status.ToString(),
        Cost = r.Cost,
        Charge = r.Charge,
        CreatedAt = r.CreatedAt,
        CompletedAt = r.CompletedAt,
        DeliveredAt = r.DeliveredAt,
        Notes = r.Notes
    };
}
