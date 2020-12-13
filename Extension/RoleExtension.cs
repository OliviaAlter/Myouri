using System.Threading.Tasks;
using Discord;
using DiscordBot.Utilities;

namespace DiscordBot.Extension
{
    public class RoleExtension
    {
        public static async Task AddRoleEmbed(IUser user, IMessageChannel channel, IRole role)
        {
            var builder = new EmbedBuilder()
                .WithDescription(
                    $"{user.Mention} has been given the role {role.Mention}!")
                .WithFooter($"{user.Username}", user.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await channel.SendMessageAsync(embed: builder.Build());
        }

        public static async Task RevokeRoleEmbed(IUser user, IMessageChannel channel, IRole role)
        {
            var builder = new EmbedBuilder()
                .WithDescription(
                    $"{role.Mention} has been removed from {user.Mention}!")
                .WithFooter($"{user.Username}", user.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await channel.SendMessageAsync(embed: builder.Build());
        }

        public static async Task RevokeRankRoleEmbed(IMessageChannel channel, IRole role)
        {
            var builder = new EmbedBuilder()
                .WithDescription(
                    $"{role.Mention} has been removed from roles list!")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await channel.SendMessageAsync(embed: builder.Build());
        }

        public static async Task AddRankRoleEmbed(IMessageChannel channel, IRole role)
        {
            var builder = new EmbedBuilder()
                .WithDescription(
                    $"{role.Mention} has been added to roles list!")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await channel.SendMessageAsync(embed: builder.Build());
        }

        public static async Task AddAutoRoleEmbed(IMessageChannel channel, IRole role)
        {
            var builder = new EmbedBuilder()
                .WithDescription(
                    $"{role.Mention} has been added to auto roles list!")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await channel.SendMessageAsync(embed: builder.Build());
        }

        public static async Task RemoveAutoRoleEmbed(IMessageChannel channel, IRole role)
        {
            var builder = new EmbedBuilder()
                .WithDescription(
                    $"{role.Mention} has been removed from auto roles list!")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await channel.SendMessageAsync(embed: builder.Build());
        }
    }
}