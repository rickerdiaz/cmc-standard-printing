using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Middleware;

public class JsonpMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JsonpMiddleware> _logger;
    private readonly string _callbackParameter;

    public JsonpMiddleware(RequestDelegate next, ILogger<JsonpMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _callbackParameter = configuration["Jsonp:CallbackParameter"] ?? "callback";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var callback = context.Request.Query[_callbackParameter].ToString();
        if (string.IsNullOrWhiteSpace(callback))
        {
            await _next(context);
            return;
        }

        var originalBody = context.Response.Body;
        await using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);

            if (!IsJsonContent(context.Response.ContentType))
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBody);
                return;
            }

            memoryStream.Position = 0;
            var bodyText = await new StreamReader(memoryStream).ReadToEndAsync();
            var wrapped = $"{callback}({bodyText})";

            context.Response.ContentType = "application/javascript";
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(wrapped);

            await using var writer = new StreamWriter(originalBody, Encoding.UTF8, leaveOpen: true);
            await writer.WriteAsync(wrapped);
            await writer.FlushAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JSONP wrapping failed for callback parameter {CallbackParameter}", _callbackParameter);
            context.Response.Body = originalBody;
            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool IsJsonContent(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) ||
               contentType.StartsWith("text/json", StringComparison.OrdinalIgnoreCase);
    }
}
