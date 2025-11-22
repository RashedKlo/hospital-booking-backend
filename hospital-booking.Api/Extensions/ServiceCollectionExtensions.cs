using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.User;
using hospital_booking.Data.Repositories.Admin;
using hospital_booking.Data.Repositories.Appointment;
using hospital_booking.Data.Repositories.Clinic;
using hospital_booking.Data.Repositories.Doctor;
using hospital_booking.Data.Repositories.Patient;
using hospital_booking.Data.Repositories.Prescription;
using hospital_booking.Data.Repositories.PrescriptionItem;
using hospital_booking.Data.Repositories.MedicalReport;
using hospital_booking.Services.Interfaces;
using hospital_booking.Services;
using hospital_booking.Data.Results;
using hospital_booking.Api.Responses;
using hospital_booking.Services.User;
using hospital_booking.Services.Admin;
using hospital_booking.Services.Appointment;
using hospital_booking.Services.Clinic;
using hospital_booking.Services.Doctor;
using hospital_booking.Services.Patient;
using hospital_booking.Services.Prescription;
using hospital_booking.Services.PrescriptionItem;
using hospital_booking.Services.MedicalReport;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;

namespace hospital_booking.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IClinicRepository, ClinicRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
            services.AddScoped<IMedicalReportRepository, MedicalReportRepository>();

            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IPrescriptionItemService, PrescriptionItemService>();
            services.AddScoped<IMedicalReportService, MedicalReportService>();

            services.AddSingleton(JwtSettings.LoadFromConfiguration(
                services.BuildServiceProvider().GetRequiredService<IConfiguration>()));
            services.AddScoped<Data.Helpers.TokenHandler>();

            return services;
        }

        public static IServiceCollection AddAuthenticationSchemes(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = JwtSettings.LoadFromConfiguration(configuration);

            if (string.IsNullOrEmpty(jwtSettings.SigningKey))
                throw new InvalidOperationException("JWT SigningKey is required in appsettings.json");

            var key = Encoding.UTF8.GetBytes(jwtSettings.SigningKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            })
            .AddGoogle(googleOptions =>
            {
                var googleSettings = configuration.GetSection("Google");
                googleOptions.ClientId = googleSettings["ClientId"] ??
                    throw new InvalidOperationException("Google ClientId is required");
                googleOptions.ClientSecret = googleSettings["ClientSecret"] ??
                    throw new InvalidOperationException("Google ClientSecret is required");
                googleOptions.CallbackPath = "/api/auth/google-callback";
            });

            return services;
        }

        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "hospital_booking API",
                    Version = "v1",
                    Description = "API for hospital_booking application with authentication support"
                });

                // Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer {your-token}' (without quotes)"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string policyName, string[] allowedOrigins)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var response = OperationResult<object>.Failure("Validation failed", errors);

                    return new BadRequestObjectResult(new ErrorResponse(response.Message, response.Errors.ToList()));
                };
            });

            return services;
        }
    }
}