using Microsoft.EntityFrameworkCore;
using Domain;
using Application.DTOs;
using Application.Interfaces;

namespace Infrastructure
{
    public class ServerRepository : IServerRepository
    {
        private readonly AppDbContext _context;

        public ServerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServerEntity> GetByIdAsync(Guid id)
        {
            return await _context.Servers.FindAsync(id);
        }

        public async Task<IEnumerable<ServerEntity>> GetAvailableServersAsync(ServerSearchFilterDto filter)
        {
            var query = _context.Servers.Where(x => x.State == ServerState.Available).AsQueryable();

            if (!string.IsNullOrEmpty(filter.OsName))
                query = query.Where(x => x.OsName.Contains(filter.OsName));

            if (filter.MinRamGb.HasValue)
                query = query.Where(x => x.RamGb >= filter.MinRamGb.Value);

            if (filter.MinCpuCores.HasValue)
                query = query.Where(x => x.CpuCores >= filter.MinCpuCores.Value);

            if (filter.MinDiskGb.HasValue)
                query = query.Where(x => x.DiskGb >= filter.MinDiskGb.Value);

            return await query.ToListAsync();
        }

        public async Task AddAsync(ServerEntity server)
        {
            await _context.Servers.AddAsync(server);
        }

        public async Task UpdateAsync(ServerEntity server)
        {
            _context.Servers.Update(server);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var server = (ServerEntity)entry.Entity;
                throw new ConcurrentRentException(server.Id);
            }
        }
        public async Task<IEnumerable<ServerEntity>> GetExpiredRentedServersAsync(DateTime threshold)
        {
            return await _context.Servers
                .Where(x => x.State == ServerState.Rented && x.RentedAt <= threshold)
                .ToListAsync();
        }
    }
}