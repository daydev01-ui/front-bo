using BCP.Connector.Branch;
using BCP.Connector.Business;
using BCP.Connector.Notification;
using BCP.Connector.Segurinet;
using BCP.Connector.Station;
using BCP.Connector.User;
using BCP.Connector.Report;
using BCP.Framework;
using BCP.QRBackOffice.Models.Options;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.AddLogger();


builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("HTTPClientWithTrustedOrUntrustedSSL")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
});

builder.Services.Configure<BaseUrl>(builder.Configuration.GetSection("BaseUrl"));
builder.Services.Configure<RoutePath>(builder.Configuration.GetSection("RoutePath"));
builder.Services.AddScoped<IReportConnector, ReportConnector>();

builder.Services.AddSingleton<IBusinessConnector, BusinessConnector>();
builder.Services.AddSingleton<IBranchConnector, BranchConnector>();
builder.Services.AddSingleton<IStationConnector, StationConnector>();
builder.Services.AddSingleton<IUserConnector, UserConnector>();

builder.Services
    .AddSingleton<INotificationConnector, NotificationConnector>(provider => new NotificationConnector(
        provider.GetRequiredService<IHttpClientFactory>(),
        provider.GetRequiredService<IConfiguration>(),
        builder.Configuration.GetSection("Connectors_PushNotification").Get<NotificationOptions>()!
    ));

builder.Services
    .AddSingleton<ISegurinetConnector, SegurinetConnector>(provider => new SegurinetConnector(
        provider.GetRequiredService<IHttpClientFactory>(),
        provider.GetRequiredService<IConfiguration>(),
        builder.Configuration.GetSection("Connectors_Sugurinet").Get<BaseIAMOptions>()!
    ));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(720);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();
