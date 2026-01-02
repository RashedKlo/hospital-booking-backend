using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hospital_booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicReviewController : ControllerBase
    {
        private readonly IClinicReviewService _clinicReviewService;

        public ClinicReviewController(IClinicReviewService clinicReviewService)
        {
            _clinicReviewService = clinicReviewService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            var result = await _clinicReviewService.GetReviewAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] ClinicReviewsRequestDto requestDto)
        {
            var result = await _clinicReviewService.GetReviewsAsync(requestDto);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ClinicReviewAddDto dto)
        {
            var result = await _clinicReviewService.CreateReviewAsync(dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ClinicReviewUpdateDto dto)
        {
            var result = await _clinicReviewService.UpdateReviewAsync(id, dto);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _clinicReviewService.DeleteReviewAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
