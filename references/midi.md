# MIDI in Udon

ワールドから MIDI デバイス・ファイルを扱う。リアルタイム演奏連動と、プレイバック同期の 2 種。

## MIDI イベント（共通）

```csharp
public override void MidiNoteOn(int channel, int number, int velocity) { }
public override void MidiNoteOff(int channel, int number, int velocity) { }
public override void MidiControlChange(int channel, int number, int value) { }
```

| 引数 | 範囲 |
|------|------|
| channel | 0-15 |
| number（note / control） | 0-127 |
| velocity / value | 0-127 |

Realtime と Playback で同じ event を使う（ソースが違うだけ）。

## Realtime MIDI

### `VRC Midi Listener` コンポーネント

```
GameObject
├── VRC Midi Listener（受信イベントの選択、対象 UdonBehaviour 指定）
```

- Listener が受け取るイベント種別はデフォルト無効、明示的に ON にする
- 対象 UdonBehaviour を指定して、そこでイベントを受ける

### VRCMidiHandler

シーン起動時に**自動生成**される MIDI ドライバ接続管理用 GameObject。**自分で追加しないこと**（自動追加/削除される）。

### デバイス選択

- **Editor**：VRChat SDK 設定で MIDI デバイスを選択（プロジェクト間で永続）
- **Runtime**：自動で最初の利用可能デバイスを開く。複数デバイスは `--midi=devicename` コマンドライン引数（部分一致、大文字小文字無視）

## MIDI Playback

### 必要なアセット

- `.mid`（MIDI ファイル）
- 対応する音声ファイル（`.aif` / `.wav` / `.mp3` / `.ogg`）

MIDI ファイルの AudioClip プロパティに音声を割り当てる。**BPM 設定が不正だと同期がずれる**（最頻出の不具合原因）。

### `VRCMidiPlayer` コンポーネント

- MIDI ファイルと AudioSource を参照
- 対象 UdonBehaviour に Note On / Off を送信

### メソッドと機能

- **`Play()`** / **`Stop()`**：MIDI と Audio を同時制御
- **`Time` プロパティ**：再生位置の get/set（両者同期）
- **Scene View デバッグ**：編集中に note data を可視化

### データ構造

| クラス | 内容 |
|-------|------|
| `MidiData` | 全 track と BPM |
| `MidiTrack` | note block 群 + note/velocity の min/max |
| `MidiBlock` | 個別 note（開始時刻、velocity、channel、duration） |

## テストワールド

公式サンプル実装：**Udon Midi Test**（`wrld_f8bc6485-dcdf-4646-89d8-14e4772561ee`）。Note On/Off と Control Change の 3 種イベント表示の参考。
