using MeAgendaAi.Application.Mapper;
using MeAgendaAi.Infra.CrossCutting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureDatabaseDependencies();
builder.Services.ConfigureDatabase(builder.Configuration.GetConnectionString("AppDb"));
builder.Services.ConfigureServicesDependecies();
builder.Services.ConfigureNotification();
builder.Services.ConfigurationMiddlewareNotification();
builder.Services.AddAutoMapper(typeof(DefaultProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }