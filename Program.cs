var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(opt => {
    opt.AddPolicy("AllowWP", p => p
      .WithOrigins("https://infosoftpa.com")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials());
});
builder.Services.AddControllers();
var app = builder.Build();
//var port = Environment.GetEnvironmentVariable("PORT") ?? "7285";
//app.Urls.Add($"http://*:{port}");
app.UseRouting();
app.UseCors("AllowWP");
app.MapControllers();
app.Run();
