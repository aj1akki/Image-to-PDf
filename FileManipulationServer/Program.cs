using FileManipulationServer;
var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("Settings").Get<Settings>();

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(
      name: "OpenCORSPolicy",
      builder => {
          builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
      });
});

// Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(settings.SyncFusionLicenseKey);

builder.Services.AddControllers();
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
app.UseCors("OpenCORSPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
