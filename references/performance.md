# Performance（UdonSharp のパフォーマンスチューニング）

## 大前提：Udon は C# の 200〜1000 倍遅い

Udon の 1 命令あたり解釈コストが支配的で、通常の C# なら無視できる処理も Udon では目立つ：

- **40 iterations を毎フレーム回すと FPS に目視できる影響が出る可能性**
- 大規模なループ（数百〜数千要素）は Update 内で回さない

## 最適化戦略

### 1. ループを避ける / Unity 組み込みに逃がす

- GameObject 操作は Unity の Animator / Animation に任せる
- 物理は Rigidbody、パーティクルは ParticleSystem
- Udon はイベント駆動の接続と制御だけに留める

### 2. TimeSlicing（時間分割）

どうしても重い反復処理が必要なら、複数フレームに分散：

```csharp
private int _index;
private const int STEP = 10;

private void Update()
{
    int end = Mathf.Min(_index + STEP, _items.Length);
    for (int i = _index; i < end; i++) Process(_items[i]);
    _index = end;
    if (_index >= _items.Length) _index = 0;
}
```

または `SendCustomEventDelayedFrames` でチェーン化する。

### 3. `GetComponent` は初期化時に限定

- Udon の `GetComponent` は特にコスト高
- `GetComponent<UdonSharpBehaviour>` は**全 UdonBehaviour をループして型検証**するため特に重い
- **`Start()` や低頻度イベント（Interact 等）でのみ使用**
- 結果はフィールドにキャッシュして再利用

### 4. SendCustomEvent を避ける

- 挙動間のメソッド呼出は内部的に `SendCustomEvent` として動く
- **ローカル呼出より明確に遅い**
- 1 つの UdonSharpBehaviour にロジックを集約する方が速い

### 5. `public` メソッドを最小限に

- Udon のメソッド検索は public メソッド数に比例
- 内部ヘルパは `private` に、ネットワーク公開しないものは先頭 `_` 接頭辞でさらに隠す

### 6. メソッド名の `_` 接頭辞

- 先頭 `_` のメソッドは**ネットワークイベント（`SendCustomNetworkEvent`）の対象外**
- ローカルからは通常通り呼べる
- 「ローカルのみで動くヘルパー」を明示するマーカーとして有効

## 避けるべきパターン

### Collision ownership 転送

- 長期間バグが存在し**オーナーシップ転送スパム**を引き起こす
- ワールド全体のパフォーマンス低下要因
- 使わない

### 1 スクリプト複数 ProgramAsset 紐付け

- 各 `.cs` は**1 つの `UdonSharpProgramAsset` のみ**に接続
- 変更すると予測不能な挙動
- プレハブ共有は「同じ ProgramAsset を共有する」で行う
