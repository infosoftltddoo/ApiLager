var builder = WebApplication.CreateBuilder(args);

// Dodaj servise
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
      policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseCors();
// Middleware pipeline
app.UseRouting();



app.MapControllers();

app.Run();

