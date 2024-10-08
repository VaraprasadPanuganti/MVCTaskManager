using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Repositories;
using System.Text;
using MVCTaskManager.Services.Interfaces;
using MVCTaskManager.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Register your services and repositories
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

//Cors implementation
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

//Jwt Implementation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
 });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowSpecificOrigin");
app.UseStaticFiles();
app.UseRouting();
// Add authentication and authorization to the pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
