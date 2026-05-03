# Networking（VRChat ネットワークの基本）

VRChat マルチプレイヤーは **3 つの柱**で構成される：

| 柱 | 役割 |
|----|------|
| **Variables（同期変数）** | 全プレイヤーで共有する永続状態 |
| **Events（ネットワークイベント）** | 一度きりの瞬間的アクション |
| **Ownership（オーナーシップ）** | どのプレイヤーがオブジェクトを書き換えられるか |

詳細は各 reference：

- 同期変数とシリアライゼーション → `references/sync-variables.md`
- ネットワークイベント → `references/network-events.md`
- オーナーシップ → `references/ownership.md`

## 帯域制限

| 上限 | 値 |
|------|------|
| スクリプトあたり送信 | 約 **11 KB/sec** |
| `Continuous` 1 serialization | 約 **200 bytes** |
| `Manual` 1 serialization | 約 **280,496 bytes** |

超過すると `IsClogged` 状態になる：

| SyncMode | 挙動 |
|----------|------|
| `Continuous` | ネットワークイベント送信失敗、ログにエラー、UdonBehaviour のロジックは動作継続 |
| `Manual` | イベントをキャッシュして再送。最終的には届く |

## 複数 UdonBehaviour 同居ルール

同一 GameObject に複数の UdonBehaviour が付いている場合、**最も制約の強い SyncMode が全体に適用**される。例：`Manual` と `Continuous` が同居すると両方 `Manual` として動作。

sync の単位は **GameObject**（UB 単位ではない）。片方の UB が `RequestSerialization` を呼ぶと、同 GO 上の全 UB の `OnPreSerialization` / `OnPostSerialization` が発火する。

## 可視性による優先度

Udon のネットワークは**ローカルプレイヤーから見えている GameObject を優先的に同期**する。同期対象の MeshRenderer 子オブジェクトの可視性を定期的にチェックし、QoS ロードバランシングに使う。視界外の同期オブジェクトは低優先度。

## 帯域節約の設計指針

- **予測可能な軌道は同期しない** — 固定軌道・固定速度なら初期位置・速度・開始時刻だけ送り、各クライアントで計算
- **`RequestSerialization` はいくら呼んでも良いが送信はレート制限される**（データサイズに応じて）
- **Continuous の 200 B 上限は厳しい** — 同期変数が多い / 配列を持つなら Manual 推奨
- **Debug Menu 6** で実際の通信量を確認
- **変数のグルーピング**：関連する bool 複数は int のビットフラグに、関連値は配列に集約
- **float 精度削減**：必要十分な精度で帯域削減
- **オーナー転送は最小限**：頻繁な転送はレイテンシとデシンクを生む（pickup など必要な場合のみ）

## バージョン互換性（ワールド更新時）

ワールド更新時に**同期対象オブジェクト構造が変わる**と、**active インスタンス**と非互換化する。

### 非互換化の条件

- **コンポーネントの型・数・順序**が一致しない
- 各コンポーネント内の**同期変数の型・数・順序**が一致しない

### ユーザーへの影響

- 更新されたワールドに入ろうとすると通知が出る
- **そのインスタンスから追い出される**（acknowledge が必要）
- 全員が退出してインスタンスが inactive になれば、新バージョンで問題なく入れる

### 運用指針

- 同期フィールド・同期コンポーネントの**追加は破壊的変更**として扱う
- 新規フィールドは**末尾に追加**（順序変更を避ける）
- 継続稼働中ワールドへの変更リリース時はダウンタイム計画を

## ネットワーク系コンポーネント

### VRCObjectSync

Transform（位置・回転・スケール）と Rigidbody（物理）を自動同期。

| メソッド | 用途 |
|---------|------|
| `FlagDiscontinuity()` | テレポート時にスムージング無効化 |
| `SetGravity()` / `GetGravity()` | 重力制御（オーナーのみ） |
| `SetKinematic()` / `GetKinematic()` | kinematic 制御（オーナーのみ） |
| `Respawn()` | 初期位置・回転に戻し、速度リセット |

### VRCObjectPool

GameObject 配列を active/inactive で管理する軽量プール。

- `TryToSpawn()` — 無効状態のオブジェクトを有効化して返す
- `Return(GameObject)` — 無効化してプールに戻す
- **late joiner にも自動同期される**

弾丸・エフェクト・使い回しオブジェクト用。

## Networking 関連のプロパティ

| プロパティ | 内容 |
|-----------|------|
| `Networking.LocalPlayer` | ローカルプレイヤー |
| `Networking.IsInstanceOwner` | このクライアントがインスタンスオーナーか |
| `Networking.IsMaster` | Instance Master か |
| `Networking.IsNetworkSettled` | ネットワークが安定状態か |
| `Networking.SimulationTime` | オブジェクトのシミュレーションタイムスタンプ（レイテンシ・複製挙動の理解に使う） |

## 追加のイベント

| イベント | 用途 |
|---------|------|
| `OnVariableChanged` | 個別変数変更の監視（`[FieldChangeCallback]` の代替候補） |
| `OnMasterTransferred` | Instance Master 変更時 |

## デバッグ手段

### World Debug Views

シーンのオーナーシップとネットワーク状態を可視化する公式ツール。`--enable-debug-gui` フラグで起動する。

### 同期バグの典型パターン

| 症状 | 確認項目 |
|------|---------|
| Object Sync が動かない | `VRCObjectSync` コンポーネント有無、`Networking.GetOwner(gameObject)` でオーナー確認、Continuous 有効か |
| Late joiner の状態がずれる | synced 変数ではなくイベントで状態変更していないか、`OnDeserialization()` で再適用しているか |
| Ownership 競合 | `Networking.IsOwner` チェック、`OnOwnershipRequest` の判定 |

### 推奨ログ

```csharp
Debug.Log($"Owner: {Networking.GetOwner(gameObject).displayName}");
```

## ネットワーク統計 API（`VRC.SDK3.Network.Stats`）

ランタイムから統計を取得する静的クラス。

### グローバル統計

- Throughput（出力データ使用率の移動平均）
- RoundTripVariance（RTT 分散）
- BytesIn/OutAverage、BytesIn/OutMax
- HitchesPerNetworkTick（欠落ティック数）
- Suffering（キュー送信数＝輻輳指標）
- TimeInRoom（インスタンス滞在時間）

### オブジェクト/プレイヤー別統計

- UpdateInterval、ReceiveInterval、FinalDelay、GroupDelay
- Sleeping（非アクティブ状態）
- Size、BytesPerSecondAverage、TotalBytes
- キュー中の reliable events 数
- LastSendTime、LastReceiveTime

対象外オブジェクトには default 値が返る。
