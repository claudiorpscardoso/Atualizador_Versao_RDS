namespace AtualizadorVersaoRds;

/// <summary>
/// Resultado consolidado de uma execucao. Sem isso a UI nao consegue distinguir
/// "atualizou tudo" de "falhou em tudo".
/// </summary>
public sealed class UpdateSummary
{
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public bool Cancelled { get; set; }

    public int Total => Succeeded + Failed;

    public bool HasErrors => Failed > 0;
}
