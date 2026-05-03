# UdonSharp Basics

UdonSharpBehaviour を書くための基本構造。

## スクリプト作成方法

### Project ウィンドウから
プロジェクトフォルダで右クリック → **Create > U# script** → 名前を入力 → `.cs` と `UdonSharpProgramAsset`（`.asset`）が同時生成される。

### UdonBehaviour 経由から
1. シーン内に GameObject を作成
2. **Udon Behaviour** コンポーネントを追加
3. 「Udon C# Program Asset」をドロップダウンから選択
4. 「New Program」ボタン → スクリプト保存先と名前を指定

## 基本テンプレート

```csharp
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class YourScriptName : UdonSharpBehaviour
{
    void Start()
    {
    }
}
```

**守るべきこと：**

- **`MonoBehaviour` ではなく `UdonSharpBehaviour` を継承**
- **クラス名と `.cs` ファイル名は完全一致必須**（`CubeCounter.cs` なら `class CubeCounter`）
- using は `UdonSharp` + `UnityEngine` を最低限入れる。ネットワーク機能を使うなら `VRC.SDKBase` + `VRC.Udon`

## テンプレート：Interact 対応

```csharp
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class MyButton : UdonSharpBehaviour
{
    public override void Interact()
    {
        Debug.Log("Interacted!");
    }
}
```

## テンプレート：Inspector 公開

```csharp
public float speed = 1f;                       // public → Inspector 公開
[SerializeField] private int count;            // private でも SerializeField で公開
[HideInInspector] public string runtimeTag;    // 非表示
[Tooltip("Speed in m/s")] public float spd;    // ツールチップ付き
```

## 作業フロー（コード生成時の必須確認）

1. `rules/language.md` の使える属性・enum を把握
2. 継承元は `UdonSharpBehaviour`、ファイル名=クラス名で作成
3. 同期が必要なら `BehaviourSyncMode` と `UdonSynced` を決める
4. パフォーマンスが問われる場面は `references/performance.md`
5. Editor 拡張が必要なら `references/editor-scripting.md`
6. 実行順序を意識する必要がある場面は `references/event-execution-order.md`
