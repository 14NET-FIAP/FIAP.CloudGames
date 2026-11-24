using System.Diagnostics;

namespace FIAP.CloudGames.API.Extensions;

public static class HttpContextExtensions
{
    // Prioriza o TraceId distribuído (W3C) para correlacionar com logs de outros serviços.
    public static string GetTraceId(this HttpContext context) =>
        Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
}
