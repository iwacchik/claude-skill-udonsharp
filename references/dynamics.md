# VRC Dynamics（PhysBones / Contacts / VRC Constraints をワールドで使う）

SDK 3.10.0 でワールドに開放されたアバター/ワールド共通コンポーネント群。ワールドでは Animator でなく **Udon から操作する**のが基本。

```csharp
using VRC.SDK3.Dynamics.PhysBone.Components;    // VRCPhysBone / VRCPhysBoneCollider
using VRC.SDK3.Dynamics.Contact.Components;     // VRCContactSender / VRCContactReceiver
using VRC.SDK3.Dynamics.Constraint.Components;  // VRC○○Constraint
using VRC.Dynamics;                             // ContactEnterInfo 等のサポート型
```

共通原則：**プロパティをスクリプトから変更したら `ApplyConfigurationChanges()` を呼ぶまで反映されない**。変更は高コストなので、まとめて変更 → 1 回呼び出しにする。

## VRCPhysBone

### ワールドでの制約

- 1 コンポーネントで最大 **256 トランスフォーム**（ルート含む）。128 超なら分割検討（マルチスレッドで分散される）
- 衝突検出のバウンディングボックスは最大 **10×10×10 m**（範囲外は掴めない）
- ワールドのグローバルコライダー数は**無制限**（アバターは 4 個）
- Quest のアバター向けハード制限は受けないが、モバイル対応ワールドでは自主的に抑える

### イベント（UdonSharpBehaviour で override）

```csharp
OnPhysBoneGrabbed(PhysBoneGrabbedInfo info)
OnPhysBoneReleased(PhysBoneReleasedInfo info)
OnPhysBonePosed(PhysBonePosedInfo info)
OnPhysBoneUnPosed(PhysBoneUnPosedInfo info)
```

### 読み取り専用プロパティ

| プロパティ | 型 | 内容 |
|-----------|-----|------|
| `IsGrabbed` | bool | 掴まれ中か |
| `IsPosed` | bool | ポーズ固定中か |
| `Angle` | float | 0–1、正規化された 180 度角 |
| `Squish` / `Stretch` | float | 0–1、最大圧縮 / 伸長への接近度 |

### 読み書きプロパティ（変更後は ApplyConfigurationChanges 必須）

`pull` `spring` `momentum` `stiffness` `gravity` `gravityFalloff` `immobileType` `limitType` `maxAngle` `maxPitch` `maxYaw` `rotation` `radius` `allowCollision` `stretchMotion` `maxStretch` `maxSquish` `allowGrabbing` `allowPosing` `grabMovement` `snapToHand` `isAnimated` `resetWhenDisabled` および `pullCurve` `stretchCurve` `squishCurve` 等のカーブ。

### メソッド

| メソッド | 内容 |
|---------|------|
| `ApplyConfigurationChanges()` | プロパティ変更のコミット（**必須**） |
| `ReleaseGrabs()` | ローカルプレイヤーの掴みを強制解除（ローカルのみ。全員に効かせるならネットワークイベント併用） |
| `ReleasePoses()` | 固定ポーズを解除（同上） |

## VRCPhysBoneCollider（SDK 3.10.4 で Udon から変更可能に）

| プロパティ | 型 |
|-----------|-----|
| `shapeType` / `radius` / `height` | 形状 |
| `position` | Vector3（ルートからのオフセット） |
| `rotation` | Quaternion（ルートからの回転オフセット） |

変更後は `ApplyConfigurationChanges()` を呼ぶ。

## Contacts（VRCContactSender / VRCContactReceiver）

### ワールドでの動作

- ワールドの Receiver は、タグが一致した Sender の進入/退出で **全 UdonBehaviour にイベントを発火**する
- `Content Types` 設定で「ワールド / アバター / アイテムのどの Contact と反応するか」をフィルタ

### 形状（SDK 3.10.4 でボックス追加）

| 形状 | パラメータ |
|------|-----------|
| Sphere | `Radius` |
| Capsule | `Radius` + `Height` |
| Box | `Size`（width / height / depth を独立指定） |

最大サイズ：半径 3 m、幅/高さ/奥行き 6 m（**スケール適用後**に評価）。

### イベント

```csharp
public override void OnContactEnter(ContactEnterInfo info) { }
public override void OnContactExit(ContactExitInfo info) { }
```

| Info プロパティ | 型 | 内容 |
|----------------|-----|------|
| `contactSender` / `contactReceiver` | Proxy | 相手側の情報（下記） |
| `enterVelocity` | Vector3 | 進入速度（Enter のみ） |
| `contactPoint` | Vector3 | 接触点（Enter のみ） |
| `matchingTags` | string[] | 一致したタグ |

### Proxy API（ContactSenderProxy / ContactReceiverProxy）

読み取り：`isValid` `player`（VRCPlayerApi）`usage` `position` `rotation` `scale`
変更（要 `ApplyConfigurationChanges()`）：`radius` `height` `size` `position` `rotation`
その他：`UpdateContentTypes(DynamicsUsageFlags)` / `UpdateCollisionTags(string[])` / Receiver のみ `CalculateProximity()`

### タグ

- Sender と Receiver が**最低 1 つ共通タグ**を持つと反応。カスタムタグ（自由文字列）可
- **上限 16 タグ**（超過分は無視される）
- ビルトインタグ：
  - **Body Parts**（ヒューマノイドアバター読み込み時に自動生成）：`Head` `Torso` `Hand(L/R)` `Foot(L/R)` `Finger(L/R)` `FingerIndex` 等の指別・手別バリエーション
  - **Object Traits**（相互運用のための推奨語彙、特別処理なし）：`Hot` `Cold` `Fire` `Water` `Wind` `Weapon` `Shield` `Damage(Blunt/Sharp)` `Projectile` `Consumable(Food/Drink)` `Brush` `Dye` 等

プレイヤーの体に反応する仕掛け（頭を撫でる、手を触れる等）は Body Parts タグの Receiver をワールドに置くだけで作れる。

## VRC Constraints

Unity Constraints のワールド利用は非推奨（重い）。**VRC Constraints を直接使う**。

### 6 種類

`VRCAimConstraint`（向き制御）/ `VRCLookAtConstraint`（Z 軸をソースへ）/ `VRCParentConstraint`（位置+回転）/ `VRCPositionConstraint` / `VRCRotationConstraint` / `VRCScaleConstraint`

### 共通 API

| メンバー | 型 | 内容 |
|---------|-----|------|
| `ApplyConfigurationChanges()` | void | 変更のコミット（**必須**） |
| `IsActive` | bool | 評価中か |
| `GlobalWeight` | float | 全体ウェイト（0–1） |
| `Locked` | bool | ロック（Play 中は常にロック扱い） |
| `Sources` | VRCConstraintSourceKeyableList | ソースリスト |
| `TargetTransform` | Transform | 影響を受ける Transform |
| `SolveInLocalSpace` / `FreezeToWorld` / `RebakeOffsetsWhenUnfrozen` | bool | 解決空間・固定制御 |
| `ActivateConstraint()` / `ZeroConstraint()` | void | オフセット保持で有効化 / オフセットゼロで有効化 |

At-Rest / Offset プロパティは型ごと（例：Position は `PositionAtRest` / `PositionOffset`）。`VRCParentConstraint` はソースごとに `ParentPositionOffset` / `ParentRotationOffset` を持つ。

Aim 専用：`AimAxis` `UpAxis` `WorldUpTransform` `WorldUpVector` `WorldUp`（`SceneUp` / `ObjectUp` / `ObjectRotationUp` / `Vector` / `None`）
LookAt 専用：`Roll` `WorldUpTransform` `UseUpTransform`

### ソース操作

`VRCConstraintSource` は**構造体**なので、書き換えたら要素に代入し直す：

```csharp
// ウェイト変更
for (int i = 0; i < constraint.Sources.Count; i++)
{
    VRCConstraintSource source = constraint.Sources[i];
    source.Weight = 0.5f;
    constraint.Sources[i] = source;   // 代入し直し必須
}

// 追加・削除
constraint.Sources.Add(new VRCConstraintSource(sourceTransform, 1.0f));
constraint.Sources.RemoveAt(i);

constraint.ApplyConfigurationChanges();
```

注意：アバターのアニメーターから操作できるのは**先頭 16 ソースのみ**（ワールド用途でも相互運用を考えるなら 16 以内推奨）。

## Allowlist との関係

ワールドで許可されている Dynamics コンポーネント：`VRCPhysBone` / `VRCPhysBoneCollider` / `VRCContactSender` / `VRCContactReceiver` / VRC Constraints 各種（`references/world-basics.md` の Allowlist も参照）。Dynamic Bone は廃止済みで PhysBone に移行する。
