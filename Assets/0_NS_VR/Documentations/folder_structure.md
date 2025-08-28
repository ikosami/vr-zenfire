Assets/
  ├── 0_NS_VR/  # VRテンプレートの主要機能
  │   ├── Common/  # 共通ユーティリティと拡張機能
  │   │   ├── CoroutineRunner.cs  # コルーチン実行管理
  │   │   ├── DictionaryUtils.cs  # 辞書操作ユーティリティ
  │   │   ├── ListExtensions.cs  # リスト拡張機能
  │   │   └── SingletonMonoBehavior.cs  # シングルトンベースクラス
  │   ├── Core/  # コアシステムと基本機能
  │   │   ├── Debug/  # デバッグツールとプレハブ
  │   │   ├── Haptics/  # 触覚フィードバックコア機能
  │   │   ├── Input/  # 入力処理と初期化
  │   │   ├── Interaction/  # VRインタラクションシステム
  │   │   ├── Movement/  # プレイヤーの移動とロコモーション
  │   │   ├── Performance/  # パフォーマンス最適化
  │   │   ├── Physics/  # 物理演算システム
  │   │   ├── References/  # 参照管理システム
  │   │   ├── SceneManagement/  # シーン管理システム
  │   │   └── Services/  # 共有サービス管理
  │   ├── Documentations/  # プロジェクトドキュメント
  │   │   ├── architecture.png  # システムアーキテクチャ図
  │   │   ├── architecture.puml  # アーキテクチャ図のPlantUMLソース
  │   │   ├── CreateSpecifications.cs  # 仕様生成スクリプト
  │   │   ├── Specifications.als  # システム仕様定義
  │   │   └── folder_structure.md  # ディレクトリ構造の説明
  │   ├── EventSystem/  # イベント管理システム
  │   ├── Hand/  # ハンドトラッキングとポーズ管理
  │   │   ├── Editor/  # ハンドポーズエディタツール
  │   │   └── Poses/  # 事前定義されたハンドポーズ設定
  │   ├── Haptics/  # 触覚フィードバックシステム
  │   │   ├── Assets/  # 触覚設定アセット
  │   │   └── Scripts/  # 触覚制御スクリプト
  │   ├── People/  # キャラクターとNPCシステム
  │   │   ├── Emotable.cs  # 感情表現システム
  │   │   └── LookAtPlayer.cs  # プレイヤー注視システム
  │   ├── Decal/  # デカール（ステッカー）システム
  │   │   └── DecalReferences.cs  # デカール参照管理
  │   ├── Rendering/  # VR特有のレンダリング
  │   │   └── Shaders/  # カスタムVRシェーダー
  │   ├── Sound/  # オーディオ管理システム
  │   │   ├── Scripts/  # サウンド制御スクリプト
  │   │   ├── AudioClips/  # 音声ファイル
  │   │   └── Mixers/  # オーディオミキサー設定
  │   ├── Template/  # テンプレートシーンとプレハブ
  │   └── UI/  # VRユーザーインターフェース
  │       └── Prefabs/  # UIプレハブコンポーネント
  ├── VRTemplateAssets/  # 追加のVRアセットとリソース
  │   ├── Materials/  # VR用マテリアル
  │   ├── Models/  # VR用3Dモデル
  │   ├── Prefabs/  # 再利用可能なVRプレハブ
  │   ├── Scripts/  # VR動作スクリプト
  │   └── Sprites/  # UIと視覚アセット
  ├── XR/  # XRプラットフォーム設定
  │   ├── Loaders/  # VRプラットフォームローダー
  │   ├── Settings/  # XR設定と構成
  │   └── Resources/  # XR専用リソース
  └── XRI/  # XRインタラクションツールキット設定
      └── Settings/  # XRインタラクション設定
