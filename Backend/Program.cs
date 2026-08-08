using GameVault.Data;
using GameVault.Services;

var builder = WebApplication.CreateBuilder(args);

string frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddDbContext<GameVaultContext>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policyBuilder =>
    {
        policyBuilder.WithOrigins(frontendUrl)
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

builder.Services.AddScoped<RawgService>();
builder.Services.AddScoped<SteamService>();

var app = builder.Build();

app.UseCors("AllowReact");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameVaultContext>();
    db.Database.EnsureCreated();
}

app.MapControllers();

app.Run();