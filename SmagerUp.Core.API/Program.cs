using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmagerUp.Core.API.Extensions;
using SmagerUp.Core.API.Services;
using System.Text;


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
builder.Services.AddHttpContextAccessor();
builder.Services.AddSmagerUpCore(builder.Environment);


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
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();// For detailed error information during development
app.UseAuthentication();   // Required for [Authorize]
app.UseAuthorization();
app.MapControllers();

app.Run();


