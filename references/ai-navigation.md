# AI Navigation（NavMesh 連携）

ワールド内 NPC が NavMesh で賢く移動する仕組み。**Unity 2022 の AI Navigation パッケージ**を VRChat で有効化したもの。

## 対応機能

- **ランタイム NavMesh 生成**（以前は Editor のみ、VRChat クライアント内でも生成・更新可）
- **動的障害物**（ランタイムで障害物を追加・削除）
- **Off-mesh links**（ドア開閉、崖飛び越えなどの特殊移動）
- **複数の NavMeshSurface**（ワールド内で用途別に分ける）
- **Area Cost**（エリア別移動コスト）— ワールド入場時も保持される

## 未対応・制限

- **カスタム Agent Type 不可** — デフォルト Agent のみ。カスタムの radius / height / slope は in-client で取得できない
- 未対応メソッド：
  - `NavMeshLink.agentTypeID`
  - `NavMeshSurface.CollectObjects`
  - `NavMeshSurface.UpdateNavMesh()` （`AsyncOperation` を返すので Udon で使えない）

## 実装の参考

- **Unity の AI Navigation パッケージ公式ドキュメント**で概念を把握
- VRChat 固有の差分は上記の未対応項目のみ
