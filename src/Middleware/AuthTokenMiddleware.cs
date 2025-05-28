using PsGenApi.Services;

namespace PsGenApi.Middleware;

public class AuthTokenMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context, IRepositoryService repositoryService, ILogger<AuthTokenMiddleware> logger)
    {
        // Skip authentication for login and specific paths
        if (context.Request.Path.StartsWithSegments("/api/login") ||
            context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("AuthToken", out var authToken) || string.IsNullOrEmpty(authToken))
        {
            logger.LogWarning("AuthToken header is missing or empty");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { IsSuccess = false, Message = "AuthToken header is missing or empty" });
            return;
        }

        var token = await repositoryService.GetTokenBySecretAsync(authToken!);
        if (token == null || token.ExpiresAt < DateTimeOffset.UtcNow)
        {
            logger.LogWarning("Invalid or expired AuthToken");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { IsSuccess = false, Message = "Invalid or expired AuthToken" });
            return;
        }

        // Store user information for controllers to access
        context.Items["UserId"] = token.UserId;
        context.Items["TokenId"] = token.Id;

        await next(context);
    }
}
