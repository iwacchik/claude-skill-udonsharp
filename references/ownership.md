# Object Ownership

## 基本ルール

- VRChat でネットワーク対象のオブジェクトは常に**オーナー（1 プレイヤー）**を持つ
- **オーナーのみが `[UdonSynced]` 変数を書き換えられる**（他者の書き換えはローカル止まり）
- **インスタンスに最初に入ったプレイヤー**がデフォルトで全オブジェクトのオーナー

## 自動移譲

オーナーが退出するとき、**VRChat が自動で残りプレイヤーから新オーナーを割り当てる**。`OnPlayerLeft` 発火前に完了しているので、`OnPlayerLeft` 内で `Networking.IsOwner` は新オーナー基準で答える。

## API

```csharp
// 確認
bool mine = Networking.IsOwner(gameObject);
bool mine2 = Networking.IsOwner(player, gameObject);
VRCPlayerApi owner = Networking.GetOwner(gameObject);

// 移譲（自分を新オーナーに）
Networking.SetOwner(Networking.LocalPlayer, gameObject);
```

## 関連イベント

### `OnOwnershipRequest`

移譲前に**拒否できる**：

```csharp
public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi newOwner)
{
    return true;   // 許可、false で拒否
}
```

### `OnOwnershipTransferred`

移譲完了後：

```csharp
public override void OnOwnershipTransferred(VRCPlayerApi newOwner) { }
```

## Instance Master との違い

**Instance Master** と **Object Ownership** は別概念：

- Instance Master = `VRCPlayerApi.isMaster` で判定、インスタンス全体の管理プレイヤー
- Object Ownership = `Networking.IsOwner(gameObject)` で判定、オブジェクトごと

### 公式の警告：**Master に依存しない**

> "Don't rely on master if you can avoid it!"
> "a master player might become unresponsive"

- Master は退出などで変わる
- Master が通信不能になる可能性がある
- **制御ロジックは `Networking.IsOwner` を優先**
- Master 依存は初期化時の「最初に入った人がやる一度きり処理」のような限定用途のみ

## 典型パターン：Interact でオーナー取得→変更

```csharp
public override void Interact()
{
    if (!Networking.IsOwner(gameObject))
        Networking.SetOwner(Networking.LocalPlayer, gameObject);

    _value++;
    RequestSerialization();
}
```

## Best Practice

- プレイヤーが変更する前にオーナー確認 or 取得
- late joiner や退出を考慮した設計
- Master 判定より Ownership 判定を優先
- 頻繁なオーナー移譲はレイテンシとデシンクの原因になるため避ける
