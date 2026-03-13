using Moq;
using Application;
using Application.Interfaces;
using Domain;
using Microsoft.Extensions.Logging;

namespace UnitTests
{
    public class ServerServiceTests
    {
        private readonly Mock<IServerRepository> _repoMock;
        private readonly ServerService _service;

        public ServerServiceTests()
        {
            _repoMock = new Mock<IServerRepository>();
            _service = new ServerService(_repoMock.Object, new Mock<ILogger<ServerService>>().Object);
        }

        [Fact]
        public async Task RentServerAsync_ShouldStartPoweringOn_WhenServerIsOff()
        {
            var serverId = Guid.NewGuid();
            var server = new ServerEntity { Id = serverId, State = ServerState.Available, IsPoweredOn = false };
            _repoMock.Setup(r => r.GetByIdAsync(serverId)).ReturnsAsync(server);

            var result = await _service.RentServerAsync(serverId);

            Assert.Equal(ServerState.PoweringOn, result.State);
            Assert.True(server.ReadyAt.HasValue);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<ServerEntity>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RentServerAsync_ShouldRentImmediately_WhenServerIsOn()
        {
            var serverId = Guid.NewGuid();
            var server = new ServerEntity { Id = serverId, State = ServerState.Available, IsPoweredOn = true };
            _repoMock.Setup(r => r.GetByIdAsync(serverId)).ReturnsAsync(server);

            var result = await _service.RentServerAsync(serverId);

            Assert.Equal(ServerState.Rented, result.State);
            Assert.False(server.ReadyAt.HasValue);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<ServerEntity>()), Times.Once);
        }

        [Fact]
        public async Task RentServerAsync_ShouldThrowException_WhenServerNotAvailable()
        {
            var serverId = Guid.NewGuid();
            var server = new ServerEntity { Id = serverId, State = ServerState.Rented };
            _repoMock.Setup(r => r.GetByIdAsync(serverId)).ReturnsAsync(server);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RentServerAsync(serverId));
        }
    }
}