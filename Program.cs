using TravelRecommendationApi.Services;
using TravelRecommendationApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache(); // Add memory cache
builder.Services.AddSingleton<DistrictService>();
builder.Services.AddSingleton<HttpHelper>();
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddSingleton<TravelService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
