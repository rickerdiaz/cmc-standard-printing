using CmcStandardPrinting.Application.Printing;
using CmcStandardPrinting.Infrastructure.Printing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPrintJobRepository, InMemoryPrintJobRepository>();
builder.Services.AddSingleton<PrintJobService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("GetHealth")
    .WithOpenApi();

app.MapGet("/print-jobs", async (PrintJobService service, CancellationToken cancellationToken) =>
{
    var jobs = await service.GetAsync(cancellationToken);
    return Results.Ok(jobs);
})
.WithName("GetPrintJobs")
.WithOpenApi();

app.MapGet("/print-jobs/{id:guid}", async (Guid id, PrintJobService service, CancellationToken cancellationToken) =>
{
    var job = await service.GetByIdAsync(id, cancellationToken);
    return job is null ? Results.NotFound() : Results.Ok(job);
})
.WithName("GetPrintJobById")
.WithOpenApi();

app.MapPost("/print-jobs", async (CreatePrintJobRequest request, PrintJobService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { error = "Name is required" });
    }

    var job = await service.CreateAsync(request.Name, cancellationToken);
    return Results.Created($"/print-jobs/{job.Id}", job);
})
.WithName("CreatePrintJob")
.WithOpenApi();

app.MapPut("/print-jobs/{id:guid}/status", async (Guid id, UpdatePrintJobStatusRequest request, PrintJobService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Status))
    {
        return Results.BadRequest(new { error = "Status is required" });
    }

    try
    {
        var job = await service.UpdateStatusAsync(id, request.Status, cancellationToken);
        return job is null ? Results.NotFound() : Results.Ok(job);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdatePrintJobStatus")
.WithOpenApi();

app.Run();

internal record CreatePrintJobRequest(string Name);
internal record UpdatePrintJobStatusRequest(string Status);
