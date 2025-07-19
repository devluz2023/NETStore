var builder = WebApplication.CreateBuilder(args);

// register controllers
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// map attribute-routed controllers
app.MapControllers();

app.Run();