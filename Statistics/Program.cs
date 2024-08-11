using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Statistics;
using Statistics.Services;

// The builder is used to configure the application and attach the components to the DOM
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

// Attach the App component to the #app element in the index.html file
builder.RootComponents.Add<App>("#app");

// Attach the HeadOutlet component to the head::after element in the index.html file
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add the other services to the builder
builder.Services.AddSingleton<GlobalAppState>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Build the application and run it
await builder.Build().RunAsync();