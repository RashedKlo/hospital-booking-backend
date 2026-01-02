using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Services.Interfaces
{
    public interface IClinicReviewService
    {
        Task<OperationResult<ClinicReviewDto>> GetReviewAsync(int reviewId);
        Task<OperationResult<ClinicReviewsDto>> GetReviewsAsync(ClinicReviewsRequestDto requestDto);
        Task<OperationResult<bool>> CreateReviewAsync(ClinicReviewAddDto dto);
        Task<OperationResult<ClinicReviewDto>> UpdateReviewAsync(int reviewId, ClinicReviewUpdateDto dto);
        Task<OperationResult<bool>> DeleteReviewAsync(int reviewId);
    }
}
