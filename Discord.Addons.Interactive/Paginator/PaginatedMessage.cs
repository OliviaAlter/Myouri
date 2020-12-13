using System;
using System.Collections.Generic;
using Discord;

namespace DiscordBot.Discord.Addons.Interactive.Paginator
{
    public class PaginatedMessage
    {
        public IEnumerable<EmbedPage> Pages { get; set; } = new List<EmbedPage>();

        public string Content { get; set; } = string.Empty;

        public EmbedAuthorBuilder Author { get; set; } = null;

        public Color Color { get; set; } = Color.Default;
        public string Title { get; set; } = null;

        public string Url { get; set; } = null;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = null;

        public string ThumbnailUrl { get; set; } = null;

        public List<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();

        public EmbedFooterBuilder FooterOverride { get; set; } = null;

        public DateTimeOffset? TimeStamp { get; set; } = null;
        public PaginatedAppearanceOptions Options { get; set; } = PaginatedAppearanceOptions.Default;
    }

    public class EmbedPage
    {
        public string AlternateAuthorTitle { get; set; }

        public string AlternateAuthorIcon { get; set; }

        public bool DisplayTotalFieldsCount { get; set; } = false;

        public string TotalFieldsMessage { get; set; } = null;

        public double TotalFieldsCountConstant { get; set; } = 1;

        public string Title { get; set; }

        public string Url { get; set; } = null;

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string ThumbnailUrl { get; set; } = null;

        public List<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();

        public EmbedFooterBuilder FooterOverride { get; set; } = null;

        public DateTimeOffset? TimeStamp { get; set; } = null;

        public Color? Color { get; set; } = null;
    }
}