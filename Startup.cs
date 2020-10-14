using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using static Platform.QueryStringMiddleWare;

namespace Platform
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(opts => {
                opts.ConstraintMap.Add("countryName",
                    typeof(CountryRouteConstraint));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();

            app.Use(async (context, next) => {
                Endpoint end = context.GetEndpoint();
                if (end != null)
                {
                    await context.Response
                    .WriteAsync($"{end.DisplayName} Selected \n");
                }
                else
                {
                    await context.Response.WriteAsync("No Endpoint Selected \n");
                }
                await next();
            });

            app.UseEndpoints(endpoints => {
                endpoints.Map("{number:int}", async context => {
                    await context.Response.WriteAsync("Routed to the int endpoint");
                })
                .WithDisplayName("Int Endpoint")
                .Add(b => ((RouteEndpointBuilder)b).Order = 1); ;
                endpoints.Map("{number:double}", async context => {
                    await context.Response
                    .WriteAsync("Routed to the double endpoint");
                })
                .WithDisplayName("Double Endpoint")
                .Add(b => ((RouteEndpointBuilder)b).Order = 2);
            });

            app.Use(async (context, next) => {
                await context.Response.WriteAsync("Terminal Middleware Reached");
            });
        }
    }
}
