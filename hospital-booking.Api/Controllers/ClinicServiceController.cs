using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicServiceController : ControllerBase
    {
        private readonly IClinicServicesService _clinicServicesService;

        public ClinicServiceController(IClinicServicesService clinicServicesService)
        {
            _clinicServicesService = clinicServicesService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            var result = await _clinicServicesService.GetServiceAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetServices([FromQuery] ClinicServicesRequestDto requestDto)
        {
            var result = await _clinicServicesService.GetServicesAsync(requestDto);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ClinicServiceAddDto dto)
        {
            var result = await _clinicServicesService.CreateServiceAsync(dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ClinicServiceUpdateDto dto)
        {
            var result = await _clinicServicesService.UpdateServiceAsync(id, dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _clinicServicesService.DeleteServiceAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
