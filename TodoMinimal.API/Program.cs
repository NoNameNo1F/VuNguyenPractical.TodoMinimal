using FastEndpoints;
using TodoMinimal.API.Extensions;
using TodoMinimal.API.Extensions.SecurityExtensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// builder.WebHost.UseLogger(configuration =>
// {
//     var appSettings = new AppSettings();
//     configuration.Bind(appSettings);
//     return appSettings.Logging;
// });

builder.Services.AddNoteInfrastructure(opt => 
    configuration.GetSection("NoteModule").Bind(opt));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddSwaggerDocumentation()
    .AddVersioning();

builder.Services.AddSecurityExtensions(configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHsts();
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.Run();