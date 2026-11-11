# Template project for Blazor WebAssembly

[English](README.md) | 日本語

[Template](https://usausa.github.io/template/) project for Blazor WebAssembly.

# ✅ 実装

| 分類 | 機能 |
| --- | --- |
| UI | Blazor WebAssembly(ASP.NET Core でホスト) / Fluent UI Blazor / データグリッド(サーバーページング・並べ替え) / ダイアログ(編集・確認) / トースト通知 / エラー境界 |
| API | Minimal API(`/api`) / OpenAPI(Swagger UI・ReDoc) / API クライアント生成(NSwag) / 共有コントラクト / 検証 / ProblemDetails |
| 認証 | JWT / リフレッシュトークン(ローテーション) / 401 時の自動更新 / ロールポリシー |
| データ | SQLite(Smart.Data.Accessor) / ページング・並べ替え・検索 / ファイル保管 API |
| HTTP | 圧縮(Brotli・Gzip) / セキュリティヘッダー / 転送ヘッダー |
| 監視 | Serilog / HTTP ログ / W3C ログ / SQL プロファイラー(MiniDataProfiler) / OpenTelemetry(OTLP・Prometheus) / ヘルスチェック |
| 運用 | 機能フラグ / Windows サービス / systemd / Aspire AppHost |
| テスト | 単体(xUnit v3・bUnit) / 結合(WebApplicationFactory) / E2E(Playwright) |

# 📚 ドキュメント

| ドキュメント | 内容 |
| --- | --- |
| [e2e.md](docs/e2e.md) | E2E テスト(Playwright) |

# 🔗 関連プロジェクト

| プロジェクト | 説明 |
| --- | --- |
| [template-web-api](https://github.com/usausa/template-web-api) | Web API のテンプレート(Minimal API) |
| [template-web-fullstack](https://github.com/usausa/template-web-fullstack) | フルスタック構成のテンプレート |
| [template-web-mvc](https://github.com/usausa/template-web-mvc) | ASP.NET Core MVC のテンプレート(Identity・パスキー) |
| [template-blazor-server](https://github.com/usausa/template-blazor-server) | Blazor Server アプリのテンプレート |
| [template-blazor-wasm-oidc](https://github.com/usausa/template-blazor-wasm-oidc) | OIDC を使う Blazor WebAssembly のテンプレート |
| [template-grpc-server](https://github.com/usausa/template-grpc-server) | gRPC サーバーのテンプレート |

# 📦 自作ライブラリ

| ライブラリ | 説明 |
| --- | --- |
| [Usa.Smart.Blazor](https://github.com/usausa/Smart-Net-Blazor) | Blazor ヘルパー |
| [Usa.Smart.Data.Accessor](https://github.com/usausa/Smart-Net-Data-Accessor) | データアクセス |
| [Usa.Smart.Mapper](https://github.com/usausa/Smart-Net-Mapper) | オブジェクトマッパー |
| [MiniDataProfiler](https://github.com/usausa/mini-data-profiler) | SQL プロファイラー |
| [BunnyTail.ServiceRegistration](https://github.com/usausa/bunnytail-service-registration) | サービス登録 |
