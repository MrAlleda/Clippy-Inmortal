using System.Text.RegularExpressions;

namespace ClippyApp;

static class ClipboardReactions
{
    private static readonly Regex UrlRegex = new(@"^(https?://|www\.)\S+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(@"^[\w.+-]+@[\w-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
    private static readonly Regex NumberRegex = new(@"^-?\d+([.,]\d+)?%?$", RegexOptions.Compiled);

    public static (string Animation, string Message) Classify(string rawText, Random rng)
    {
        var text = rawText.Trim();

        if (UrlRegex.IsMatch(text))
            return ("GetTechy", rng.Pick(new[]
            {
                "¡Copiaste un enlace! ¿Se lo vas a mandar a alguien?",
                "Un link interesante, seguro.",
            }));

        if (EmailRegex.IsMatch(text))
            return ("Explain", "Parece una dirección de email.");

        if (NumberRegex.IsMatch(text))
            return ("GetWizardy", rng.Pick(new[]
            {
                $"Copiaste un número: {text}. ¿Haciendo cuentas?",
                $"{text}... ¿una fórmula, un precio, una fecha?",
            }));

        if (text.Length > 300)
            return ("Searching", "¡Copiaste bastante texto! Parece un párrafo entero.");

        if (text.Contains('\n'))
            return ("CheckingSomething", "Copiaste varias líneas. ¿Una lista?");

        if (text.Length <= 60)
            return ("CheckingSomething", $"Copiaste: \"{text}\"");

        return ("Thinking", "Copiaste algo al portapapeles.");
    }

    private static readonly (string Animation, string Message)[] ScreenshotReactions =
    {
        ("GetArtsy", "¡Buena captura!"),
        ("GetAttention", "¿Copiaste una imagen? ¡A ver, a ver!"),
        ("Congratulate", "¡Lindo screenshot!"),
    };

    public static (string Animation, string Message) RandomScreenshotReaction(Random rng) =>
        rng.Pick(ScreenshotReactions);
}
