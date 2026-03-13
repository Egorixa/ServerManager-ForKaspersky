using Domain;

namespace Application.Interfaces
{
    public interface IServerRepository
    {
        Task<ServerEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<ServerEntity>> GetAvailableServersAsync(DTOs.ServerSearchFilterDto filter);
        Task AddAsync(ServerEntity server);
        Task UpdateAsync(ServerEntity server);
        Task SaveChangesAsync();
        Task<IEnumerable<ServerEntity>> GetExpiredRentedServersAsync(DateTime threshold);
    }
}