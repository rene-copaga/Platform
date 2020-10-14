using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Platform.Services;

namespace Platform
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseMiddleware<WeatherMiddleware>();

            app.Use(async (context, next) => {
                if (context.Request.Path == "/middleware/function")
                {
                    await TextResponseFormatter.Singleton.Format(context,
                    "Middleware Function: It is snowing in Chicago");
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapGet("/endpoint/class", WeatherEndpoint.Endpoint);
                endpoints.MapGet("/endpoint/function", async context => {
                    await TextResponseFormatter.Singleton.Format(context, 
                        "Endpoint Function: It is sunny in LA");
                });
            });
        }
    }
}
