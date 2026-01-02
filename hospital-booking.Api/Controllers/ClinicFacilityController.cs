using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicFacilityController : ControllerBase
    {
        private readonly IClinicFacilityService _clinicFacilityService;

        public ClinicFacilityController(IClinicFacilityService clinicFacilityService)
        {
            _clinicFacilityService = clinicFacilityService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFacility(int id)
        {
            var result = await _clinicFacilityService.GetFacilityAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<IActionResult> GetFacilitiesByClinic(int clinicId)
        {
            var result = await _clinicFacilityService.GetFacilitiesByClinicAsync(clinicId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFacility([FromBody] ClinicFacilityAddDto dto)
        {
            var result = await _clinicFacilityService.CreateFacilityAsync(dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFacility(int id, [FromBody] ClinicFacilityUpdateDto dto)
        {
            var result = await _clinicFacilityService.UpdateFacilityAsync(id, dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var result = await _clinicFacilityService.DeleteFacilityAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
