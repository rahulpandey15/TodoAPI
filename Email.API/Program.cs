var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/email/send", (string recipient, ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(recipient))
    {
        return Results.BadRequest(new { status = "error", message = "recipient query parameter is required" });
    }
    logger.LogInformation("Sending email to {recipient}", recipient);
    return Results.Ok(new { status = "success", message = $"Mail has been sent to {recipient}" });
})
.WithName("Send");

app.Run();