using AspNetCoreRateLimit;

using FluentEmail.Core;
using FluentEmail.Mailgun;

using vjksolutions.api.Infrastructure;
using vjksolutions.api.Models;

var builder = WebApplication.CreateBuilder(args);

// app settings

var section = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(section);
var settings = section.Get<AppSettings>();

// CORS

var origins = builder.Configuration.GetSection("CORSOrigins").Get<string[]>();
if (origins is null || !origins.Any())
{
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(builder =>
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyHeader()));
}
else
{
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(builder =>
            builder.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()));
}

// fluent email with Mailgun

builder.Services.AddFluentEmail(settings.Sender)
    .AddMailGunSender(settings.Domain, settings.ServerKey);
var sender = new MailgunSender(settings.Domain, settings.ServerKey);
Email.DefaultSender = sender;

// miscellaneous

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(builder.Environment);

// rate limiting

builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimit"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors();

app.UseIpRateLimiting();

app.MapEndpoints();

app.Run();
