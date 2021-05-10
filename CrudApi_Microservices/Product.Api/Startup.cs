using CRUD.Transaction.CRUDApi.Core;
using CRUD.Transaction.CRUDApi.Core.Context;
using CRUD.Transaction.CRUDApi.Core.Repository;
using CRUD.Transaction.Product.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRUD.Transaction.Product.Api
{
    public class Startup : ApiApplicationBuilder
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            base.StartUp(configuration, env);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // Module/Sub-Module specific context          

            services.AddScoped<IAsyncContext<Entities.Product>, ProductContext>();
            services.AddScoped<IRepositoryAsync<Entities.Product>, ProductRepository>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }
    }
}
