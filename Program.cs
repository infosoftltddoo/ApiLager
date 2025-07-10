var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(opt => {
    opt.AddPolicy("AllowWP", p => p
    .AllowAnyOrigin()
      //.WithOrigins("https://infosoftpa.com")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials());
});
builder.Services.AddControllers();
var app = builder.Build();
//app.UseRouting();
app.UseCors("AllowWP");
app.MapControllers();
app.Run();
