using System;
using Discord;

namespace DiscordBot.Discord.Addons.Interactive.Paginator
{
    public class PaginatedAppearanceOptions
    {
        public IEmote First { get; set; } = new Emoji("⏮");

        public IEmote Back { get; set; } = new Emoji("◀");

        public IEmote Next { get; set; } = new Emoji("▶");

        public IEmote Last { get; set; } = new Emoji("⏭");

        public IEmote Stop { get; set; } = new Emoji("⏹");

        public IEmote Jump { get; set; } = new Emoji("🔢");

        public IEmote Info { get; set; } = new Emoji("ℹ");

        public string FooterFormat { get; set; } = "Page {0}/{1}";

        public string InformationTitle { get; set; } = "Paginator Information";

        public string InformationText { get; set; } =
            "This funny-looking embed is called a **Paginator**. It's just like a book. You can flip through the pages using the arrows, enter the number of the page you want to navigate to, or close the it entirely.";

        public Color InformationColor { get; set; } = new Color(252, 166, 205);

        public JumpDisplayOptions JumpDisplayOptions { get; set; } = JumpDisplayOptions.WithManageMessages;

        public bool DisplayInformationIcon { get; set; } = true;

        public TimeSpan? Timeout { get; set; } = null;

        public TimeSpan InfoTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public static PaginatedAppearanceOptions Default { get; set; } = new PaginatedAppearanceOptions();
    }

    public enum JumpDisplayOptions
    {
        Never,
        WithManageMessages,
        Always
    }
}