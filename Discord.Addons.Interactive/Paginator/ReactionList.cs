namespace DiscordBot.Discord.Addons.Interactive.Paginator
{
    public class ReactionList
    {
        public bool First { get; set; } = true;
        public bool Last { get; set; } = true;
        public bool Forward { get; set; } = true;
        public bool Backward { get; set; } = true;
        public bool Jump { get; set; } = true;
        public bool Trash { get; set; } = true;
        public bool Info { get; set; } = true;
    }
}