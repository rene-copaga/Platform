﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Platform.Models;
using Platform.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Platform
{
    public class SumEndpoint
    {
        public async Task Endpoint(HttpContext context,
                CalculationContext dataContext)
        {
            int count = int.Parse((string)context.Request.RouteValues["count"]);
            long total = dataContext.Calculations
                .FirstOrDefault(c => c.Count == count)?.Result ?? 0;
            if (total == 0)
            {
                for (int i = 1; i <= count; i++)
                {
                    total += i;
                }
                dataContext.Calculations!
                    .Add(new Calculation() { Count = count, Result = total });
                await dataContext.SaveChangesAsync();
            }
            string totalString = $"({ DateTime.Now.ToLongTimeString() }) {total}";
            await context.Response.WriteAsync(
                $"({DateTime.Now.ToLongTimeString()}) Total for {count}"
                + $" values:\n{totalString}\n");
        }
    }
}
