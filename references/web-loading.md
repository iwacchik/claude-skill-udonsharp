# Web Loading（Image / String ダウンロード）

実行時に外部 URL からコンテンツを取得する API。掲示板、動的テクスチャ、データ駆動コンテンツに使う。

## 共通：VRCUrl

**`VRCUrl` は Inspector 経由で設定した URL のみ許可**される（スクリプトから `new VRCUrl(string)` で作ると弾かれる）。

- `[SerializeField] private VRCUrl url;`
- 動的入力なら `VRCUrlInputField`（UI コンポーネント）でユーザー入力

## Image Loading（`VRCImageDownloader`）

### using

```csharp
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
```

### 基本

```csharp
public class ImageLoader : UdonSharpBehaviour
{
    [SerializeField] private VRCUrl url;
    [SerializeField] private Material material;

    private VRCImageDownloader _downloader;

    private void Start()
    {
        _downloader = new VRCImageDownloader();
        _downloader.DownloadImage(url, material, (IUdonEventReceiver)this, null);
    }

    public override void OnImageLoadSuccess(IVRCImageDownload result) { }
    public override void OnImageLoadError(IVRCImageDownload result)
    {
        Debug.LogError(result.Error);
    }

    private void OnDestroy()
    {
        if (_downloader != null) _downloader.Dispose();
    }
}
```

### `DownloadImage` パラメータ

| パラメータ | 用途 |
|-----------|------|
| `url` | `VRCUrl` |
| `material` | 完了時に `_MainTex` を自動差し替え（省略可） |
| `udonBehaviour` | `(IUdonEventReceiver)this` などコールバック先（省略可、省略時は現在の UB） |
| `textureInfo` | テクスチャ設定（省略可） |

戻り値：`IVRCImageDownload`

### `TextureInfo` 設定項目

`GenerateMipmaps`, `FilterMode`, `WrapModeU/V/W`, `AnisoLevel`（デフォルト 9、9-16 にクランプ）, `MaterialProperty`（デフォルト `_MainTex`）

### `IVRCImageDownload` プロパティ

| プロパティ | 内容 |
|-----------|------|
| `Error` / `ErrorMessage` | エラー情報 |
| `Material` | 結果を適用した material |
| `Progress` | 0-1 の進捗 |
| `Result` | 完了時の `Texture2D` |
| `SizeInMemoryBytes` | メモリ上のサイズ |
| `State` | ダウンロード状態 |
| `TextureInfo` | 使った設定 |
| `UdonBehavior` | コールバック先 |
| `URL` | URL |

### VRAM 管理（必須）

**使い終わったら `Dispose()` で必ず解放**。しないとリーク＆最悪 VRChat クラッシュ。

```csharp
_downloader.Dispose();  // 全 download と texture が無効化
// または個別
imageDownload.Dispose();
```

### 制限

- **最大解像度 2048 × 2048**
- **5 秒に 1 画像**
- **キュー 1000 要素上限**
- **バッファ 32 MB 上限**

大量の一括ロードは自前でキュー制御が必要。

## String Loading（`VRCStringDownloader`）

### using

```csharp
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
```

### 基本

```csharp
public class StringLoader : UdonSharpBehaviour
{
    [SerializeField] private VRCUrl url;

    private void Start()
    {
        VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        string text = result.Result;
        byte[] bytes = result.ResultBytes;
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.LogError($"{result.ErrorCode}: {result.Error}");
    }
}
```

### `VRCStringDownloader.LoadUrl`

```csharp
static void LoadUrl(VRCUrl url, IUdonEventReceiver udonBehaviour);
```

`udonBehaviour` に自分を渡さないとコールバックを受けない。

### `IVRCStringDownload` プロパティ

| プロパティ | 型 | 内容 |
|-----------|-----|------|
| `Result` | string | UTF-8 デコード済み |
| `ResultBytes` | byte[] | 生データ |
| `Error` | string | エラーメッセージ |
| `ErrorCode` | int | HTTP エラーコード |
| `Url` | VRCUrl | URL |
| `UdonBehaviour` | UdonBehaviour | コールバック先 |

`ResultBytes` を使えば UTF-8 以外のエンコードにも対応できる。

## String Loading の制限と許可ドメイン

### 制限
- **レート**：5 秒に 1 ダウンロード（超過分はランダムキュー）
- **ファイルサイズ**：1 ストリングあたり最大 **100 MB**
- **キュー上限**：1000 要素

### 標準で許可されるドメイン（Allow-list）

以下は**「Allow Untrusted URLs」設定不要**で誰でもダウンロード可能：

- `*.disbridge.com`
- `*.github.io`（GitHub Pages）
- `gist.githubusercontent.com`
- `pastebin`
- `*.vrcdn.cloud`

これ以外のドメインは、**プレイヤー側で「Allow Untrusted URLs」設定を ON にしている必要がある**。

### SDK 提供プレハブ

自前実装せずに使える：**`DownloadString`** スクリプトを UdonBehaviour に乗せて URL と対象 Text コンポーネントを設定。

## `VRCUrl` のルール

- **コンストラクタは Editor 時のみ有効**（runtime で `new VRCUrl(string)` しても弾かれる）
- `VRCUrl.Empty` 静的プロパティ（null 参照代わり）
- `Get()` で文字列取得、`IsNullOrEmpty()` で検証
- `VRCUrlInputField` でユーザー入力を受け取る

## 共通の注意点

- **ローカルクライアントのみで実行**（プレイヤーごとに独立ダウンロード）
- 結果を他プレイヤーと共有したいなら、受信後に `SendCustomNetworkEvent` で通知
- クライアント設定によっては信頼ドメインのみに制限される場合がある

## 許可ドメイン（Trusted URLs）

以下のドメインは **allowlist 済み**で、ユーザー追加設定なしでアクセス可能：

- `*.disbridge.com`（Disbridge）
- `*.github.io`（GitHub Pages）
- `gist.githubusercontent.com`（GitHub Gist）
- `pastebin.com`（Pastebin）
- `*.vrcdn.cloud`（VRCDN）

その他ドメインはユーザーが設定で「Allow Untrusted URLs」を有効化しないとアクセスできない。

## String Loading の追加制限

- **1 ファイル 100 MB 上限**
- **5 秒に 1 ダウンロード**（Image Loading と同じ）
- **キュー 1000 要素**

## `VRCUrl` クラスの API

```csharp
VRCUrl url = new VRCUrl("https://...");    // Editor 時のみ有効
VRCUrl empty = VRCUrl.Empty;               // 空参照

string s = url.Get();
bool isEmpty = VRCUrl.IsNullOrEmpty(url);
```

ランタイム入力：`VRCUrlInputField`（UI コンポーネント）経由。

## プレビルト：`DownloadString` スクリプト

SDK は「URL と Text コンポーネントを指定するだけで使える」標準スクリプトを提供。単純な掲示板程度なら自作不要。
