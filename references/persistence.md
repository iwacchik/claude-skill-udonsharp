# Persistence（PlayerData / PlayerObject）

プレイヤーのアカウントに紐付いたデータを**セッション・デバイスを超えて**保存する仕組み。VRChat サーバー上で管理される。

## 2 つの仕組み

| 仕組み | 概要 | 向き |
|-------|------|------|
| **PlayerData** | キーバリューストア。任意のスクリプトからアクセス可 | シンプルな値の保存 |
| **PlayerObject** | 各プレイヤーに自動スポーンされる GameObject | オブジェクトごとの状態保存、頻繁変更 |

## ストレージ制限

**1 ワールドあたり 1 プレイヤー 100KB**（PlayerData と PlayerObject それぞれ、圧縮後）。

超過すると**保存されない**。

## 環境別挙動

- **アップロード済みワールド** — VRChat サーバーに保存（本番）
- **ローカル Build & Test** — ローカル保存、テストクライアント終了でリセット
- **ClientSim** — プロジェクト内に JSON ファイルで保存される

## キー命名規約

プレハブ間の衝突を避けるため、**接頭辞を付ける**：

```
Momo-PPP-BloomAmount
```

---

## PlayerData

### using

```csharp
using VRC.SDK3.Persistence;
using VRC.Udon.Common;
```

### Set メソッド（ローカルプレイヤー限定）

各型：`SetInt`, `SetUInt`, `SetLong`, `SetULong`, `SetSByte`, `SetByte`, `SetShort`, `SetUShort`, `SetFloat`, `SetDouble`, `SetBool`, `SetString`, `SetBytes`, `SetVector2/3/4`, `SetColor`, `SetColor32`, `SetQuaternion`

```csharp
PlayerData.SetInt("jumps", 5);
PlayerData.SetString("name", "Alice");
```

### Get / TryGet メソッド（他プレイヤーも読める）

```csharp
int jumps = PlayerData.GetInt(Networking.LocalPlayer, "jumps");

if (PlayerData.TryGetInt(player, "jumps", out int value)) { /* ... */ }
```

各型に対応する TryGet あり。キー存在しないときの default 返しを明示判別したいなら TryGet を使う。**10 キー以上チェックするなら TryGet でまとめた方が効率的**。

### クエリ

```csharp
bool has = PlayerData.HasKey(player, "jumps");
System.Type t = PlayerData.GetType(player, "jumps");
bool ok = PlayerData.TryGetType(player, "jumps", out System.Type t);
```

**キー削除はできない**。不要になったら default 値で上書きする運用。

### イベント：`OnPlayerDataUpdated`

```csharp
public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
```

- フレーム終了時に、誰かの PlayerData が変更・受信されたとき発火
- `infos[i].Key`（string）、`infos[i].State`（`Unchanged` / `Added` / `Removed` / `Changed` / `Restored`）

ローカルプレイヤーのみに反応するなら `if (player.isLocal)` を先頭に。

### 重要タイミング：`OnPlayerRestored`

**`OnPlayerJoined` ではまだ PlayerData が届いていない**。データアクセスは `OnPlayerRestored` 以降：

```csharp
public override void OnPlayerRestored(VRCPlayerApi player)
{
    if (player.isLocal) { /* PlayerData 読み書き OK */ }
}
```

## PlayerObject

各プレイヤー分身の GameObject。**そのプレイヤーがオーナー**として扱われる。

### セットアップ

1. シーン内に GameObject を配置（テンプレート）
2. **`VRCPlayerObject`** コンポーネントを追加（テンプレート指定）
3. 任意で `UdonBehaviour`（スクリプト）
4. 任意で **`VRCEnablePersistence`**（同期変数を永続化）

→ プレイヤー入室時に VRChat がコピーを自動生成し、そのプレイヤーがオーナーに。

### 所有権の利点

- 各プレイヤーが自分の object を所有
- **他プレイヤーに "盗まれない"**（pickup やインベントリ向き）

### 永続化の発動条件

- PlayerObject に `UdonBehaviour` がある
- そこに `[UdonSynced]` 変数がある
- 同じ GameObject に `VRCEnablePersistence` がある

→ この 3 条件で synced 変数が自動保存される。

### アクセス API

```csharp
// 特定プレイヤーの PlayerObjects 全取得
GameObject[] objs = Networking.GetPlayerObjects(player);

// テンプレートから特定プレイヤーの実体を探す
SomeComponent c = FindComponentInPlayerObjects<SomeComponent>(template, player);
```

## 共通の設計指針

- **`OnPlayerRestored` を読み書きの起点**にする
- 退出直前（`OnPlayerLeft`）に保存しようとしても**間に合わない**（保存は退出前に完了していること）
- 頻繁に更新する状態（アバター位置など）は PlayerObject の方が得意
- 小さなフラグ・スコアは PlayerData でシンプルに
