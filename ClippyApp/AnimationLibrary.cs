namespace ClippyApp;

static class AnimationLibrary
{
    public static readonly string[] All =
    {
        "Alert", "CheckingSomething", "Congratulate", "EmptyTrash", "Explain",
        "GestureDown", "GestureLeft", "GestureRight", "GestureUp", "GetArtsy",
        "GetAttention", "GetTechy", "GetWizardy", "GoodBye", "Greeting",
        "Hearing_1", "Hide", "Idle1_1", "IdleAtom", "IdleEyeBrowRaise",
        "IdleFingerTap", "IdleHeadScratch", "IdleRopePile", "IdleSideToSide",
        "IdleSnooze", "LookDown", "LookDownLeft", "LookDownRight", "LookLeft",
        "LookRight", "LookUp", "LookUpLeft", "LookUpRight", "Print", "Processing",
        "RestPose", "Save", "Searching", "SendMail", "Thinking", "Wave", "Writing",
    };

    public static readonly string[] Idle =
    {
        "Idle1_1", "IdleAtom", "IdleEyeBrowRaise", "IdleFingerTap",
        "IdleHeadScratch", "IdleRopePile", "IdleSideToSide", "IdleSnooze",
        "LookDown", "LookLeft", "LookRight", "LookUp",
    };

    public static readonly Dictionary<string, string[]> SpeakLines = new()
    {
        ["Greeting"] = new[] { "¡Hola! Soy Clippy, tu asistente de Office.", "¿Parece que necesitás ayuda con algo?" },
        ["Wave"] = new[] { "¡Hola de nuevo!" },
        ["GoodBye"] = new[] { "¡Nos vemos! Fue un placer ayudarte." },
        ["Explain"] = new[] { "Dejame explicarte cómo funciona esto..." },
        ["Thinking"] = new[] { "Hmm... dejame pensarlo un segundo." },
        ["Processing"] = new[] { "Procesando tu solicitud, un momento..." },
        ["Searching"] = new[] { "Buscando la mejor respuesta para vos..." },
        ["CheckingSomething"] = new[] { "Estoy revisando eso ahora mismo." },
        ["Congratulate"] = new[] { "¡Felicitaciones! Lo hiciste muy bien." },
        ["GetAttention"] = new[] { "¡Che! ¿Tenés un segundo?" },
        ["GetArtsy"] = new[] { "¡Hora de ponerse creativos! 🎨" },
        ["GetTechy"] = new[] { "Vamos a ponernos técnicos por un momento." },
        ["GetWizardy"] = new[] { "✨ Un poco de magia nunca está de más. ✨" },
        ["Alert"] = new[] { "⚠️ ¡Atención! Algo necesita que lo revises." },
        ["EmptyTrash"] = new[] { "¿Seguro que querés vaciar la papelera? Eso no se puede deshacer." },
        ["Print"] = new[] { "Enviando tu documento a la impresora..." },
        ["Save"] = new[] { "Guardando tu trabajo. ¡No lo pierdas!" },
        ["SendMail"] = new[] { "Enviando tu correo..." },
        ["Hearing_1"] = new[] { "Te escucho, contame." },
        ["Writing"] = new[] { "Parece que estás escribiendo algo." },
    };

    public static readonly HashSet<string> Silent = new(
        Idle.Concat(new[] { "GestureDown", "GestureLeft", "GestureRight", "GestureUp", "RestPose", "Hide" })
    );

    public static readonly (string Animation, string Line)[] AnnoyingTips =
    {
        ("GetAttention", "¿Sabías que podés hacer click derecho para ver más opciones?"),
        ("Explain", "¡Parece que estás usando la computadora! ¿Necesitás ayuda?"),
        ("CheckingSomething", "¿Ya tomaste agua hoy?"),
        ("Thinking", "Recordá guardar tu trabajo seguido."),
        ("Congratulate", "¡Vas muy bien! Seguí así."),
        ("Wave", "¡Hola de nuevo! Solo pasaba a saludar."),
        ("GetTechy", "¿Sabías que me podés cambiar de tamaño desde el menú?"),
    };

    public static readonly string[] SurpriseLines =
    {
        "¡Sorpresa! 🎉",
        "¿Me extrañabas?",
        "¡Bip bop! Soy un clip parlante.",
        "Dato random: los clips se inventaron en 1899.",
        "¡Aquí ando, dando vueltas!",
        "¡Ta-tán! Nada en particular, solo quería saludar.",
        "Si me ves seguido es porque te caigo bien.",
        "¿Sabías que también reacciono si copiás algo?",
    };

    public static string RandomLine(string animation, Random rng)
    {
        if (!SpeakLines.TryGetValue(animation, out var lines) || lines.Length == 0) return "";
        return rng.Pick(lines);
    }
}
