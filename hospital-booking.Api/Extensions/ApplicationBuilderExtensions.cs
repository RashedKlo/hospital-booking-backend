namespace hospital_booking.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app, string policyName)
        {
            app.UseCors(policyName);
            return app;
        }
    }
}