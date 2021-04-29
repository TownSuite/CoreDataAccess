using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TownSuite.CoreWebAPI.Repository;
using TownSuite.CoreWebAPI.Service;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreWebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private static List<AppDbConnectionVM> _appDbConnections;
        private static List<AppDbTenant> _appDbTenants;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Direct read the section then we have to write a tenant resolver class before get this section
            _appDbTenants = Configuration.GetSection("AppTenants").Get<List<AppDbTenant>>();   
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string tenantId = "023728"; // Middleware request pipeline get the tenantId from header and push into here
            //services.Configure<List<AppDbConnectionVM>>(Configuration.GetSection("AppTenantConfig"));
            services.AddControllers();
            services.AddScoped<IUserRepository>(c=>new UserRepository());
            services.AddScoped<IUserService>(c => new UserService(new UserRepository(), GetAppDbConnections(tenantId,_appDbTenants)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ///Loading Static and default files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// This method behave as same as App Tenant resolver does
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="appDbTenants"></param>
        /// <returns></returns>
        private IEnumerable<AppDbConnectionVM> GetAppDbConnections(string tenantId, List<AppDbTenant> appDbTenants)
        {
            IEnumerable<AppDbConnectionVM> result = null;
            appDbTenants.ForEach(each =>
            {
                if (each.TenantId.Equals(tenantId))
                    result = each.AppDbConnections;
            });
            return result;
        }
    }
}
