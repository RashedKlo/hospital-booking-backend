using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.ClinicReview.Commands;
using hospital_booking.Data.Repositories.ClinicReview.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.ClinicReview
{
    public class ClinicReviewRepository : IClinicReviewRepository
    {
        private readonly ILogger<ClinicReviewRepository> _logger;

        public ClinicReviewRepository(ILogger<ClinicReviewRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateReviewAsync(ClinicReviewAddDto dto)
        {
            return await CreateReviewCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<ClinicReviewDto>> UpdateReviewAsync(int reviewId, ClinicReviewUpdateDto dto)
        {
            return await UpdateReviewCommand.ExecuteAsync(reviewId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteReviewAsync(int reviewId)
        {
            return await DeleteReviewCommand.ExecuteAsync(reviewId, _logger);
        }

        public async Task<OperationResult<ClinicReviewDto>> GetReviewAsync(int reviewId)
        {
            return await GetReviewQuery.ExecuteAsync(reviewId, _logger);
        }

        public async Task<OperationResult<ClinicReviewsDto>> GetReviewsAsync(ClinicReviewsRequestDto requestDto)
        {
            return await GetReviewsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
