# ClientSim

Unity Editor 内で VRChat クライアントの挙動をシミュレートするツール。**VRChat Worlds SDK に同梱**。

## できること

- Unity Editor で Play Mode に入るだけで、VRChat クライアントを起動せずにテスト
- キーボード / マウス / ゲームパッドでプレイヤー操作
- Pickup / UI / Station とのインタラクト
- Play 中の Udon 変数を inspector で確認
- テスト時に**Editor-only オブジェクトを自動除去**

## 起動

1. Unity でワールドシーンを開く
2. **Play ボタンを押す**だけ
3. Game ウィンドウでテスト

## 重要な制約

> "Always test your world in VRChat before making it public!"

ClientSim で全てを検証することはできない。特に**ネットワーク系の挙動は大きく異なる**：

- **ローカルプレイヤーのみ**シミュレート（リモートプレイヤー不在）
- **シリアライゼーションイベントのデータ構造が本番と異なる場合**あり
- 実ネットワーク経由ではないため、`OnPostSerialization` の `byteCount` や `success` は実測値ではない

→ `#if UNITY_EDITOR` で OnPostSerialization を擬似発火する設計が有効（`references/editor-scripting.md` 参照）。

また、**Play → Edit 切替時に UdonBehaviour の `OnDisable()` 等の Unity イベントは実行されない**（SDK 3.10.5+、VRChat クライアントの挙動に合わせた変更）。Play 終了時の後始末を `OnDisable` 等で確認することはできない。

## 付属エディタウィンドウ

- **PlayerObject Editor** — PlayerObject の状態を編集・確認
- **PlayerData Editor** — 永続化データを編集・確認

ローカルテスト時のデータは **`Assets/` 内に JSON で保存**される。

## 自動テストフレームワーク

ClientSim には **integration test framework** が含まれており：

- 入力イベントを送る（ボタン押下、UI 操作のシミュレート）
- ClientSim イベントを listen して挙動検証
- ワールドの機能テストをプログラマティックに実行可能

CI に組み込める。

## PlayerObject Editor（Play 中の PlayerObject 調査）

Play Mode に入ると、ClientSim が PlayerObject を自動スポーンし、永続化プロパティを復元する。

### 主要コンポーネント

- `ClientSimNetworkingView` — ネットワーク側面の管理
- `ClientSimNetworkIdHolder` — synced プロパティ名と値の表示

### Inspector 表示

- **Network Id** — シーンロード前に割り当てられる ID
- **Network Components** — 永続化データを含む保存コンポーネント一覧

### 保存場所

```
{projectName}/ClientSimStorage/PlayerObjects/PlayerObjects_#_SceneName.json
```

エクスプローラーからも直接開ける。

## PlayerData Editor Window（Play 中の PlayerData 調査）

`VRChat SDK > ClientSim PlayerData` で開く。

### 機能

- **プレイヤー選択** — ローカル / リモートプレイヤー別に data keys を表示
- **値の表示・編集** — key / value / type をリアルタイム確認
- **クリア / リフレッシュ** — シーンの PlayerData を全削除、表示更新
- **JSON 直接編集**：Play Mode 中に JSON ファイルを編集すると**即座に反映**
- **リモートプレイヤー用ランダムデータ生成** — 多人数テスト向け

### 保存場所

```
{projectName}/ClientSimStorage/PlayerData/
```

シーンごとに JSON ファイル。**シーン名を変えたら JSON ファイル名も変える必要**あり。
