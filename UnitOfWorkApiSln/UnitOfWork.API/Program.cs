using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UnitOfWork.API.configuration;
using UnitOfWork.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // no use

builder.Services.AddDbContext<DBContext>(o => o.UseSqlite("Data Source=developmentDB.db"));

builder.Services.AddScoped<IUnitOfWork, UnitofWork>();



builder.Services.AddSwaggerGen(c =>
{
   // c.IncludeXmlComments(string.Format(@"{0}\CQRS.WebApi.xml", System.AppDomain.CurrentDomain.BaseDirectory));
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UOW_101", Version = "v1" });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UOW_101 v1"));

app.UseAuthorization();

app.MapControllers();

app.Run();
