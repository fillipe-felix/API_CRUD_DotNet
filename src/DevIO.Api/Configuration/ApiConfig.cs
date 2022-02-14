using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Configuration;

public static class ApiConfig
{
    public static IServiceCollection WebApiConfig(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddCors(options =>
        {
            options.AddPolicy("Development", builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            //.AllowCredentials());
        });

        return services;
    }

    public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseCors("Development");
        app.UseMvc();

        return app;
    }
}
