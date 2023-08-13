using System.Text;
using EventProvider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManagement;
using TaskManagement.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new Producer());
builder.Services.AddSingleton(new UserRepository());
builder.Services.AddSingleton<TaskRepository>();
builder.Services.AddSingleton<TaskService>();
builder.Services.AddHostedService<ConsumerService>();


builder.Services
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
                     {
                         options.TokenValidationParameters = new TokenValidationParameters
                                                             {
                                                                 ValidateIssuer = false,
                                                                 ValidateAudience = false,
                                                                 ValidateLifetime = false,
                                                                 ValidateIssuerSigningKey = true,

                                                                 // ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                                                                 // ValidAudience = builder.Configuration["JwtSettings:Audience"],
                                                                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
                                                             };
                     });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();