var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


// Add logging
builder.Logging.ClearProviders(); // Clears default providers
builder.Logging.AddConsole(); // Adds console logging
builder.Logging.AddDebug();   // Adds debug logging (visible in the debugger)

builder.Logging.AddEventLog(settings =>
{ 
    settings.SourceName = "MVCTaskManager";  
    settings.LogName = "Application";     
});  

builder.Logging.SetMinimumLevel(LogLevel.Information);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowSpecificOrigin");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
