using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using RentalSystem.Backend.Services;
using RentalSystem.Shared;
using RentalSystem.Shared.AppConstants;
using RentalSystem.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(AppConstants.BACKEND_HTTP_PORT, o => o.Protocols = HttpProtocols.Http1);

    options.ListenLocalhost(AppConstants.BACKEND_GRPC_PORT, o => o.Protocols = HttpProtocols.Http2);
});

string projectId = AppConstants.FIRESTORE_PROJECT_ID;
string keyPath = Path.Combine(Directory.GetCurrentDirectory(),AppConstants.FIREBASE_KEY_FILENAME);
if (!File.Exists(keyPath))
{
    throw new Exception("Config do firestore nie istnieje");
}

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority = $"https://securetoken.google.com/{projectId}";

        options.RequireHttpsMetadata = false;
        options.Authority = authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,

            ValidateAudience = true,
            ValidAudience = projectId,

            ValidateLifetime = true
        };
    });

builder.Services.AddSingleton<FirestoreDb>(provider => FirestoreDb.Create(projectId));
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Wpisz 'Bearer <twoj_token>' w polu poni¿ej."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<AuthGrpcService>();

app.Run();