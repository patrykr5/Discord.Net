using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using _02_commands_framework.Services;
using System;

namespace _02_commands_framework.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }

        // Ban a user
        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can ban
        [RequireUserPermission(GuildPermission.BanMembers)]
        // make sure the bot itself can ban
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }

        // Kick a user
        [Command("kick")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can kick
        [RequireUserPermission(GuildPermission.KickMembers)]
        // make sure the bot itself can kick
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            // Creates the pattern TextBlock
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle(user.Username + " kicked from server!")
                .WithColor(Color.Orange)
                .WithDescription("by " +
                                $"{Context.User.Username}" +
                                Environment.NewLine +
                                "Reason: " + reason);

            await user.KickAsync(reason, null);
            await ReplyAsync("", false, builder.Build());
        }

        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));
    }
}
