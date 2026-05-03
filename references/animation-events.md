# Animation Events

`AnimationClip` から **UdonBehaviour のメソッド**を呼ぶ仕組み。Unity Animation Window の Inspector で Animation Event を追加する。

## 呼び出せるメソッド（allowlist）

以下 13 個のみ。allowlist 外のメソッドは**無視される**：

1. `RunProgram`
2. **`SendCustomEvent`** ← UdonSharp のメソッドを呼ぶのに使う
3. `Play`
4. `Pause`
5. `Stop`
6. `PlayInFixedTime`
7. `Rebind`
8. `SetBool`
9. `SetFloat`
10. `SetInteger`
11. `SetTrigger`
12. `ResetTrigger`
13. `SetActive`

## UdonSharp メソッドを呼ぶ典型

1. UdonSharpBehaviour に `public` メソッドを定義
2. AnimationClip に Animation Event を追加
3. Function に `SendCustomEvent`、String パラメータにメソッド名

```csharp
public class MyAnim : UdonSharpBehaviour
{
    public void OnAnimHit()
    {
        // アニメーション中に発火
    }
}
```

## 用途

- アニメーション中の特定フレームで音・エフェクト発動
- アニメーションステートからの状態更新（Animator の SetBool 等）
- 複雑なタイミング制御を Animator に任せ、Udon はトリガー受信に集中
