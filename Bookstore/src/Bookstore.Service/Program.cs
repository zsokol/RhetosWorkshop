using Autofac;
using Rhetos;
using Microsoft.AspNetCore.Authentication.Negotiate;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.CustomSchemaIds(type => type.ToString()));

builder.Services.AddRhetosHost((serviceProvider, rhetosHostBuilder) => rhetosHostBuilder
        .ConfigureRhetosAppDefaults()
        .UseBuilderLogProviderFromHost(serviceProvider)
        .ConfigureContainer(builder =>
        {
            // Registering custom components for Bookstore application:
            builder.RegisterType<Bookstore.SmtpMailSender>().As<Bookstore.IMailSender>(); // Application uses SMTP implementation for sending mails. The registration will be overridden in unit tests by fake component.
            builder.Register(context => context.Resolve<Rhetos.Utilities.IConfiguration>().GetOptions<Bookstore.MailOptions>()).SingleInstance(); // Standard pattern for registering an options class.
        })
        .ConfigureConfiguration(cfg => cfg.MapNetCoreConfiguration(builder.Configuration)))
    .AddAspNetCoreIdentityUser()
    .AddHostLogging()
    .AddDashboard()
    .AddRestApi(o => 
        {
            o.BaseRoute = "rest";
            o.GroupNameMapper = (conceptInfo, controller, oldname) => "v1";
        });


var app = builder.Build();

app.UseRhetosRestApi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
    app.MapRhetosDashboard();

app.Run();
