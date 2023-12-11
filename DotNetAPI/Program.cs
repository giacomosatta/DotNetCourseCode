var builder = WebApplication.CreateBuilder(args); //Crea il server per le chiamate APIs

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); //Risolve gli endpoints
builder.Services.AddSwaggerGen(); //Ci consente di utilizzare Swagger

builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors",(corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://localhost:4200","http://localhost:3000","http://localhost:8000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });

     options.AddPolicy("ProdCors",(corsBuilder) =>
    {
        corsBuilder.WithOrigins("https://dominiofidato")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.MapControllers();
app.UseAuthentication();

// app.MapGet("/weatherforecast", () =>
// {

// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();


