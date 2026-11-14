# Starmy Knife

StarmyKnife is a utility that performs various operations on strings (conversion, generation, validation, etc.).
The software also supports extensions through plugins to add your own functionality.

## Installation

No installation is required.
Download the latest zip file from [Releases](https://github.com/sashimi343/StarmyKnife/releases), extract it to any folder and launch `StarmyKnife.exe`.

* `StarmyKnife-standalone-x64-x.x.x.x.zip`: Self-contained package. No additional runtime is required.
* `StarmyKnife-x.x.x.x.zip`: Framework-dependent package. Requires [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).

## Features / Tools

### ChainConverter

Apply a series of conversion processes (Base64, URL encoding, escaping, etc.) to a single input string,
and outputs the result of conversion.

### Generator

Batch generation of the required number of strings of the specified type, such as GUIDs, random numbers, etc.

### PrettyValidator

Performs syntax validation, prettify and minify processing based on a specified grammar (JSON, XML, etc.) on strings.

### *PathFinder

Perform string searches using XPath or JSON paths.

### ListConverter

Apply a ChainConverter-like string conversion process to multiple strings at once.

### Counter

Count the number of characters, words, lines, etc. in the input string.

## Lisence

This software is released under MIT License, see the [LICENSE.txt](LICENSE.txt) for details.