using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Admin.Helpers;

namespace hospital_booking.Data.Repositories.Admin.Queries
{
    public class GetAdminsQuery
    {
        public static async Task<OperationResult<AdminsDto>> ExecuteAsync(
            AdminsRequestDto requestDto, 
            ILogger logger)
        {
            // Validation
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetAdminsQuery received invalid params");
                return OperationResult<AdminsDto>.Failure("Invalid parameters");
            }

            try 
            {
                // Build dynamic SQL and parameters
                var (sql, parameters) = BuildQuery(requestDto);

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                
                // Add all parameters
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

                using var reader = await command.ExecuteReaderAsync();
                
                // Read total count from first result set
                int totalCount = 0;
                if (await reader.ReadAsync())
                {
                    totalCount = reader.GetInt32(0);
                }

                // Move to second result set (admin data)
                await reader.NextResultAsync();

                var admins = new List<AdminDto>();
                while (await reader.ReadAsync())
                {
                    admins.Add(AdminMapper.MapFromReader(reader));
                }

                // Build response with pagination
                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var adminsDto = new AdminsDto
                {
                    Admins = admins,
                    Pagination = new PaginationDto
                    {
                        Page = requestDto.Page,
                        PageSize = requestDto.Limit,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasPrevious = requestDto.Page > 1,
                        HasNext = requestDto.Page < totalPages
                    }
                };

                logger.LogInformation("Retrieved {Count} admins (Page {Page}/{TotalPages})", 
                    admins.Count, requestDto.Page, totalPages);

                return OperationResult<AdminsDto>.Success(adminsDto, "Admins retrieved successfully");
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error retrieving admins: {Error}", ex.Message);
                return OperationResult<AdminsDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving admins: {Error}", ex.Message);
                return OperationResult<AdminsDto>.Failure("An unexpected error occurred");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(AdminsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            // Add pagination parameters (always required)
            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            // Build WHERE conditions dynamically
            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                // Sanitize search input to prevent SQL injection
                var sanitizedSearch = SanitizeSearchInput(request.SearchQuery);
                whereConditions.Add("(full_name LIKE @SearchQuery OR email LIKE @SearchQuery)");
                parameters.Add("@SearchQuery", $"%{sanitizedSearch}%");
            }

            if (request.IsActive.HasValue)
            {
                whereConditions.Add("is_active = @IsActive");
                parameters.Add("@IsActive", request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                whereConditions.Add("role = @Role");
                parameters.Add("@Role", request.Role.Trim());
            }

            // Combine WHERE conditions with AND
            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            // Build complete SQL with both COUNT and SELECT
            var sql = $@"
-- Get total count
SELECT COUNT(*) 
FROM dbo.admins
{whereClause};

-- Get paginated data
SELECT
    admin_id,
    full_name,
    email,
    role,
    phone,
    is_active,
    created_at,
    updated_at
FROM dbo.admins
{whereClause}
ORDER BY admin_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";

            return (sql, parameters);
        }

        /// <summary>
        /// Sanitizes search input to prevent SQL injection attacks
        /// </summary>
        private static string SanitizeSearchInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Escape SQL LIKE wildcards and special characters
            return input
                .Replace("[", "[[]")  // Escape bracket
                .Replace("%", "[%]")  // Escape percent (wildcard)
                .Replace("_", "[_]")  // Escape underscore (single char wildcard)
                .Trim();              // Remove leading/trailing whitespace
        }
    }
}