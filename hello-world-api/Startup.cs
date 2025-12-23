using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Alias to avoid ambiguity (keep this)
using HostingEnv = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace hello_world_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 🔑 THIS IS THE FIX
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
        }

        public void Configure(IApplicationBuilder app, HostingEnv env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
