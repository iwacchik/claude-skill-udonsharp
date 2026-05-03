# UI Events（uGUI を Udon に繋ぐ）

Unity UI の OnClick / OnValueChanged などから、Udon の `SendCustomEvent` / `Interact` 等を呼び出せる。シンプルな操作は**スクリプトを書かずに Inspector 接続**で済む。

## 使える Unity コンポーネントとメソッド（allowlist）

VRChat はセキュリティ上、呼び出せるメソッドを制限した allowlist を持つ。

### アニメーション・オーディオ

- `Animator`：`Play`, `SetBool`, `SetFloat`, `SetTrigger` 等
- `AudioSource`：`Play`, `Stop`, `Pause`、各種フィルタプロパティ

### UI コントロール

- `Button`, `Slider`, `Toggle`, `Dropdown`, `InputField`

### レンダリング

- `Image`, `Text`, `LineRenderer`, `ParticleSystem` 等

### GameObject

- `GameObject.SetActive(bool)`
- `Light` プロパティ

### UdonBehaviour

- `RunProgram`
- **`SendCustomEvent`**（Udon メソッド呼出）
- **`Interact`**

## 典型：Button → UdonSharp

1. UdonSharpBehaviour で `public` メソッドを定義（引数なし）
2. `Button.OnClick` に対象 GameObject をドラッグ
3. 関数ドロップダウンで **UdonBehaviour → SendCustomEvent**
4. 引数にメソッド名（文字列）を入力

```csharp
public class PlaySoundButton : UdonSharpBehaviour
{
    public void OnClicked()
    {
        Debug.Log("Clicked!");
    }
}
```

Inspector 側で Button の OnClick → target UdonBehaviour の SendCustomEvent("OnClicked")。

## Interact との使い分け

| 用途 | 使うもの |
|------|---------|
| 3D オブジェクト（Cube や Mesh）を触って実行 | **`Interact`**（UdonSharpBehaviour の override） |
| Canvas 上の UI Button（World Space or Screen Space） | **Button.OnClick → SendCustomEvent** |

両方同じ UdonSharpBehaviour に混在させても OK。

## InputField の制限

最大 **16,000 文字**（テキストレンダリングの上限）。

## 頻度・レート制限

通常の Unity UI イベントなので、VRChat のネットワークイベントレート制限とは無関係。**ローカルでの呼び出しのみ**。ネットワーク越しに反映したいなら、UI イベントから `SendCustomNetworkEvent` を呼ぶ UdonSharpBehaviour 経由で。
