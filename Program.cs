using MixBrev.Domain;
using MixBrev.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar los servicios de infraestructura
builder.Services.AddScoped<IMixpanelService, MixpanelService>();
builder.Services.AddScoped<ISupabaseService, SupabaseService>();
builder.Services.AddScoped<IDataMicro, DataMicro>();
builder.Services.AddScoped<IBrevoService, BrevoService>();

// Registrar el orquestador principal
builder.Services.AddScoped<ISyncService, SyncService>();

// Si usas HttpClient en tus servicios, no olvides esto:
builder.Services.AddHttpClient();

builder.Services.AddControllers();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
