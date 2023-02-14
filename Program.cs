using API_Movies.Data;
using API_Movies.Repository;
using API_Movies.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using API_Movies.MoviesMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API_Movies.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MySQLConn");

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// Caché
builder.Services.AddResponseCaching();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// CORS
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

//Auto mapper
builder.Services.AddAutoMapper(typeof(MoviesMapper));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers(option =>
{
    option.CacheProfiles.Add("Default10Sec", new CacheProfile() { Duration = 10});
});

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());

    options.SwaggerDoc("categories", new OpenApiInfo
    {
        Title = "categories",
        Version = "v1",
    });

    options.SwaggerDoc("movies", new OpenApiInfo
    {
        Title = "movies",
        Version = "v1",
    });

    options.SwaggerDoc("users", new OpenApiInfo
    {
        Title = "users",
        Version = "v1",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticando JWT usando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Beare' seguida de un [espacio] y luego su token en el campo de abajo \r\n\r\n" + 
        "Ej: \"Bearer tdadahsdh\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/moviesapi/swagger/categories/swagger.json", "categories");
//        options.SwaggerEndpoint("/moviesapi/swagger/movies/swagger.json", "movies");
//        options.SwaggerEndpoint("/moviesapi/swagger/users/swagger.json", "users");
//    });
//}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/categories/swagger.json", "categories");
    options.SwaggerEndpoint("/swagger/movies/swagger.json", "movies");
    options.SwaggerEndpoint("/swagger/users/swagger.json", "users");
});

app.UseRouting();

app.UseHttpsRedirection();

// CORS
app.UseCors("PolicyCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
