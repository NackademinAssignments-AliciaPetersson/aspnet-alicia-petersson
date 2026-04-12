using Domain.Abstractions.Logging;

namespace Tests.Fakes;

internal sealed class TestLogger : ILogger
{
    public void Log(Exception ex)
    { 
        // no-op
    } 
    public void Log(string message) { 
        // no-op
    }
}
