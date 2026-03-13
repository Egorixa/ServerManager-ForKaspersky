using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class ServerApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ServerApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddServer_ShouldReturnOk_AndCreateServer()
        {
            var dto = new CreateServerDto
            {
                OsName = "Ubuntu",
                RamGb = 16,
                CpuCores = 4,
                DiskGb = 500,
                IsPoweredOn = true
            };

            var response = await _client.PostAsJsonAsync("/api/servers", dto);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAvailable_ShouldReturnSuccess()
        {
            var response = await _client.GetAsync("/api/servers/available?OsName=Ubuntu");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}