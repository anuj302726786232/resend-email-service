using Resend;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .AllowAnyOrogin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddResend(options =>
{
    options.ApiToken = builder.Configuration["ResendEmail:API_KEY"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();

// CORS must be placed before authorization
app.UseCors("ReactPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
