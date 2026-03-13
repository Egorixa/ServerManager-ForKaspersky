using Application.DTOs;

namespace Application.Interfaces
{
    public interface IServerService
    {
        Task<Guid> AddServerAsync(CreateServerDto dto);
        Task<IEnumerable<ServerDto>> GetAvailableServersAsync(ServerSearchFilterDto filter);
        Task<ServerDto> RentServerAsync(Guid id);
        Task ReleaseServerAsync(Guid id);
        Task<bool> IsServerReadyAsync(Guid id);
    }
}