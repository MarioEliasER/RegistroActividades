using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegistroActividades.Helpers;
using RegistroActividades.Models.Entities;
using RegistroActividades.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("registroactividades");
builder.Services.AddDbContext<ItesrcneActividadesContext>(x =>
x.UseMySql(connection, ServerVersion.AutoDetect(connection)));

builder.Services.AddTransient<ActividadesRepository>();
builder.Services.AddSingleton<JwtHelper>();

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    var issuer = builder.Configuration.GetSection("JwtBearer").GetValue<string>("Issuer");
    var audience = builder.Configuration.GetSection("JwtBearer").GetValue<string>("Audience");
    var secret = builder.Configuration.GetSection("JwtBearer").GetValue<string>("Secret");

    x.TokenValidationParameters = new()
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")),
        ValidateLifetime = true
    };
});
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

app.UseDeveloperExceptionPage();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
