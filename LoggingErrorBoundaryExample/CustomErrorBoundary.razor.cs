namespace LoggingErrorBoundaryExample;


public partial class CustomErrorBoundary : ErrorBoundary
{
    [Parameter] public EventCallback<Exception> OnLogging { get; set; } = default!;
    private string LogID = string.Empty;
    private bool ChildContentIsViewable = true;

    protected override async Task OnErrorAsync(Exception exception)
    {
        ChildContentIsViewable = false;
        exception = exception.StackTrace is null ? exception.GetBaseException() : exception;
        await base.OnErrorAsync(exception);
        await LogError(exception);
    }

    public new void Recover()
    {
        base.Recover();
        ChildContentIsViewable = true;
        InvokeAsync(StateHasChanged);
    }

    private async Task LogError(Exception e)
    {
        if (OnLogging.HasDelegate)
        {
            LogID = RandomIdGenerator.GetRand(6);
            await OnLogging.InvokeAsync(e);
        }
    }
}

public static class RandomIdGenerator
{
    private static readonly char[] _base62chars =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
        .ToCharArray();

    private static readonly Random _random = new();

    public static string GetRand(int length)
    {
        StringBuilder sb = new(length);

        for (int i = 0; i < length; i++)
            sb.Append(_base62chars[_random.Next(62)]);

        return sb.ToString();
    }
}
