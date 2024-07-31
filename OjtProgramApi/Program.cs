using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OjtProgramApi.CustomMiddleware;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;
using Serilog;

Log.Logger = new LoggerConfiguration() // serilog configuration
    .WriteTo.Console()
    .CreateBootstrapLogger();
Log.Information("WebApi starting up");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSerilog(
    (services, lc) =>
        lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
);

// Add services to the container.
// builder.Services.AddDbContext<TodoContext>(opt =>
//     opt.UseInMemoryDatabase("TodoList"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDB>(opt =>
    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        // The following three options help with debugging, but should
        // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
            ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["TokenAuthentication:SecretKey"])
            )
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Roleid", "1"));//

    options.AddPolicy("InstructorPolicy", policy => policy.RequireClaim("Roleid", "2"));

    options.AddPolicy("StudentPolicy", policy => policy.RequireClaim("Roleid", "3"));
});

builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
builder.Services.AddTransient<AuthorizeUserMiddleware>();
var app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorizeUser();

app.MapControllers();

app.Run();

