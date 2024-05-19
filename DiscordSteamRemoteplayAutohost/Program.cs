// See https://aka.ms/new-console-template for more information
using Discord.WebSocket;
using Discord;
using DiscordSteamRemoteplayAutohost;
using DiscordSteamRemoteplayAutohost.Steam;
using System;
using System.Runtime.InteropServices;
using System.Text;
using YamlDotNet.RepresentationModel;
using System.Text.Json;
using System.Reflection;

namespace DiscordSteamRemoteplayAutohost;

class Program
{
    public static async Task Main()
    {
        try
        {
            // 実行ディレクトリの設定
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            Directory.SetCurrentDirectory(exeDir);

            // トークンの読み込み
            var token = File.Exists("token.txt") ? await File.ReadAllTextAsync("token.txt") : null;
            if (token == null)
            {
                Console.WriteLine("トークンが設定されていません。token.txtにBotのトークンを記述してください");
                return;
            }

            // Yamlファイルの読み込み
            var yaml = new YamlStream();
            try
            {
                using var input = new StreamReader("config.yml", Encoding.UTF8);
                yaml.Load(input);
            }
            catch
            {
                Console.WriteLine("config.ymlが見つかりません");
                return;
            }
            var config = yaml.Documents.FirstOrDefault()?.RootNode;
            if (config == null)
            {
                Console.WriteLine("config.ymlが読み込めません");
                return;
            }

            // ギルドIDの取得
            var guildId = ulong.Parse(config["guildId"].ToString());


            // SteamStuff.dllの初期化
            if (!NativeMethods.SteamStuff_Init())
            {
                Console.WriteLine("Failed to initialize SteamStuff.dll");
                return;
            }

            // ゲームIDの取得
            var gameId = new CGameID(NativeMethods.SteamStuff_GetRunningGameID());
            Console.WriteLine($"ゲームの自動ホストを開始します: GameID={gameId.m_GameID}");

            // ゲームが実行されていない場合
            if (!gameId.IsValid)
            {
                Console.WriteLine("Steamゲームが実行されていません");
                return;
            }
            // Steam以外のゲームが実行されている場合
            if (gameId.Type != CGameID.EGameIDType.k_EGameIDTypeApp)
            {
                Console.WriteLine("Steam以外のゲームが実行されています");
                return;
            }

            // Webからゲームの情報を取得
            var url = $"https://store.steampowered.com/api/appdetails?appids={gameId.AppID}&l=japanese";
            var json = await new HttpClient().GetStringAsync(url);
            var data = JsonDocument.Parse(json).RootElement.GetProperty(gameId.AppID.ToString());

            var name = data.GetProperty("data").GetProperty("name").GetString();
            var headerImage = data.GetProperty("data").GetProperty("header_image").GetString();
            var storeLink = $"https://store.steampowered.com/app/{gameId.AppID}?l=japanese";

            Console.WriteLine($"ゲーム情報: {name} ({storeLink})");


            // コールバックの設定
            Action<string>? onInviteCreated = null;
            NativeMethods.SteamStuff_SetOnInviteCreated((invitee, connectURL) =>
            {
                Console.WriteLine($"招待リンクが作成されました: {connectURL}");
                onInviteCreated?.Invoke(connectURL);
                onInviteCreated = null;
            });
            NativeMethods.SteamStuff_SetOnSessionClosed(() =>
            {
                Console.WriteLine("セッションが終了しました");
            });


            // DiscordのBotの初期化
            var client = new DiscordSocketClient();
            client.Log += (msg) =>
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            };
            client.Ready += async () =>
            {
                Console.WriteLine("Bot is ready");

                // コマンドの登録
                var guild = client.GetGuild(guildId);
                await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                    .WithName("steam_invite")
                    .WithDescription("Steam Remote Play Together を使用して起動中のゲームに招待します")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("message")
                        .WithDescription("メッセージ")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(false))
                    .Build());
            };
            client.InteractionCreated += async (interaction) =>
            {
                if (interaction is SocketSlashCommand slashCommand)
                {
                    if (slashCommand.Data.Name == "steam_invite")
                    {
                        var message = slashCommand.Data.Options.FirstOrDefault(x => x.Name == "message")?.Value?.ToString();

                        // コマンド送信者のVCを取得
                        var inviterVcId = (slashCommand.User as SocketGuildUser)?.VoiceChannel?.Id ?? 0;

                        await interaction.RespondAsync(
                            text: message,
                            embed: new EmbedBuilder()
                                .WithTitle($"{name} を無料で一緒に遊びましょう！")
                                .WithUrl(storeLink)
                                .WithDescription("参加したい人はあらかじめ以下の参加手順に沿って部屋に入っておいてください。\n" +
                                    "(順番になったら、こっちで勝手にコントローラーを割り当てます)")
                                .AddField("特徴", "- ゲームを**持っていなくても**無料で参加できます\n" +
                                    "- PCだけでなく、**スマホでも**参加できます！\n" +
                                    "- Steamアカウントはなくてもプレイできます")
                                .AddField("参加手順 (スマホの場合)", "1. ↓の「招待リンク取得」を押してリンクを踏んでください\n" +
                                    "2. ページ内の iOS/Android リンクを押してSteam Linkアプリをインストールしてください\n" +
                                    "3. ページ内の「ゲームに参加」ボタンを押して、アプリを開きます")
                                .AddField("参加手順 (PCの場合)", "1. ↓の「招待リンク取得」を押してリンクを踏んでください\n" +
                                    "2. (Steamクライアントが入っていない人は) ページ内の Windows/macOS/Linux リンクを押してSteam Linkアプリをインストールしてください\n" +
                                    "3. コントローラー(Proコン、Joyコン、PlayStationコン、Xboxコンなど)をPCに接続してください\n" +
                                    "  ない人は [x360ceインストール手順](https://bit.ly/x360ce-tutorial) に沿ってコントローラーエミュレーターをインストールしてください\n" +
                                    "  x360ceがうまくいかない場合は、スマホでプレイできるので、そっちをお試しください\n" +
                                    "4. ページ内の「ゲームに参加」ボタンを押して、Steam Linkアプリを開きます")
                                .WithImageUrl(headerImage)
                                .WithColor(Color.DarkBlue)
                                .Build(),
                            components: new ComponentBuilder()
                                .WithButton("招待リンク取得", $"create_steam_invite_{inviterVcId}", ButtonStyle.Success, new Emoji("🔗"))
                                .Build()
                        );
                    }
                }

                if (interaction is SocketMessageComponent messageComponent)
                {
                    if (messageComponent.Data.CustomId.StartsWith("create_steam_invite_"))
                    {
                        // 考え中
                        await messageComponent.DeferAsync(ephemeral: true);


                        // IDを取得
                        var inviterVcId = ulong.Parse(messageComponent.Data.CustomId.Split('_').Last());

                        // 押した人がVCに入っているか確認
                        var inviteeVcId = (messageComponent.User as SocketGuildUser)?.VoiceChannel?.Id ?? 0;
                        if (inviterVcId != inviteeVcId)
                        {
                            var link = $"https://discord.com/channels/{guildId}/{inviterVcId}";
                            await messageComponent.FollowupAsync(
                                text: $"VC ({link}) に入ってからボタンを押してください",
                                ephemeral: true
                            );
                            return;
                        }


                        // 招待を作成
                        onInviteCreated = async (connectURL) =>
                        {
                            await messageComponent.FollowupAsync(
                                text: $"招待リンクを作成しました！\n{connectURL}\nリンクを踏んでゲームに参加してください～",
                                ephemeral: true
                            );
                        };
                        NativeMethods.SteamStuff_SendInvite(0, gameId.m_GameID);
                    }
                }
            };

            // ログイン
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            // ループ処理 (Steamのコールバックを回す)
            while (true)
            {
                NativeMethods.SteamStuff_RunCallbacks();
                // Console.WriteLine("Running...");
                await Task.Delay(1000);
            }
        }
        finally
        {
            NativeMethods.SteamStuff_Shutdown();
        }
    }
}
