using hospital_booking.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add custom services
builder.Services.AddApplicationServices();
builder.Services.AddAuthenticationSchemes(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddCustomValidationResponse();

// Add CORS policy
string[] allowedOrigins = {
    "http://localhost:3000",
    "https://localhost:5173",
    "http://localhost:5193",
    "https://localhost:5193"
};
builder.Services.AddCorsPolicy("AllowSpecificOrigins", allowedOrigins);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "hospital_booking API V1");
        c.RoutePrefix = "swagger";
    });
}

// CRITICAL: CORS must come BEFORE HttpsRedirection
app.UseAppCors("AllowSpecificOrigins");

// Skip HTTPS redirection in Development to allow local HTTP requests
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();