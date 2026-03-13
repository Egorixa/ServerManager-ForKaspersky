using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly IServerService _service;

        public ServersController(IServerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateServerDto dto)
        {
            var id = await _service.AddServerAsync(dto);
            return Ok(id);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable([FromQuery] ServerSearchFilterDto filter)
        {
            var servers = await _service.GetAvailableServersAsync(filter);
            return Ok(servers);
        }

        [HttpPost("{id}/rent")]
        public async Task<IActionResult> Rent(Guid id)
        {
            try
            {
                var server = await _service.RentServerAsync(id);
                return Ok(server);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/release")]
        public async Task<IActionResult> Release(Guid id)
        {
            await _service.ReleaseServerAsync(id);
            return Ok();
        }

        [HttpGet("{id}/ready")]
        public async Task<IActionResult> IsReady(Guid id)
        {
            var isReady = await _service.IsServerReadyAsync(id);
            return Ok(isReady);
        }
    }
}