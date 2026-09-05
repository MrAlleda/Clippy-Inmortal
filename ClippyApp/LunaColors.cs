namespace ClippyApp;

static class LunaColors
{
    public static readonly Color WindowBorder = ColorTranslator.FromHtml("#0831D9");
    public static readonly Color Body = ColorTranslator.FromHtml("#ECE9D8");

    public static readonly Color TitleTop = ColorTranslator.FromHtml("#4A9CF5");
    public static readonly Color TitleStop20 = ColorTranslator.FromHtml("#1257D6");
    public static readonly Color TitleStop70 = ColorTranslator.FromHtml("#0F4FD0");
    public static readonly Color TitleBottom = ColorTranslator.FromHtml("#0A3FB0");

    public static readonly Color CloseTop = ColorTranslator.FromHtml("#F5A08A");
    public static readonly Color CloseStop40 = ColorTranslator.FromHtml("#D63C14");
    public static readonly Color CloseBottom = ColorTranslator.FromHtml("#B52A08");

    public static readonly Color ButtonTop = Color.White;
    public static readonly Color ButtonStop85 = ColorTranslator.FromHtml("#ECEBE5");
    public static readonly Color ButtonBottom = ColorTranslator.FromHtml("#D8D0C4");
    public static readonly Color ButtonBorder = ColorTranslator.FromHtml("#003C74");
    public static readonly Color ButtonHoverRing = ColorTranslator.FromHtml("#F8B338");
    public static readonly Color ButtonDisabledTop = Color.FromArgb(248, 248, 246);
    public static readonly Color ButtonDisabledBottom = Color.FromArgb(214, 210, 202);
    public static readonly Color ButtonDisabledText = Color.FromArgb(150, 150, 150);

    public static readonly Color InputBorder = ColorTranslator.FromHtml("#7F9DB9");
    public static readonly Color GroupBorder = ColorTranslator.FromHtml("#D0C9A8");
    public static readonly Color GroupTitle = ColorTranslator.FromHtml("#0046D5");

    public static readonly Color BalloonFill = ColorTranslator.FromHtml("#FFFBE1");
    public static readonly Color BalloonBorder = ColorTranslator.FromHtml("#D0C9A8");

    public static readonly Color GridHeaderTop = ColorTranslator.FromHtml("#FEFEFE");
    public static readonly Color GridHeaderBottom = ColorTranslator.FromHtml("#E5E2D5");
    public static readonly Color GridAltRow = ColorTranslator.FromHtml("#F5F4EC");
    public static readonly Color GridSelected = ColorTranslator.FromHtml("#316AC5");

    public static readonly Color WizardPanelTop = ColorTranslator.FromHtml("#7BA2E0");
    public static readonly Color WizardPanelBottom = ColorTranslator.FromHtml("#3A63B0");

    public static readonly Color TabBorder = ColorTranslator.FromHtml("#919B9C");
    public static readonly Color TabActiveBg = ColorTranslator.FromHtml("#FDFDF8");
    public static readonly Color TabInactiveBg = ColorTranslator.FromHtml("#E5E2D5");

    public static readonly Color SuccessText = Color.FromArgb(10, 122, 10);
    public static readonly Color ErrorText = Color.FromArgb(180, 20, 20);
    public static readonly Color SecondaryText = Color.DimGray;

    public static readonly Font Ui = new("Tahoma", 8f);
    public static readonly Font UiBold = new("Tahoma", 8f, FontStyle.Bold);
}
