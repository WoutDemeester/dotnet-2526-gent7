using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rise.Client;
using Rise.Client.Identity;
using MudBlazor.Services;
using Rise.Client.Products;
using Rise.Shared.Departments;
using Rise.Shared.Resto;
using Rise.Client.Services;
using Rise.Client.Support;
using Rise.Shared.Support;
using Rise.Shared.Deadlines;
using Rise.Client.Deadlines;    

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);

    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.BrowserConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
        .CreateLogger();

    Log.Information("Starting web application");

    // Register the cookie handler
    builder.Services.AddTransient<CookieHandler>();

    // Set up authorization
    builder.Services.AddAuthorizationCore();

    // Register the custom state provider
    builder.Services.AddScoped(sp =>
    {
        var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:5001";
        var httpClient = new HttpClient { BaseAddress = new Uri(backendUrl) };
        return (IRestoService)new RestoService(httpClient);
    });

    // add custom services
    builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
    // Register the account management interface
    builder.Services.AddScoped(sp => (IAccountManager)sp.GetRequiredService<AuthenticationStateProvider>());

    // Configure named HttpClient for auth interactions
    builder.Services.AddHttpClient("SecureApi", opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001"))
        .AddHttpMessageHandler<CookieHandler>();
    
    builder.Services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
     {
         client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
     });

    // Add DeadlineService registration
    builder.Services.AddHttpClient<IDeadlineService, DeadlineService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    builder.Services.AddScoped<ISupportService, FakeSupportService>();

    // Adding MudBlazor (component library)
    builder.Services.AddMudServices();


    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An exception occurred while creating the WASM host");
    throw;
}