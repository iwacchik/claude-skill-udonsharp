# Network Events（`SendCustomNetworkEvent` / `[NetworkCallable]`）

## 用途

**一度きりのアクション**を他プレイヤーで実行する。音・エフェクト・即時的な演出向け。**永続的な状態変更には使わない**（late joiner に届かないため）。

## 基本構文

```csharp
SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PlaySound));

public void PlaySound() { audioSource.Play(); }
```

## `NetworkEventTarget`

| 値 | 配信先 |
|----|-------|
| `All` | 全員（送信者含む） |
| `Others` | 送信者以外の全員 |
| `Owner` | そのオブジェクトのオーナーのみ |
| `Self` | ローカル（自分）のみ。**レート制限をバイパス** |

## メソッド定義の要件

| 項目 | 要件 |
|------|------|
| アクセス修飾子 | **`public` 必須** |
| 属性 | `[NetworkCallable]`（引数付きの場合は必須） |
| 不可 | `static`, `virtual`, `override`, メソッドオーバーロード |
| 戻り値 | `void`（値返却不可） |
| メソッド名先頭 | `_` 付きはネットワーク対象外（ローカル呼出は可） |

受信側 UdonBehaviour の SyncMode が `None` ではダメ（`None` はネットワーク機能全無効）。

## 引数付きイベント（`[NetworkCallable]`、SDK 3.8.1+）

```csharp
[NetworkCallable]
public void AddScore(int delta, string playerName)
{
    score += delta;
}

// 送信
SendCustomNetworkEvent(NetworkEventTarget.All, nameof(AddScore), 10, "Alice");
```

### 制限

| 項目 | 値 |
|------|-----|
| 最大引数数 | **8 個** |
| 総サイズ | **16 KB** |
| 分割サイズ | **1024 バイト超は内部分割** |
| 引数型 | 同期可能な型のみ |

`null` を渡すと受信側で `default(T)` になる。

## レート制限

| 項目 | 値 |
|------|-----|
| デフォルト | **5 events/sec** |
| 範囲 | 1 〜 **100 events/sec** |
| 指定方法 | `[NetworkCallable(maxEventsPerSecond: N)]` |
| 超過時 | キューイングされて順次送信 |
| `Self` 宛 | **レート制限バイパス** |

```csharp
[NetworkCallable(maxEventsPerSecond: 30)]
public void FastEvent() { ... }
```

## 輻輳監視

```csharp
int queuedForThis = NetworkCalling.GetQueuedEvents(this);
int queuedAll = NetworkCalling.GetAllQueuedEvents();
```

キュー長が増え続けていたら、送信頻度を下げるか値の集約を検討。

## 後方互換

引数なしの `SendCustomNetworkEvent` は `[NetworkCallable]` なしでも動く（後方互換）。新規コードでは `[NetworkCallable]` を明示推奨。

## late joiner との使い分け

| やりたいこと | 使う機構 |
|------------|---------|
| 全員の共有状態（ドアの開閉、スコア） | **`[UdonSynced]` 変数**（自動で late joiner 配布） |
| 瞬間的アクション（発射音、爆発エフェクト） | **`SendCustomNetworkEvent`**（late joiner は受けない） |
