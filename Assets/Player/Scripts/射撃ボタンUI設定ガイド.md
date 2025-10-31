# 射撃ボタンUI設定ガイド

加速度センサーに加えて、UIボタンで銃を撃てるようにする設定方法です。

## 1. Canvasの確認

既にゲーム画面に `UICanvas` があるか確認してください。
- Hierarchy で `UICanvas` を探す
- なければ、GameObject → UI → Canvas で作成

## 2. 射撃ボタンの作成

### 2.1 ボタンオブジェクトの作成
1. Hierarchy で `UICanvas` を右クリック
2. UI → Button - TextMeshPro（または Button）を選択
3. 名前を「ShootButton」に変更

### 2.2 ボタンの位置とサイズ調整
1. `ShootButton` を選択
2. Rect Transform を調整：
   - **推奨位置**: 右下（親指で押しやすい位置）
   - **Anchors**: 右下に設定
     - Min: (1, 0)
     - Max: (1, 0)
   - **Pivot**: (1, 0)
   - **Pos X**: -20（右端から20px）
   - **Pos Y**: 20（下から20px）
   - **Width**: 100-150
   - **Height**: 100-150

### 2.3 ボタンの見た目をカスタマイズ（オプション）
1. `ShootButton` の Image コンポーネントで色や画像を変更
2. 子オブジェクトの `Text` で表示テキストを変更
   - 例: "🔫" や "FIRE" など

## 3. ShootButtonスクリプトのアタッチ

### 3.1 スクリプトを追加
1. `ShootButton` オブジェクトを選択
2. Inspector で `Add Component`
3. `ShootButton` スクリプトを検索して追加

### 3.2 スクリプトの設定
Inspector で以下を設定：

#### 射撃設定
- **Cooldown Time**: `0.5`（連射制限、秒単位）

#### エフェクト設定
- **Button Image**: 自動設定されます（ShootButtonのImageコンポーネント）
- **Normal Color**: 通常時の色（白など）
- **Pressed Color**: 押された時の色（少し暗めの白など）
- **Cooldown Color**: クールダウン中の色（半透明のグレーなど）

#### オーディオ
- **Gun Shot Sound Source**: AudioSourceコンポーネント（下記で作成）
- **Gun Shot Sound Clip**: 銃声のオーディオファイル

### 3.3 AudioSourceの設定
射撃音を鳴らすため：

#### 既存のAudioSourceを使う場合
1. シーン内の既存AudioSource（`PushGestureHandler`と同じもの）を参照
2. `ShootButton`の `Gun Shot Sound Source` にドラッグ＆ドロップ

#### 新しくAudioSourceを作る場合
1. `ShootButton` に `Add Component` → `Audio Source`
2. Audio Source の設定：
   - **Play On Awake**: OFF
   - **Loop**: OFF
   - **Spatial Blend**: 0（2D音声）

## 4. 既存のButtonコンポーネントを無効化（重要）

`ShootButton`には2つのボタン機能が重複するのを防ぐため：

1. `ShootButton`の **Buttonコンポーネント**（Unity標準）を探す
2. `Interactable` のチェックを**外す**（または削除）
   - `ShootButton`スクリプトが `IPointerDownHandler` で直接タッチを処理するため

**または**、Buttonコンポーネントの `OnClick()` イベントを使う方法：
1. Buttonコンポーネントの `OnClick()` イベントリストの `+` をクリック
2. `ShootButton`オブジェクトをドラッグ＆ドロップ
3. 関数を `ShootButton.OnShootButtonClick()` に設定

## 5. 加速度センサーとボタンの併用設定

### 5.1 両方使う場合（デフォルト）
- `PushGestureHandler` の `Enable Gesture` を **ON** のまま
- 加速度センサーとUIボタンの両方で射撃可能

### 5.2 ボタンのみ使う場合
1. `PushGestureHandler` オブジェクトを探す（MainCameraなどにアタッチされている）
2. Inspector で `Enable Gesture` のチェックを **OFF**
3. これで加速度センサーは無効、UIボタンのみで射撃

## 6. レイアウト例

```
UICanvas
├── JoyStickBackground （左下：移動用）
│   └── JoyStick
└── ShootButton （右下：射撃用）
```

ジョイスティックとボタンの配置例：
- 左下: 移動用ジョイスティック
- 右下: 射撃ボタン

## 7. テスト

### プレイモードで確認
1. Unityエディタでプレイモードに入る
2. マウスで射撃ボタンをクリック
3. 確認事項：
   - ✓ 弾が発射される
   - ✓ 射撃アニメーションが再生される
   - ✓ 銃声が鳴る
   - ✓ ボタンの色が変わる（押した時/クールダウン中）
   - ✓ 連射制限が効いている（クールダウン中は撃てない）

### モバイルデバイスで確認
1. ビルドしてモバイルデバイスにインストール
2. 右下のボタンをタップして射撃
3. 加速度センサー（有効の場合）とボタンの両方をテスト

## 8. カスタマイズオプション

### ボタンの画像を変更
1. 射撃ボタン用の画像（PNG、透過背景推奨）を用意
2. Unityの `Assets` フォルダにインポート
3. Texture Type を `Sprite (2D and UI)` に設定
4. `ShootButton` の Image コンポーネントの `Source Image` に設定

### 連射速度の調整
- `ShootButton` の `Cooldown Time` を変更
  - 小さい値（0.2-0.3）: 速い連射
  - 大きい値（1.0-2.0）: ゆっくり連射

### ボタンサイズの調整
- スマートフォン向け: 100-120px
- タブレット向け: 150-200px
- 親指で押しやすいサイズに調整

### ボタンの配置バリエーション
- **右下**: 一般的（右利き用）
- **左下**: ジョイスティックの反対側
- **画面中央下**: 両手持ち用
- **複数配置**: 左右両方に配置して左右どちらでも撃てるように

## トラブルシューティング

### ボタンを押しても反応しない
- `EventSystem` がシーンに存在するか確認
  - なければ、GameObject → UI → Event System で作成
- Canvas の Render Mode を確認（Screen Space - Overlay 推奨）
- `ShootButton` が Canvas の子オブジェクトか確認

### 弾が発射されない
- `PlayerMovement.Local` が正しく設定されているか確認
- プレイヤーに `NetworkedBulletSpawner` または `BulletSpawner` がアタッチされているか確認
- コンソールでエラーメッセージを確認

### 音が鳴らない
- `Gun Shot Sound Source` と `Gun Shot Sound Clip` が設定されているか確認
- AudioSource の Volume が 0 になっていないか確認
- AudioClipが正しくインポートされているか確認

### ボタンが重なって押せない
- Hierarchy でボタンの順序を確認（下にあるものが手前に表示される）
- Canvas の Sort Order を確認
- Raycast Target（Image コンポーネント）がONになっているか確認
