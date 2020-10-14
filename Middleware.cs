﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Platform
{
    public class QueryStringMiddleWare
    {
        private RequestDelegate next;
        public QueryStringMiddleWare(RequestDelegate nextDelegate)
        {
            next = nextDelegate;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Get
            && context.Request.Query["custom"] == "true")
            {
                await context.Response.WriteAsync("Class-based Middleware \n");
            }
            if (next != null)
            {
                await next(context);
            }
        }

        public class LocationMiddleware
        {
            private RequestDelegate next;
            private MessageOptions options;
            public LocationMiddleware(RequestDelegate nextDelegate,
            IOptions<MessageOptions> opts)
            {
                next = nextDelegate;
                options = opts.Value;
            }
            public async Task Invoke(HttpContext context)
            {
                if (context.Request.Path == "/location")
                {
                    await context.Response
                    .WriteAsync($"{options.CityName}, {options.CountryName}");
                }
                else
                {
                    await next(context);
                }
            }
        }
    }
}
