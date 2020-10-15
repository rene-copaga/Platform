using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Services;
using System;

namespace Platform
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        private IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IResponseFormatter>(serviceProvider => {
                string typeName = Configuration["services:IResponseFormatter"];
                return (IResponseFormatter)ActivatorUtilities
                .CreateInstance(serviceProvider, typeName == null
                ? typeof(GuidService) : Type.GetType(typeName, true));
            });
            services.AddScoped<ITimeStamper, DefaultTimeStamper>();
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
                    IResponseFormatter formatter 
                        = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context,
                        "Middleware Function: It is snowing in Chicago");
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapEndpoint<WeatherEndpoint>("/endpoint/class");

                endpoints.MapGet("/endpoint/function", async context => {
                    IResponseFormatter formatter
                        = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context, 
                        "Endpoint Function: It is sunny in LA");
                });
            });
        }
    }
}
