# VRCTween（SDK 組み込みトゥイーンシステム）

SDK 3.10.4 で追加。DOTween ベースの補間システムを Udon に公開したもの。UI アニメーション、オブジェクトの滑らかな移動、フェードなどに使う。UdonSharp / Udon Graph 両対応。

```csharp
using VRC.SDK3.Components;   // 公式サンプル準拠
```

## 基本形

拡張メソッドとして GameObject / Transform / 各コンポーネントから呼ぶ。戻り値の `VRCTweenHandle` で制御する。

```csharp
VRCTweenHandle handle = cube.TweenPosition(new Vector3(0, 5, 0), 2f, VRCTweenEase.OutQuad);
```

## Tween メソッド一覧（対象別）

| 対象 | メソッド | 内容 |
|------|---------|------|
| Transform / GameObject | `TweenPosition` / `TweenLocalPosition` | ワールド / ローカル位置 |
| 〃 | `TweenRotation` / `TweenLocalRotation` | オイラー角回転 |
| 〃 | `TweenScale` | localScale |
| 〃 | `TweenPath` / `TweenLocalPath` | ウェイポイント移動（下記） |
| Graphic（Image/Text 等） | `TweenColor` / `TweenFade` | 色 / アルファ |
| CanvasGroup | `TweenFade` | パネル全体フェード |
| Slider | `TweenValue` | HP バー等 |
| RectTransform | `TweenAnchorPos` / `TweenSizeDelta` | UI レイアウト |
| Renderer | `TweenColor(string prop, ...)` / `TweenFloat(string prop, ...)` | **MaterialPropertyBlock 経由**のシェーダープロパティ（マテリアル複製なし） |
| SpriteRenderer | `TweenColor` / `TweenFade` | スプライト |
| Light | `TweenIntensity` / `TweenColor` | ライト |
| AudioSource | `TweenVolume` / `TweenPitch` | 音量 / ピッチ |

共通シグネチャ：`Tween○○(値, float duration, VRCTweenEase ease) → VRCTweenHandle`

### パス移動

```csharp
TweenPath(Vector3[] waypoints, float duration, VRCTweenPathType pathType,
          bool closePath, int resolution, VRCTweenEase ease)
```

- `waypoints`：最小 2 点。NaN / Infinity は拒否
- `pathType`：`Linear` / `CatmullRom`（曲線）
- `closePath`：true で終点→始点を自動接続（ループ巡回は `closePath=true` + `SetLoops(-1, Restart)`）
- `resolution`：CatmullRom の滑らかさ（1–50 に自動クランプ、デフォルト 10）

## VRCTweenHandle の制御

| メソッド | 内容 |
|---------|------|
| `Kill()` | 停止・破棄 |
| `Pause()` / `Resume()` / `Restart()` | 一時停止 / 再開 / 最初から |
| `Goto(float time, bool andPlay)` | 指定時刻へシーク |
| `PlayForwards()` / `PlayBackwards()` / `Flip()` | 再生方向制御（`Flip` は方向切替のみ、再生は別途） |
| `IsValid` | ハンドル有効判定（作成失敗検知にも使う） |
| `gameObject.KillAllTweens()` | 対象オブジェクトの全 tween 破棄（`OnDestroy` でのクリーンアップ等） |

## 設定メソッド（チェーン可）

**作成直後のみ有効**：

| メソッド | 内容 |
|---------|------|
| `SetLoops(int loops, VRCTweenLoopType type)` | -1 で無限。`Restart` / `Yoyo` / `Incremental` |
| `SetDelay(float)` | 開始遅延 |
| `SetUpdate(VRCTweenUpdateType)` | `Update` / `LateUpdate` / `FixedUpdate` / `PostLateUpdate` |
| `From()` | 開始値と終了値を反転（現在値へ向かって再生） |
| `SetSpeedBased()` | duration を「秒」でなく「単位/秒」として解釈 |
| `OnComplete(UdonBehaviour, string eventName)` | 完了コールバック（**メソッドは public 必須**） |
| `OnRewind(UdonBehaviour, string eventName)` | 逆再生が開始位置に到達した時（`Restart()` では発火しない） |

**アクティブな tween でも呼べるもの**：

| メソッド | 内容 |
|---------|------|
| `SetEase(VRCTweenEase)` / `SetEase(AnimationCurve)` | イージング変更（カスタムカーブ可） |
| `SetDuration(float)` | 時間変更 |
| `ChangeEndValue(値, bool snapStartValue)` | 終了値変更。仮想 Color/Vector3・MaterialPropertyBlock・Light・Path では不可 |

## コールバック付きシーケンス

```csharp
door.TweenPosition(openPos, 1f, VRCTweenEase.OutQuad)
    .OnComplete(this, nameof(OnDoorOpened));

public void OnDoorOpened() { /* 次の tween を発火してシーケンス化 */ }
```

## DelayedCall / DelayedSetActive（キャンセル可能な遅延実行）

`SendCustomEventDelayedSeconds` と違い**ハンドルで中断できる**：

```csharp
private VRCTweenHandle _timer;

_timer = VRCTween.DelayedCall(this, nameof(OnTimer), 3.0f);
_timer.Kill();   // キャンセル

VRCTween.DelayedSetActive(myObject, false, 3f);   // 遅延 SetActive(対象破棄時は無視)
```

## Virtual Tweens（任意値のアニメーション）

値を public 変数に書き込み、毎フレームコールバックを呼ぶ。FOV・スコア・Animator パラメータ等、組み込み tween が無い対象に使う。

```csharp
VRCTween.TweenFloat(float from, float to, float duration,
    UdonBehaviour target, string valueName, string callbackName, VRCTweenEase ease)
// 同様に TweenInt / TweenColor / TweenVector3
```

```csharp
[System.NonSerialized] public float fovValue;

void Start()
{
    VRCTween.TweenFloat(60f, 90f, 2f, this,
        nameof(fovValue), nameof(OnFovUpdate), VRCTweenEase.OutQuad);
}

public void OnFovUpdate()
{
    myCamera.fieldOfView = fovValue;
}
```

- 受け側変数は **public 必須**、`[System.NonSerialized]` 推奨
- Vector3 は軸別でなくベクトル全体にイージング適用

## 入力検証（silent ignore に注意）

作成失敗・無視される条件。失敗時は例外でなく**静かに無視される**ので `handle.IsValid` で確認する：

- target が null
- duration が負数 / NaN / Infinity（0 は許可）
- 位置・スケール・パスに NaN / Infinity、または絶対値 ~520,000 超
- `SetDuration` / `SetDelay` / `Goto` への無効値

## ネットワーク

**Tween はローカル実行で同期されない**。同期が必要なら：

```csharp
[UdonSynced] float _tweenStartTime;   // オーナーが Networking.GetServerTimeInSeconds() を書く

public override void OnDeserialization()
{
    float elapsed = (float)(Networking.GetServerTimeInSeconds() - _tweenStartTime);
    _syncedTween.Goto(elapsed, true);   // サーバー時刻基準でシーク
}
```

## パフォーマンス

- 0.01 秒未満の duration は滑らかにならない
- 数百同時 tween は避け、`SendCustomEventDelayedSeconds` でずらして生成する
- **高頻度に作り直すなら再利用**（公式ベンチマーク：500 tween × 300 フレームでメモリ 46 倍削減・10 倍高速）：

```csharp
VRCTweenHandle _moveHandle;

void Start()
{
    _moveHandle = gameObject.TweenPosition(Vector3.zero, 1f, VRCTweenEase.OutQuad)
        .SetLoops(-1, VRCTweenLoopType.Restart)
        .Pause();   // 無限ループ + Pause で生存させておく
}

public void MoveTo(Vector3 target, float duration)
{
    _moveHandle.ChangeEndValue(target, true)
        .SetDuration(duration)
        .SetEase(VRCTweenEase.OutCubic);
    _moveHandle.Restart();
}
```

- ワンショット（ドア・ボタン等）は使い捨てで問題ない
