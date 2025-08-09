using TodoMinimal.IdentityServer.HostedServices;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddInfrastructure(
    options => configuration.GetSection("IdentityInfrastructure").Bind(options));

builder.Services.AddOpenIdDictService(configuration);

builder.Services.AddAuthenticationExtension();
builder.Services.AddAuthorizationExtension();

// builder.Services.AddHostedService<Worker>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error");
app.UseForwardedHeaders();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();

app.UseEndpoints(cfg =>
{
    cfg.MapControllers();
    cfg.MapDefaultControllerRoute();
});

app.Run();
