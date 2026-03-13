using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.Extensions.Logging;

namespace Application
{
    public class ServerService : IServerService
    {
        private readonly IServerRepository _repository;
        private readonly ILogger<ServerService> _logger;

        public ServerService(IServerRepository repository, ILogger<ServerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Guid> AddServerAsync(CreateServerDto dto)
        {
            var server = new ServerEntity
            {
                Id = Guid.NewGuid(),
                OsName = dto.OsName,
                RamGb = dto.RamGb,
                CpuCores = dto.CpuCores,
                DiskGb = dto.DiskGb,
                IsPoweredOn = dto.IsPoweredOn,
                State = ServerState.Available
            };

            await _repository.AddAsync(server);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Добавлен новый сервер в пул. ID: {Id}", server.Id);
            return server.Id;
        }

        public async Task<IEnumerable<ServerDto>> GetAvailableServersAsync(ServerSearchFilterDto filter)
        {
            var servers = await _repository.GetAvailableServersAsync(filter);
            return servers.Select(MapToDto);
        }

        public async Task<ServerDto> RentServerAsync(Guid id)
        {
            var server = await _repository.GetByIdAsync(id);
            if (server == null || server.State != ServerState.Available)
            {
                _logger.LogWarning("Попытка аренды недоступного сервера. ID: {Id}", id);
                throw new InvalidOperationException("Сервер недоступен для аренды.");
            }

            if (!server.IsPoweredOn)
            {
                server.StartPoweringOn(TimeSpan.FromMinutes(5));
                _logger.LogInformation("Сервер {Id} начал процесс включения.", id);
            }
            else
            {
                server.Rent();
                _logger.LogInformation("Сервер {Id} выдан в аренду мгновенно.", id);
            }

            await _repository.UpdateAsync(server);
            await _repository.SaveChangesAsync();

            return MapToDto(server);
        }

        public async Task ReleaseServerAsync(Guid id)
        {
            var server = await _repository.GetByIdAsync(id);
            if (server != null && server.State == ServerState.Rented)
            {
                server.Release();
                await _repository.UpdateAsync(server);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Сервер {Id} успешно освобожден.", id);
            }
        }

        public async Task<bool> IsServerReadyAsync(Guid id)
        {
            var server = await _repository.GetByIdAsync(id);
            if (server == null) return false;

            if (server.State == ServerState.PoweringOn && server.ReadyAt.HasValue && DateTime.UtcNow >= server.ReadyAt.Value)
            {
                server.Rent();
                await _repository.UpdateAsync(server);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Сервер {Id} включился и автоматически переведен в статус аренды.", id);
                return true;
            }

            return server.State == ServerState.Rented;
        }

        private ServerDto MapToDto(ServerEntity server)
        {
            return new ServerDto
            {
                Id = server.Id,
                OsName = server.OsName,
                RamGb = server.RamGb,
                CpuCores = server.CpuCores,
                DiskGb = server.DiskGb,
                State = server.State,
                IsPoweredOn = server.IsPoweredOn,
                ReadyAt = server.ReadyAt
            };
        }
    }
}