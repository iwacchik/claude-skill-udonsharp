# Data Containers（DataToken / DataList / DataDictionary / VRCJson）

`List<T>` / `Dictionary<T,K>` は使えない（`rules/language.md` 参照）。代わりに VRChat SDK の Data Containers を使う。

```csharp
using VRC.SDK3.Data;
```

## DataToken（単一要素の汎用ラッパー）

DataList / DataDictionary の要素は全て `DataToken`。

### 格納可能な型

`Null`, `Boolean`, `SByte`, `Byte`, `Short`, `UShort`, `Int`, `UInt`, `Long`, `ULong`, `Float`, `Double`, `String`, `DataList`, `DataDictionary`, `Reference`（任意オブジェクト、シリアライズ不可）, `Error`

### 作成（UdonSharp は暗黙変換対応）

```csharp
DataToken a = 5;          // Int
DataToken b = 5.3f;       // Float
DataToken c = "value";    // String
DataToken d = true;       // Boolean
DataToken e = new DataToken(obj);   // 明示コンストラクタ
```

### 値取得（プロパティ）

| プロパティ | 内容 |
|-----------|------|
| `Boolean` / `Int` / `Float` / `Double` / `String` | 特定型 |
| `Number` | 数値全般を double として取得 |
| `DataList` / `DataDictionary` | コンテナ取得 |
| `Reference` | オブジェクト参照 |
| `Error` | エラー情報 |
| `TokenType` | 型判定用 |
| `IsNumber` / `IsNull` | ヘルパー判定 |

### 型安全な取り出し

```csharp
if (token.TokenType == TokenType.String) { /* token.String */ }
if (token.IsNumber) { /* token.Number */ }
```

### `String` プロパティと `ToString()` の違い

- `.String` — 型が String の場合のみ正しく値を返す
- `.ToString()` — 任意の型を文字列表現に変換（常に安全）

### Error Token

失敗した操作は Error トークンを返す：
- enum：`KeyDoesNotExist`, `IndexOutOfRange`, `TypeMismatch` 等
- `.ToString()` でエラー詳細を得られる

## DataList（インデックスベースの可変長配列）

### 初期化

```csharp
private DataList _list = new DataList()
{
    "Bananas", "Grapes", "Milk"
};

DataList list = new DataList(64);   // 初期容量指定（SDK 3.10.4+、再確保を減らす）
```

**制約：初期化子は `private` / `[NonSerialized] public` フィールドでのみ。関数内では使えない。**

### 追加

| メソッド | 用途 |
|---------|------|
| `Add(DataToken)` | 末尾追加 |
| `AddRange(DataList)` | 別リストを末尾連結 |
| `Insert(int index, DataToken)` | 指定位置に挿入（以降は後ろへ） |
| `InsertRange(int, DataList)` | 範囲挿入 |

### 取得（安全・非安全）

```csharp
// 安全（推奨）
if (list.TryGetValue(0, out DataToken v)) { /* ... */ }

// 型チェック付き
if (list.TryGetValue(0, TokenType.DataDictionary, out DataToken v)) { /* ... */ }

// ブラケット（危険。不正値で UdonBehaviour が停止しうる）
list[0] = 5;
int sum = list[0].Int + list[1].Int;
```

### 削除

`Remove`, `RemoveAll`, `RemoveAt`, `RemoveRange`, `Clear`

### 検索

`Contains`, `IndexOf`, `LastIndexOf`, `BinarySearch`（ソート済みのみ）

### その他

`.Count`, `.Capacity`, `Sort()`（混合型時は Null → Number → String → DataList → DataDictionary → Reference の順）, `Reverse()`, `GetRange`, `ToArray`, `DeepClone()`（再帰深いコピー）, `ShallowClone()`（浅いコピー、参照維持）, `TrimExcess()`

### ループ

```csharp
for (int i = 0; i < list.Count; i++)
{
    if (list.TryGetValue(i, out DataToken v)) { /* 処理 */ }
}
```

## DataDictionary（キーバリューペア）

### 初期化・追加

```csharp
private DataDictionary _dict = new DataDictionary();
DataDictionary dict = new DataDictionary(64);   // 初期容量指定（SDK 3.10.4+）

_dict.Add("key", 5);          // 既存キー → 例外（初期化の重複検出に有用）
_dict.SetValue("key", 5);     // 上書き可（runtime 向け）
_dict.EnsureCapacity(128);    // 容量を事前確保（SDK 3.10.4+）
```

### 取得

```csharp
if (_dict.TryGetValue("key", out DataToken v)) { /* ... */ }
if (_dict.TryGetValue("key", TokenType.Int, out DataToken v)) { /* ... */ }
```

### 操作

| メソッド | 内容 |
|---------|------|
| `Remove(key)` / `Remove(key, out value)` | 削除 |
| `ContainsKey(key)` | 存在確認 |
| `GetKeys()` | 全キーを `DataList` で返す（ループ推奨） |
| `GetValues()` | 全値を `DataList` で返す（順序保証なし） |

### ループ（推奨：`GetKeys`）

```csharp
DataList keys = _dict.GetKeys();
for (int i = 0; i < keys.Count; i++)
{
    DataToken key = keys[i];
    DataToken value = _dict[key];
}
```

## VRCJson（JSON ⇔ DataContainer 変換）

```csharp
using VRC.SDK3.Data;
```

### シリアライズ

```csharp
if (VRCJson.TrySerializeToJson(_dict, JsonExportType.Minify, out DataToken result))
{
    string json = result.String;
}
```

`JsonExportType`：
- `Minify` — 単行・圧縮（ネットワーク向け）
- `Beautify` — 整形インデント（デバッグ向け）

### デシリアライズ

```csharp
if (VRCJson.TryDeserializeFromJson(json, out DataToken result))
{
    // result は DataList か DataDictionary（JSON の root に合わせる）
    if (result.TokenType == TokenType.DataDictionary) { /* ... */ }
}
```

### 制約

- **Reference 型不可**
- **Dictionary のキーは string のみ**
- **NaN / Infinity 不可**
- ルート要素は必ず Dictionary または List
- **数値は全て double としてデシリアライズされる**（元の型情報は失われる）

## ネットワーク同期（DataList / Dictionary は直接同期不可）

JSON 文字列化して同期する：

```csharp
[UdonSynced] private string _json;
private DataList _list;

public override void OnPreSerialization()
{
    if (VRCJson.TrySerializeToJson(_list, JsonExportType.Minify, out DataToken result))
        _json = result.String;
}

public override void OnDeserialization()
{
    if (VRCJson.TryDeserializeFromJson(_json, out DataToken result))
        _list = result.DataList;
}
```

## Byte and Bit Operations

バイナリレベル操作。通信効率化や低レベル永続化で使う。

### 主要ツール

- **`DataToken.Bitcast`** — プリミティブ型の値保持型変換（`float` ⇔ `int` のビット表現交換など）
- **`System.BitConverter`** — プリミティブ ⇔ byte 配列
- **`System.Buffer`** — raw バイナリ操作

### ユースケース

- 混合データ（int / double / string / 配列等）を byte[] に詰めて送信
- 逆にオフセット追跡しながら読み戻す
- float のビット表現を 16 進で可視化

上級者向け。通常のゲームロジックでは DataContainer + VRCJson で十分。
