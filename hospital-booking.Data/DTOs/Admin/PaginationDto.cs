namespace hospital_booking.Data.DTOs.Admin;
public class PaginationDto
{
    public int Page { get; set; }
    public int CurrentPage { get => Page; set => Page = value; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int TotalItems { get => TotalCount; set => TotalCount = value; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}