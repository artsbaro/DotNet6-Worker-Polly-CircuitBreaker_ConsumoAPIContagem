using ConsumoAPIContagem.Resilience;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

int numberOfExceptionsBeforeBreaking = 1;
int durationOfBreakInSeconds = 10;

builder.Services.AddSingleton(CircuitBreakerExtensions.CreatePolicy(numberOfExceptionsBeforeBreaking, durationOfBreakInSeconds));

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
