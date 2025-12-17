using Google.Cloud.Firestore;
using RentalSystem.Shared.Constants;
using RentalSystem.Shared.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
string path = Path.Combine(Directory.GetCurrentDirectory(), Constants.FIREBASE_KEY_FILENAME);
Console.WriteLine($"U¿ywany plik klucza Firebase: {path}");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

builder.Services.AddSingleton<FirestoreDb>(provider =>
{
    string projectId = Constants.FIRESTORE_PROJECT_ID;
    return FirestoreDb.Create(projectId);
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/test-db", async (FirestoreDb db) =>
{
    try
    {
        var snapshot = await db.Collection("users").Limit(1).GetSnapshotAsync();
        return Results.Ok($"Po³¹czono z Firebase! Projekt ID: {db.ProjectId}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"B³¹d po³¹czenia: {ex.Message}");
    }
});

app.MapPost("add/user", async (FirestoreDb db) =>
{
    UserProfile newUserProfile = new UserProfile    
    );
});

app.Run();