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

app.MapPost("/email/send", (Message mail,ILogger<Program>  logger) =>
{
    logger.LogInformation("Sending email to {recipient}", mail.receipient);
    return Results.Ok(new { status = "success", message = "Mail has been sent" });
})
.WithName("Send");

app.Run();

internal record Message(string receipient, string mailBody);
