# VRCGraphics（GPU / カメラ / 品質 / シェーダー）

高度なグラフィックス API。通常のワールドでは使わない場面が多いが、ポストエフェクト・カメラ制御・品質設定などが必要な時に使う。

## `VRCGraphics`

Unity `Graphics` クラスの一部を Udon に公開したもの。

### `VRCGraphics.Blit(source, dest, material, ...)`

RenderTexture へのコピー。

**Quest 注意**：シェーダーに `ZTest Always` を追加するか、対象 RenderTexture の Depth を無効化する。しないと失敗する。

### `VRCGraphics.DrawMeshInstanced(...)`

同じ Mesh を GPU で大量レンダリング。

## `VRCShader`

グローバルシェーダプロパティ管理。

### `VRCShader.PropertyToID(string name) → int`

シェーダプロパティ名から ID を取得。**プロパティ名は `_Udon` 接頭辞付き、または `_AudioTexture` のみ** `SetGlobal` で使える。

```csharp
int id = VRCShader.PropertyToID("_UdonMyValue");
```

### `VRCShader.SetGlobalX(int id, X value)`

ワールド全体のシェーダ（アバターシェーダ含む）にグローバル値を渡す。

対応型：Color / Float / Int / Matrix / Texture / Vector（および配列形）

## `VRCAsyncGPUReadback`

GPU の RenderTexture / Texture から非同期で CPU にデータを読み取る。フレームをブロックしない。

### Unity 標準との違い

- クラス名：`VRCAsyncGpuReadback`（Unity は `AsyncGpuReadback`）
- コールバック：UdonBehaviour 経由（Action 不可）
- 完了通知：`OnAsyncGpuReadbackComplete` イベント
- データ取得：`TryGetData`（`GetData` でなく）

### 実装ステップ

1. 結果格納配列を初期化
2. Readback リクエストを投げる
3. `OnAsyncGpuReadbackComplete` でコールバック受信
4. `TryGetData` で成功判定＋データ取得

## `VRCCameraSettings`

プレイヤーカメラへの限定アクセス（生 Camera コンポーネントは取れない）。

### 取得可能なカメラ

- **`ScreenCamera`** — メインビューポート（VR だと Stereo）
- **`PhotoCamera`** — 手持ち撮影カメラ（ClientSim では null）

### Read-Only プロパティ

- `Position`, `Rotation`, `Forward`, `Up`, `Right`
- `PixelWidth`, `PixelHeight`, `Aspect`, `FieldOfView`

### 書換可能プロパティ

- `NearClipPlane`（0.001-0.05）, `FarClipPlane`
- `AllowHDR`
- Depth Texture モード
- Occlusion Culling, MSAA
- `CullingMask`（レイヤー可視性）
- Clear Flags

### イベント

```csharp
public override void OnVRCCameraSettingsChanged() { }
```

**毎フレーム発火することがある**ので重い処理は入れないこと。

## `VRCQualitySettings`

Unity Quality 設定への**読み取りアクセス**＋一部書き込み。

### 取得できる項目

`AntiAliasing`, `PixelLightCount`, `LODBias`, `MaximumLODLevel`, `ShadowResolution`, `ShadowDistance`, `ShadowCascades`, `vSyncCount`

### 書換（一部のみ）

```csharp
VRCQualitySettings.SetShadowDistance(low, med, high, mobile);
// 各値 0.1f 〜 10000.0f
// 実行時ユーザーに警告が出る

VRCQualitySettings.ResetShadowDistance();
```

Read-Write：`shadowCascade2Split`, `shadowCascade4Split`

### イベント

```csharp
public override void OnVRCQualitySettingsChanged() { }
```

ワールド遷移時に設定はリセットされる。

## VRChat Shader Globals

シェーダから使える VRChat 固有のグローバル変数。

### カメラモード

- `_VRChatCameraMode` — 通常描画 / VR 手持ち / Desktop / スクリーンショット等を識別

### 時刻関連

- `_VRChatTimeNetworkMs` — 同期ネットワーク時刻（ms）
- `_VRChatTimeUTCUnixSeconds` — UNIX epoch 秒
- 時刻パック変数（時分秒ミリ秒、タイムゾーンがビット詰めされている）

### ヘルパー

SDK 同梱：**`VRCTime.cginc`** — 時刻フォーマットをデコードする HLSL ユーティリティ。

### 禁則

**独自のシェーダ変数に `_VRChat` 接頭辞を使わない**。将来の新規グローバル変数と衝突する。
