
using Application;
using Application.Interfaces;
using CaseService.API.CaseService.Application.Interfaces;
using CaseService.API.CaseService.Infrastructure.Messaging;
using CaseService.API.CaseService.Infrastructure.Repositories;
using Infrastructure;
using Infrastructure.Messaging;
using Infrastructure.Options;
using MassTransit;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1) Add Swagger for API exploration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")   // your React app URL
                .AllowAnyMethod()                       // GET, POST, PUT, DELETE…
                .AllowAnyHeader()                       // Content-Type, Authorization…
                .AllowCredentials();                    // if you ever send cookies/auth
        });
});




builder.Services.AddControllers();




builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowReactDev");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

