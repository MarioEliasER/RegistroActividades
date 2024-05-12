using Microsoft.EntityFrameworkCore;
using RegistroActividades.Models.Entities;
using RegistroActividades.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ItesrcneActividadesContext>(x =>
x.UseMySql("server=204.93.216.11;database=itesrcne_actividades;user=itesrcne_deptos;password=sistemaregistrotec24",
Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb")));

builder.Services.AddTransient<ActividadesRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
