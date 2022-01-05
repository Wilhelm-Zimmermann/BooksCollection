using backend.Database;
using backend.Repositories;
using backend.Services;
using backend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(TokenSettings.Secret);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy => policy.RequireRole("user"));
});

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IFileManager, FileManager>();


builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseContext"))
);

var AllowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,

    builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Images", "Books")),
    RequestPath = "/book/image"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Images", "Users")),
    RequestPath = "/user/image"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
