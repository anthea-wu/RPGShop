using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string server = Configuration["DatabaseServer"];
            string database = Configuration["DatabaseName"];
            string user = Configuration["DatabaseUser"];
            string password = Configuration["DatabasePassword"];
            string connectionString = string.Format("Server={0};Database={1};User={2};Password={3};",
                server, database, user, password);

            services.AddControllers();
            services.AddDbContext<CatalogContext>(options => options.UseSqlServer(connectionString));

            // 將 appsettinss.json 中的資訊加載到 CatalogSettings 中
            services.Configure<CatalogSettings>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
