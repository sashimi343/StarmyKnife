# StarmyKnife

StarmyKnifeは文字列に対する様々な処理（変換、生成、バリデーション等）を行うユーティリティです。
また、本ソフトウェアはプラグイン機能に対応しており、独自の処理を追加することも可能です。

## インストール方法

インストールは不要です。
[Releases](https://github.com/sashimi343/StarmyKnife/releases)から最新のZIPファイルをダウンロードして任意のフォルダに解凍した後、
`StarmyKnife.exe`を実行してください。

## 機能一覧

### ChainConverter

文字列に対して複数の変換処理（Base64、URLエンコード、各種エスケープ等）を適用し、その結果を出力します。

### Generator

GUID、乱数等、指定した条件に基づく文字列を必要な数だけ一括生成します。

### PrettyValidator

JSON、XML等の文字列に対して、文法チェック（バリデーション）、整形（prettify）およびminifyを行います。

### *PathFinder

XML文字列またはJSON文字列に対して、XPath/JSONPathによる文字列検索を行います。

### ListConverter

複数の入力文字列に対して、ChainConverterと同様の文字列変換を行います。

## ライセンス

本ソフトウェアはMITライセンスのもとでリリースされています。
詳細は[LICENSE.txt](LICENSE.txt)を参照してください。