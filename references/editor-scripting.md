# Editor Scripting

UdonSharpBehaviour のカスタム Inspector、Gizmos、プログラマティック操作のルール。

## 2 つのプリプロセッサシンボル

### `UNITY_EDITOR`

- 通常の Unity プリプロセッサ
- **Editor 内でのみ定義**、本番ビルドでは undef
- `#if UNITY_EDITOR ... #endif` で囲まれたコードは**本番 Udon Program に含まれない**

```csharp
#if UNITY_EDITOR
using UnityEditor;
#endif
```

### `COMPILER_UDONSHARP`

- **UdonSharpBehaviour と同一 `.cs` 内でのみ true**（別ファイルでは常に false）
- UdonSharp がコンパイル中であることを示す
- **UdonSharp に見せたくないコードを隠す**ために使う

### 同一ファイルに Editor コードを書くパターン

```csharp
#if !COMPILER_UDONSHARP && UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(MyBehaviour))]
public class MyBehaviourEditor : UnityEditor.Editor { }
#endif
```

- `!COMPILER_UDONSHARP`：UdonSharp コンパイラからは見えない（Udon に含めない）
- `UNITY_EDITOR`：通常 Unity Editor コンパイル時のみ有効

## 禁則：`#if` でフィールドを追加・削除しない

公式警告：**「Never use these to conditionally add/remove fields」**（シリアライズ不整合により予期しない動作）

```csharp
// NG
#if UNITY_EDITOR
[SerializeField] private int _debugOnly;
#endif
```

フィールド定義は常に unconditional にする。Editor 専用値は `[HideInInspector]` 等で隠す手段を使う。

## CustomEditor の配置

| 方法 | 配置 | 特徴 |
|------|------|------|
| **Editor フォルダ** | `Assets/.../Editor/MyBehaviourEditor.cs` | UdonSharp ビルド対象外として Unity が自動処理。推奨 |
| **同一ファイル** | `#if !COMPILER_UDONSHARP && UNITY_EDITOR` ブロック | 小規模カスタムに |

### 必須パターン：OnInspectorGUI 冒頭

```csharp
public override void OnInspectorGUI()
{
    if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
    // カスタム GUI
}
```

`true` を返すと UdonSharp の状態表示処理が走るので `return` する。

## Proxy System

UdonSharp は `.cs` の C# クラスを**ディセーブルド Proxy として Unity 側に保持**し、実行時の Udon Program とリンクする。

### 制約

- **Proxy は disabled のまま保つ**（有効化するとイベントが二重発火する）
- **`UdonSharpBehaviour` 型フィールド**に保存すれば、UdonSharp が自動で proxy 処理する
- **非 UdonSharpBehaviour 型変数**（例：`UdonBehaviour` 基底型）に proxy 参照を保存すると**ビルド時にクリアされる**
- 参照は**必ず `UdonSharpBehaviour` 型**（または具体派生型）で持つ

## エディタスクリプトから UdonSharp を操作する API

| 操作 | API |
|------|-----|
| コンポーネント追加 | `gameObject.AddUdonSharpComponent<T>()` |
| コンポーネント取得 | `gameObject.GetUdonSharpComponent<T>()`（`GetComponent` ではない） |
| 削除 | `UdonSharpEditorUtility.DestroyImmediate(component)` |

### フィールド更新の正しい流れ

```csharp
myComponent.UpdateProxy();              // 最新値を Proxy に読み込む
myComponent.myField = newValue;
myComponent.ApplyProxyModifications();  // Proxy を Udon に書き戻す
```

## Gizmos の同一ファイル記法

```csharp
#if !COMPILER_UDONSHARP && UNITY_EDITOR
private void OnDrawGizmos()
{
    this.UpdateProxy();          // または this.CopyUdonToProxy()
    // Gizmos 描画
}
#endif
```
