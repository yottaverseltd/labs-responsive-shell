namespace LabsResponsiveShell.Data;

public static class InitialsFormatter
{
    public static string FromTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "?";
        }

        var parts = title.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length switch
        {
            0 => char.ToUpperInvariant(title[0]).ToString(),
            1 => char.ToUpperInvariant(parts[0][0]).ToString(),
            _ => $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}",
        };
    }
}
