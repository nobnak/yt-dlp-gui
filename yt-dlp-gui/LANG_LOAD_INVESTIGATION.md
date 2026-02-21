# AudioLang が "Lang" のまま・多言語ファイルが反映されない理由

## 原因

1. **読み込みパスが1つだけ**
   - `App.xaml.cs` で次のように **exe と同じフォルダの `yt-dlp-gui.lang` のみ** を参照している。
   - `var langPath = App.Path(App.Folders.root, App.AppName + ".lang");`
   - つまり `(exeのディレクトリ)\yt-dlp-gui.lang` だけが対象。

2. **`languages\` フォルダは参照されていない**
   - リポジトリの `languages\en-US\`, `languages\ja-JP\` 等の多言語ファイルは **実行時に一切参照されない**。
   - ビルド時に `languages\` を出力フォルダへコピーする設定もない。

3. **ファイルが無いとデフォルトのまま**
   - `Libs.Yaml` の `Open<T>(path)` は、ファイルが存在しない場合 `new T()` を返す。
   - そのため exe 横に `yt-dlp-gui.lang` が無いと `new Lang()` が使われ、**すべて既定値**（例: `AudioLang = "Lang"`）のままになる。

## 結論

- **多言語ファイルが反映されない理由**: アプリが `languages\{ロケール}\yt-dlp-gui.lang` を読んでおらず、かつ exe 横に `.lang` を置いていないため。
- **AudioLang が "Lang" のままの理由**: 上記のため Lang が常にデフォルトで初期化され、`AudioLang` の既定値 "Lang" が表示されている。

## 対応方針

- 実行時に `languages\{CurrentUICulture}\yt-dlp-gui.lang` 等を参照するようにする。
- ビルド時に `languages\` を出力フォルダへコピーし、そのパスから読み込む。
