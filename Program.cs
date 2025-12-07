var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// ✅ Add CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        // Allow requests only from this origin (Angular dev server)
        policy.WithOrigins(
                "http://geminiapikey.runasp.net",  // ⬅️ Angular production URL
                "http://localhost:4200")
            // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
            .AllowAnyMethod()
            // Allow any HTTP headers
            .AllowAnyHeader()
            // Allow credentials such as cookies or authentication headers
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseCors("AllowAngular");
app.MapControllers();
app.Run();
