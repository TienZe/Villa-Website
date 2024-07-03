using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using VillaAPI;
using VillaAPI.Data;
using VillaAPI.Infrastructure;
using VillaAPI.Models;
using VillaAPI.Repository;
using VillaAPI.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>(); 
builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => {
        options.InvalidModelStateResponseFactory = context => {
            var modelBindingAndValidationError = context.ModelState.ToErrorDictionary();
            return new BadRequestObjectResult(APIResponse.BadRequest(
                validationErrors: modelBindingAndValidationError
            ));
        };
    })
    .AddNewtonsoftJson();
                    
// Identity's default authentication is cookie authentication
// Must place this config before AddIdentity to override the authentication scheme
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["ApiSettings:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddResponseCaching();

// Register Repositories
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register SeedData
builder.Services.AddScoped<SeedData>();

builder.Services.AddSingleton<FileService>();

// Register configuration for SwaggerGenOptions
builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply seed data
using (var scope = app.Services.CreateScope()) 
{
    var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();
    await seedData.SeedRoles();
    await seedData.SeedAdminAccount();
    await seedData.SeedExampleData();
}


app.Run();