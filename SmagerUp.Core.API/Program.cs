using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmagerUp.Core.API.Services;
using ApiData=SmagerUp.Core.API.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Configure JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

// ✅ Dependency Injection
builder.Services.AddSingleton<ApiData.Core.CoreDapperContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClientDbResolver, ApiData.Client.ClientDbResolver>();

builder.Services.AddScoped<ApiData.Core.HostRepository>();
builder.Services.AddScoped<ApiData.Core.ClientRepository>();
builder.Services.AddScoped<ApiData.Core.LicenseTypeRepository>();
builder.Services.AddScoped<ApiData.Core.ComponentRepository>();
builder.Services.AddScoped<ApiData.Core.ResourceRepository>();

builder.Services.AddScoped<ApiData.Client.UserRepository>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddDataProtection();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

//builder.Services.AddScoped<AccountLicenseRepository>();


// ✅ CORS Policy
builder.Services.AddCors(p =>
    p.AddPolicy("AllowAll", b =>
        b.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod()
    )
);

// ✅ Build App
var app = builder.Build();

// ✅ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();   // Required for [Authorize]
app.UseAuthorization();
app.MapControllers();

app.Run();


