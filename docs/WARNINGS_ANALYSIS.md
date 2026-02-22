# ビルドワーニングの原因と対応

## 1. 原因の分類

### 1.1 Fody (MSBUILD warning FodyPackageReference)
- **原因**: `PropertyChanged.Fody` がビルド時のみ使うパッケージなのに、参照として公開されていた。
- **対応**: csproj で `PrivateAssets="All"` を付与済み。

### 1.2 CS0067 (The event 'X.PropertyChanged' is never used)
- **原因**: `INotifyPropertyChanged` を実装したクラスで、`PropertyChanged` イベントがコード上で直接購読されていない。WPF のデータバインディングや Fody が内部的に利用するため、静的解析では「未使用」と出る。
- **対応**: プロジェクトで `NoWarn` に CS0067 を追加済み。

### 1.3 Nullable 参照型 (CS86xx, CS87xx)
- **原因**: プロジェクトで `<Nullable>enable</Nullable>` のため、参照型の null の扱いが厳密になっている。一方で、従来の `= null` のオプション引数や、未初期化フィールド・戻り値の null が「非 null 型」として扱われて警告になる。
- **主なパターン**:
  - **CS8625**: `null` を非 null 型の引数に渡している → 引数を `T?` に変更、または `null!` で表明。
  - **CS8600/8601/8602/8603/8604**: 可能な null の代入・参照・戻り値・引数 → `?` 付き型や null チェックで解消。
  - **CS8618**: 非 null フィールドがコンストラクタ終了時に未代入 → `string?` にするか、`= null!` で初期化。
  - **CS8765/CS8767**: オーバーライド・インターフェース実装の nullability 不一致 → 基底・インターフェースに合わせて `?` を付与。
- **対応**: 代表的な箇所（Web.cs, App.Path.cs, Menu.xaml.cs, DLP.cs, YamlDescription.cs, Yaml.cs, Util.PropertyCopy.cs）を修正済み。残りは同様に `?` 付き型・null チェック・`null!` で対応可能。

### 1.4 その他
- **CS0162**: 到達不能コード (Yaml.cs など)。分岐や return の見直しで削除または修正。
- **CS0168**: 宣言したが未使用の変数 (`ex`, `e`)。`_` に変更するか削除。
- **CS0219**: 代入したが未使用の変数 (`r`)。削除するか実際に使用。
- **CS4014**: async メソッド内で await していない非同期呼び出し。`await` するか、意図的なら `_ = Task.Run(...)` などで明示。
- **CS8620**: ジェネリックの nullability の違い（例: `List<Subs?>` と `IList<Subs>`）。型定義を揃えるか、null を除外して渡す。

## 2. 修正済みファイル一覧

| ファイル | 内容 |
|----------|------|
| yt-dlp-gui.csproj | Fody に PrivateAssets="All"、NoWarn に CS0067 |
| Libs/Web.cs | Download のオプション引数に `?`、GetLastTag の戻り値を `Task<List<GitRelease>?>` に変更 |
| App/App.Path.cs | AppExe / AppPath / AppName を `string?` に変更 |
| Controls/Menu.xaml.cs | Create/Open の target、MenuDataItem の header/execute/HeaderTemplate/Action/Owner を nullable に、implicit operator で `null!` を返すよう変更 |
| Wrappers/DLP.cs | Exec の戻り値とコールバック引数を nullable に変更 |
| Libs/YamlDescription.cs | GetProperties の container、TypeOverride、Write の value を nullable に合わせて変更 |
| Libs/Yaml.cs | IYamlConfig._YAMLPATH を `string?`、Load で data の null チェック、DirectoryName に `!` を付与 |
| Libs/Util.PropertyCopy.cs | from を TParent? にし、先頭で null チェック |

## 3. 残作業の進め方

- 同じ CS86xx/CS87xx が出ている他ファイルも、「引数・戻り値・フィールドに `?` を付ける」「null チェックを入れる」「どうしても null を渡す/返す場合は `null!` で表明」のいずれかで解消できる。
- 未使用変数・未使用イベント・到達不能コードは、該当行を開いて削除またはロジック修正。
- 一括でワーニングを出さないようにしたい場合は、csproj の `NoWarn` に該当番号を追加する方法もある（非推奨: 本当のバグを見逃しやすくなる）。
