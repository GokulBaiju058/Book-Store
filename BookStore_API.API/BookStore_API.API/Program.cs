using BookStore_API.API.Middleware;
using BookStore_API.Business.Abstractions;
using BookStore_API.Business.LoginAggregate;
using BookStore_API.Business.Mapper;
using BookStore_API.Business.Services.Logger;
using BookStore_API.Business.UserAggregate;
using BookStore_API.Data;
using BookStore_API.Repositories;
using BookStore_API.Repositories.Abstractions;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
builder.Host.UseSerilog(((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration)));

var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
var generalMapper = new GeneralMapper();
generalMapper.Register(typeAdapterConfig);
builder.Services.AddSingleton<IMapper>(new Mapper(typeAdapterConfig));

// Configure DbContext with SQL Server
builder.Services.AddDbContext<BookStore_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreDev")));

// Configure Services
builder.Services.AddSingleton<IExceptionLoggerService, ExceptionLoggerService>();
builder.Services.AddSingleton<IAPILoggerService, APILoggerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:JwtBearer:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Authentication:JwtBearer:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JwtBearer:SecurityKey"]))
    };
});
builder.Services.AddSwaggerGen(option =>
{

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
});

var app = builder.Build();

app.UseMiddleware<Middleware>();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();  // Enable authorization middleware
app.MapControllers();

app.Run();
