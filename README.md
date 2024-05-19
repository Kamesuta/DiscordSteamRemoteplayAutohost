# Steam Remote Play Together 招待リンク自動発行Bot

Steam Remote Play Togetherの招待リンクを自動で発行するDiscord Botです。

ゲームをホストする際、誰かが参加するたびに招待リンクをいちいち手動で発行する手間を省くことができます。

`/steam_invite` コマンドを実行すると、そのチャンネルに参加手順と招待リンク発行ボタンが表示され、ユーザーはボタンをクリックするだけで招待リンクを発行してゲームに参加できます。

## 機能

- `/steam_invite` コマンドで招待リンクを発行できる
  - 参加手順が表示されるため、初めてのユーザーでも簡単に参加できます。
- 招待リンク発行ボタンをクリックするだけで招待リンクを発行できる
  - ボタンをクリックするだけで招待リンクを発行できるため、ホストがゲームをわざわざ中断して招待リンクを作成する手間が省けます。

## 使い方

1. Botを作成し、サーバーに追加します。
2. token.txtにBotのトークンを記入します。
3. config.ymlに設定を記入します。
```yaml
# config.yml
guildId: 123456789012345678 # サーバーID
```
4. exeを実行します。
5. VCに入った状態で、`/steam_invite` コマンドを実行します。