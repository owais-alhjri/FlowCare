using System.Text.Json.Serialization;
using FlowCare.API.Authentication;
using FlowCare.Infrastructure.Extensions;
using FlowCare.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Enter your name and password"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicAuth"
                }
            },
            []
        }
    });
});


builder.Services.AddDbContext<FlowCareDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("BasicAuth")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuth", null);

builder.Services.AddApplicationServices();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("ManagerOrAbove", policy => policy.RequireRole("ADMIN", "BRANCH_MANAGER"));
    options.AddPolicy("StaffOrAbove", policy => policy.RequireRole("ADMIN", "BRANCH_MANAGER", "STAFF"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("CUSTOMER"));
    options.AddPolicy("AnyAuthenticatedUser",
        policy => policy.RequireRole("CUSTOMER", "ADMIN", "BRANCH_MANAGER", "STAFF"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddMinio(config => config
    .WithEndpoint(builder.Configuration["MinIO:Endpoint"])
    .WithCredentials(builder.Configuration["MinIO:AccessKey"], builder.Configuration["MinIO:SecretKey"])
    .WithSSL(false)
    .Build());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FlowCareDbContext>();
    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();