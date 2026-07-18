# VRC Scene Components

VRChat ワールドで使う VRC 系コンポーネント一覧。

## `VRCSceneDescriptor`（必須）

ワールドに**必ず 1 つ配置**するコンポーネント。無いとワールドとして認識されない。通常は `VRCWorld` プレハブ経由で追加。

### 主要パラメータ

| 分類 | 項目 | 内容 |
|------|------|------|
| **Spawn** | `spawns[]` | スポーン位置配列（Transform） |
| | SpawnOrder | `First` / `Sequential` / `Random` / `Demo` |
| | SpawnOrientation | Spawn 位置の向き、ルームスケール対応 |
| **World** | Reference Camera | 全プレイヤーへ適用されるカメラ設定（Post Processing 等） |
| | Respawn Height Y | 落下したプレイヤー / オブジェクトが戻される下限 Y |
| | Object Behaviour at Respawn | `Destroy` / `Respawn` |
| **Access** | Allow Non-Master to Manipulate Unsynced | 非マスターの操作可否 |
| | Disable Player Portals | ユーザーポータル禁止 |
| | Voice falloff range | カスタム音量減衰 |
| | Interaction Layer Mask | Interact 対象レイヤー |

### 非推奨フィールド

- `Dynamic Prefabs` / `Dynamic Materials` — SDK3 では使わない

## `VRCObjectSync`

GameObject の Transform と Rigidbody をネットワーク同期。

### 同期される状態

- Position / Rotation / Scale
- Kinematic 状態
- Gravity 状態

### パラメータ

| 項目 | 内容 |
|------|------|
| **Allow Collision Ownership Transfer** | 衝突で所有権が移譲されるか（**バグ既知、OFF 推奨**） |
| **Force Kinematic On Remote** | 非オーナー側で kinematic 固定 |

### メソッド

```csharp
objSync.SetKinematic(true);
objSync.SetGravity(false);
objSync.FlagDiscontinuity();   // テレポート時のスムージング無効
objSync.TeleportTo(pos, rot);
objSync.Respawn();             // 初期位置・速度リセット
```

## `VRC_Pickup`

持ち運び可能なオブジェクト。要件：**Rigidbody + Collider**。

### パラメータ

| 項目 | 内容 |
|------|------|
| **Auto Hold** | 一度掴むと自動で手に固定（もう一度で離す） |
| **Disallow Theft** | 他プレイヤーが奪えない |
| **Allow Manipulation When Equipped** | 掴んだまま位置調整可 |
| **Exact Gun / Grip** | 掴み位置を定義 |
| **Throw Velocity Boost** | 投げ速度スケール |
| **Pickup Orientation** | Grip / Gun / Any |
| **Interaction Text** | プロンプト表示 |

### Grabbable 判定

- **Raycast**：VR は手 → インパクト、Desktop はカメラ位置ベース
- **Hover（VR のみ）**：手の周囲 0.4m の球で判定、微細な交差を許す

### 関連イベント（UdonSharpBehaviour で override）

- `OnPickup()` / `OnDrop()`
- `OnPickupUseDown()` / `OnPickupUseUp()`

## `VRC_Station`

座るためのコンポーネント。StationGraph サンプルあり。

### パラメータ

| 項目 | 内容 |
|------|------|
| **Player Mobility** | `Mobile` / `Immobilize` / `Immobilize For Vehicle` |
| **Can Use Station From Station** | 座ったまま別 Station に移動可 |
| **Animator Controller** | カスタム着席アニメーション |
| **Disable Station Exit** | 通常の退出を禁止（Trigger で制御） |
| **Seated** | 座り姿勢 vs 立ち姿勢 |

### 典型構成

1. `VRC_Station`（Entry / Exit 指定）
2. Collider（通常 Is Trigger）
3. `UdonBehaviour`
4. 任意の Mesh

### イベント

```csharp
public override void OnStationEntered(VRCPlayerApi player) { }
public override void OnStationExited(VRCPlayerApi player) { }
```

### アバター側の Animator パラメータ

- `InStation` — Station 着席中
- `Seated` — 座り IK 有効
- `AvatarVersion` — SDK2 / SDK3 の識別

### アバター側制約

- **最大 6 Station / アバター**
- Entry / Exit 点は Animator から 2m 以内

## `VRC_SpatialAudioSource`

3D 空間音響を AudioSource に付与。

### パラメータ

| 項目 | 内容 |
|------|------|
| **Gain** | 0-24 dB（world デフォルト 10、avatar 最大 10） |
| **Far** | 減衰終了半径、デフォルト 40m（avatar 最大 40m） |
| **Near** | 減衰開始半径、リアルには 0m 推奨 |
| **Volumetric Radius** | 音源を点でなく球として見せる |
| **Use AudioSource Volume Curve** | カスタムカーブ使用（デフォルトは逆 2 乗） |
| **Enable Spatialization** | 空間化処理 ON/OFF |

### アバター側の制約

- Gain 10 dB 上限
- Far 40m 上限
- 圧縮器による過大音量の自動抑制
- GameObject 全体でなく **AudioSource コンポーネント単位で enable/disable** 推奨

### ベストプラクティス

- 2D audio より spatial audio を優先
- アバター音ファイルは **-6〜-12 dB で正規化**（圧縮器回避）

## TextMeshPro

VRChat は以下 3 コンポーネントを Udon に公開：

- `TMP_Text`（`TextMeshProUGUI` と 3D の `TextMeshPro` 両方の基底）
- `TMP_Dropdown`
- `TMP_InputField`

### Unity 標準 Text より優位

- リッチテキスト対応
- **文字数制限なし**（Unity 標準は 16,250 文字、アンチエイリアス不良）

### UdonSharp フィールド型の推奨

**必ず `TMP_Text` で持つ**：

```csharp
[SerializeField] private TMPro.TMP_Text countText;
// × TextMeshPro countText (3D 専用)
// × TextMeshProUGUI countText (UGUI 専用)
```

基底型にすることで 3D / UGUI どちらも受け取れる。

### フォント管理

- デフォルト **LiberationSans SDF**
- カスタムフォントは**アトラス解像度を下げ・使わない文字を除外**すると DL/RAM 軽量化
- フォールバックフォントアセットを外すと、VRChat 内蔵フォールバックで Unicode 欠落をカバー（Editor では box 表示になる）

### TMP_Text の主要プロパティ（Udon 公開）

`text` / `color` / `alpha` / `fontSize` / `enableAutoSizing`（+ `fontSizeMin` / `fontSizeMax`）、
配置（`alignment` / `horizontalAlignment` / `verticalAlignment`）、
間隔（`characterSpacing` / `wordSpacing` / `lineSpacing` / `paragraphSpacing`）、
折り返し・表示（`enableWordWrapping` / `overflowMode` / `richText` / `parseCtrlCharacters` / `isRightToLeftText`）、
表示制限（`firstVisibleCharacter` / `maxVisibleCharacters` / `maxVisibleWords` / `maxVisibleLines`）、
`fontMaterial`（取得時にマテリアルをクローン）/ `fontSharedMaterial`（共有）

### TMP_InputField

| メンバー | 内容 |
|---------|------|
| `text` | 入力値の取得・設定 |
| `isFocused` | フォーカス状態（読み取り専用） |
| `readOnly` / `richText` | 読み取り専用化 / リッチテキスト許可 |
| `SetTextWithoutNotify(string)` | `onValueChanged` を発火させずに値変更 |

Unity 標準の `UI.InputField` でなくこちらを使う。

### TMP_Dropdown

| メンバー | 内容 |
|---------|------|
| `value` | 選択中インデックスの取得・設定 |
| `IsExpanded` | 展開中か |
| `SetValueWithoutNotify(int)` | イベント発火なしで値変更 |
| `RefreshShownValue()` | 表示中のテキスト・画像を更新 |
| `ClearOptions()` / `Show()` / `Hide()` | 選択肢クリア / 表示 / 非表示 |

選択肢の動的追加は VRChat SDK 拡張の `VRCTMPDropdownExtension` で行う：

```csharp
dropdown.AddOptions(new string[] { "A", "B" });   // string[] / Sprite[] / OptionData[] 対応
dropdown.AddOptions(new TMP_Dropdown.OptionData[]
{
    new TMP_Dropdown.OptionData("テキスト", sprite)
});
```

## `VRC_AvatarPedestal`

アバターを展示してインタラクトで切替させるコンポーネント。

| 項目 | 内容 |
|------|------|
| Blueprint ID | アバター識別子（必須） |
| Placement | 位置合わせ用 Transform |
| Change Avatar On Use | Interact で自分のアバターに適用するか |
| Scale | 表示スケール |

### アバターの可視性

- **Public** — 全員が見て使える
- **Private** — 投稿者本人のみ、他人はエラー
- **Marketplace** — 全員表示、非所有者は詳細ページに飛ぶ

### Udon から

`SetAvatarUse` メソッドでローカルプレイヤーのアバターを切り替える（ワールド側トリガー向け）。

## `VRC_CameraDolly`

カメラのアニメーション動作。ネスト構造：

- `VRCCameraDollyAnimation`（親）
- └ `VRCCameraDollyPath`（子、複数可）
- &nbsp;&nbsp; └ `VRCCameraDollyPoint`（孫、キーフレーム）

### セットアップ

1. Animation コンポーネントのパラメータ設定
2. Path 子オブジェクト + Point 孫で軌道定義
3. **"Collect Paths & Points"** で階層を登録
4. ランタイムで `VRCCameraDollyAnimation.Import()` を呼ぶ

### 設定項目

- 相対 / ワールド位置
- 時間 or 速度ベースの動作
- Look-At-Me オフセット
- グリーンスクリーン HSL 調整
- 補間タイプ、ループ挙動

### 制約

**ClientSim でプレビュー不可**。Build & Test で実機確認する。

## `VRC_MirrorReflection`

ミラーのリアルタイム反射。同一 GameObject に **MeshRenderer 必須**。Material の `_MainTex` に書き込む。

### 主要設定

| 項目 | 内容 |
|------|------|
| Reflect Layers | 反射対象レイヤー（**Water レイヤーは絶対に反射されない**） |
| Mirror Resolution | 片目あたり最大 **2048×2048**、Auto あり |
| Camera Clear Flags | Skybox / Solid Color |
| Antialiasing | MSAA 上限設定 |
| Pixel Lights | リアルタイム光 ON/OFF |

### パフォーマンス注意

ミラーは重い。以下を推奨：

- **デフォルト OFF**、近づいた時・ユーザー操作時に ON
- 反射レイヤーを最小限に
- 必要ならユーザーに質感レベルを選ばせる
- 2048 超の解像度は品質が落ちる（ユーザー設定で Unlimited 可だが代償大）

### SDK プレハブ

`VRCMirror.prefab` 参照。

## `VRC_PortalMarker`

別ワールドへのポータル。Udon 不要で設置可。

### ターゲット指定方法

| 方法 | 内容 |
|------|------|
| World ID | `wrld_xxx` 形式で特定ワールド |
| Home World | プレイヤー自身のホーム |
| Hub World | VRChat Hub |

### オプション

- Custom Portal Name — 表示名上書き
- 対象 Public インスタンスが無い場合、**自動で新規作成**

## `VRC_UIShape`

Canvas をレーザーポインタで遠隔操作可能にする（VRChat メニューのような UI 操作）。

### セットアップ要件

1. `VRC_UIShape` を **Canvas と同じ GameObject** に
2. **レイヤーを UI から Default に変更**（UI レイヤーだと interact 不可）
3. **Scale を 0.01 程度に**（1px = 1m なため）
4. Canvas の Render Mode を **World Space** に
5. Box Collider を追加

### よくある不具合

| 症状 | 原因 |
|------|------|
| ポインターが出ない | VRC_UIShape が Canvas でなく子に付いている / UI レイヤー / Box Collider 無し |
| ポインター出るが反応しない | EventSystem 不在、Raycast Target OFF、Graphic Raycaster 無し、透明要素がブロック |
| 移動中に誤操作 | Navigation を None にする |

### Focus View（モバイル向け）

Canvas 側で **Allow Focus View** 有効化すると、モバイル/タブレットでユーザーが UI を拡大閲覧可能（距離 0.6-6m の範囲）。

## `VRC_EnablePersistence`

PlayerObject の synced 変数を永続化する。

### 要件

同じ GameObject に **`VRCPlayerObject` コンポーネント**が必要。

### 挙動

PlayerObject 上の全 UdonBehaviour とその**子 GameObject の UdonBehaviour** も含めて、`[UdonSynced]` 変数が永続化される。

### 用途

- プレイヤー固有インベントリ
- セーブデータ（PlayerData より構造化された状態向け）
- 個別カスタマイズ設定
