namespace AtualizadorVersaoRds;

public sealed class UpdateProgressInfo
{
    public double ProgressPercent { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsError { get; init; }
    public bool AppendToLog { get; init; } = true;
}
