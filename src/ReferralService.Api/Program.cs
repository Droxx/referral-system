using Microsoft.Extensions.Options;
using ReferralService.Core.Settings;
using ReferralService.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddApiServices();

var serviceUris = new ServiceUrisSettings();
builder.Configuration.GetSection("ServiceUris").Bind(serviceUris);

var section = builder.Configuration.GetSection("ServiceUris");
builder.Services.Configure<ServiceUrisSettings>(section);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ServiceUrisSettings>>().Value);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();