var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// ✅ Add CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        // Allow requests only from this origin (Angular dev server)
        // .WithOrigins(
        //         "http://localhost:4200",
        //         "http://geminiapikey.runasp.net" // ⬅️ production URL
        //     )
            // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
            // .AllowAnyMethod()
            // Allow any HTTP headers
            // .AllowAnyHeader()
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseRouting();
app.UseCors("AllowAngular");
app.MapControllers();
app.Run();
