using MeAgendaAi.Application.Mapper;
using MeAgendaAi.Infra.CrossCutting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
        .AddControllers()
        .AddValidation<Program>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAndConfigureSwaggerGen();
builder.Services.ConfigureDatabaseDependencies();
builder.Services.ConfigureDatabase(builder.Configuration.GetConnectionString("AppDb"));
builder.Services.ConfigureServicesDependecies();
builder.Services.ConfigureNotification();
builder.Services.ConfigureMiddlewareNotification();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddAutoMapper(typeof(DefaultProfile));
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});
builder.Services.ConfigureCacheDistributedService();
builder.Services.ConfigureCacheDistributed(builder.Configuration);
builder.Services.ConfigureServiceAndHttpClientMailJet(builder.Configuration);

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }