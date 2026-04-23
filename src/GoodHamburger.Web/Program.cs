using GoodHamburger.Web;
using GoodHamburger.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<GoodHamburgerApi>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5173");
});
builder.Services.AddScoped<ToastService>();
await builder.Build().RunAsync();