namespace hospital_booking.Data.DTOs.Admin;
public class AdminsRequestDto
{
  
    public int Page { get; set; }
    public int Limit { get; set; }
    public string? SearchQuery{get;set;}
    public bool? IsActive{get;set;}
    public string? Role{get;set;}
    
}