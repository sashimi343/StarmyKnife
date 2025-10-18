# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)

## [Unreleased]

## [1.1.0.0] - 2025-10-18

### Added

* Counter: tool for counting characters, words, lines, etc. in the input string
* New builtin converter:
  * Convert from/to quoted-printable encoding
  * Print input as hexdump format
  * Sort input items
  * Split and join by specified delimiters
  * Unique items extraction
  * Remove null (0x00) characters from input

## [1.0.2.0] - 2025-10-14

### Added

* New builtin converters
  * Narrow to wide / wide to narrow conversion
  * Unicode normalization (NFC, NFD, NFKC, NFKD)
  * Japanese hiragana to katakana / katakana to hiragana conversion

### Changed

* Synchronised scrolling of input and output rows in ListConverter

## [1.0.1.0] - 2025-09-30

### Added

* Settings of FontFamily and FontSize for input/output controls
* Splash screen

## [1.0.0.0] -2025-08-28

### Changed

* Separate interface for plug-ins in a separate project

## [0.2.3.0] - 2025-08-28

### Added

* Some converters
  * To Lower/Upper case
  * Change case (PascalCase, camelCase, snake_case, kebab-case, UPPER_SNAKE_CASE)

### Changed

* *PathFinder search results history can be saved

### Fixed

* Fix bugs in converters

## [0.2.2.0] - 2025-06-16

### Added

* Clipboard integration
* Converter reordering function

## [0.2.1.0] - 2025-03-22

### Added

* Generator plugins: RandomTextGenerator

### Changed

* Improove a style of PluginParameterBox view
* PluginBoxes are no longer selected when focusing on the Converter tab in ListConverter

## [0.2.0.0] - 2024-12-08

### Added

* List Converter: multi-input-multi-output string conversion tool

### Changed

* Add JSONpath search support to XPathFinder

### Fixed

* Fix a bug that `replace text (simple substitution)` converter cannot replace to tabs or newlines

## [0.1.0.0] - 2024-11-30

### Added

* Chain Converter: multi-step tool for string conversion
* Generator: tool for generate string of specified pattern
* Pretty Validator: tool for syntax checking, prettify, minify of string
* XPath Finder: tool for perform XPath searches on XML string