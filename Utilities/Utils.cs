using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Utilities
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        public static readonly GuildPermissions
            ModPermissions = new GuildPermissions(
                true, addReactions: true, viewChannel: true, sendMessages: true,
                sendTTSMessages: true, embedLinks: true, readMessageHistory: true, useExternalEmojis: true,
                connect: true, speak: true, useVoiceActivation: true, stream: true, changeNickname: true,
                kickMembers: true, manageMessages: true, manageChannels: true, attachFiles: true
                , deafenMembers: true, manageEmojis: true, manageNicknames: true, manageWebhooks: true,
                moveMembers: true, muteMembers: true);

        public static readonly GuildPermissions
            MemPermissions = new GuildPermissions(
                true, addReactions: true, viewChannel: true, sendMessages: true,
                sendTTSMessages: true, embedLinks: true, readMessageHistory: true, useExternalEmojis: true,
                connect: true, speak: true, useVoiceActivation: true, stream: true, changeNickname: true,
                attachFiles: true);

        public static readonly GuildPermissions
            NoInvite = new GuildPermissions(
                false, addReactions: true, viewChannel: true, sendMessages: true,
                sendTTSMessages: true, embedLinks: true, readMessageHistory: true, useExternalEmojis: true,
                connect: true, speak: true, useVoiceActivation: true, stream: true, changeNickname: true,
                attachFiles: true);

        public static readonly GuildPermissions MutedPermissions =
            new GuildPermissions(viewChannel: true, addReactions: true, readMessageHistory: true);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);

        public static float InstalledMemory()
        {
            GetPhysicallyInstalledSystemMemory(out var memKb);
            var memoryInMb = (float) memKb / 1024;
            return memoryInMb;
        }

        public static float RamUsage()
        {
            var freeMemory = GetRamCounter();
            var totalMemory = InstalledMemory();
            var usedMemory = totalMemory - freeMemory;
            var memUsage = usedMemory / totalMemory * 100;
            return memUsage;
        }

        public static float GetCpuCounter()
        {
            var cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };
            var initialValue = cpuCounter.NextValue();
            Thread.Sleep(1000);
            var cpuValue = cpuCounter.NextValue();
            return cpuValue;
        }

        public static float GetRamCounter()
        {
            var memCounter = new PerformanceCounter
            {
                CategoryName = "Memory",
                CounterName = "Available MBytes"
            };
            var initialValue = memCounter.NextValue();
            Thread.Sleep(1000);
            var ramValue = memCounter.NextValue();
            return ramValue;
        }

        public static async Task SendInvalidPerm(IUser user, IMessageChannel channel)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("Command issuer", $"{user.Mention}")
                .WithDescription(
                    $"{user.Mention} does not have the permission to do that.")
                .WithFooter($"{user.Username}", user.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            await channel.SendMessageAsync(embed: builder.Build());
        }


        public static int GetRolePosition(SocketGuildUser u1)
        {
            return u1.Roles.Select(r => r.Position).Prepend(0).Max();
        }

        public static bool CanInteractRole(SocketGuildUser user, SocketRole role)
        {
            return GetRolePosition(user) > role.Position;
        }

        public static bool CanInteractUser(SocketGuildUser u1, SocketGuildUser u2)
        {
            return GetRolePosition(u1) > GetRolePosition(u2);
        }

        public static string GetRandomAlphaNumeric(int length)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static int RandomColor()
        {
            return Random.Next(1, 255);
        }
    }
}