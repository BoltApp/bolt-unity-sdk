# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Roadmap
- Demo scene with Bolt quick start guide
- Serverless quick start (no game server needed)

## [0.1.1] - 2025-09-23
### Fixed
- Add missing dependency to package.json

### Removed
- Got rid of the cap of payment links stored in history

## [0.1.0] - 2025-09-23
### Fixed
- Improved handling of payment links history, only tracks pending payment links. Resolving a link now removes it from SDK history (keeps memory usage light)
- Uses Newton soft to deal with dictionaries and improves serialization
- Keeps payment links in memory to minimize I/O only to app load and checkout link open

### Removed
- Got rid of the cap of payment links stored in history

## [0.0.9] - 2025-09-22
### Added
- Experimental change to introduce a cap to the number of payment links stored in history

## [0.0.8] - 2025-09-19
### Fixed
- Fix an issue where user was always initialized without a created at date
- Improved user ToString formatting
- Added a missing var in the sample code snippets

## [0.0.7] - 2025-09-18
### Fixed
- Fix compile issue in PaymentLinks

## [0.0.6] - 2025-09-18

### Fixed
- Resolved an issue where payment links can be initialized with a null date

## [0.0.5] - 2025-08-03

### Fixed
- Introduced payment links to simplify handling of transaction statuses
- Fixed serialization issues

## [0.0.4] - 2025-07-30

### Fixed
- Fixed compilation issues with Immutable type

## [0.0.3] - 2025-07-28

### Added
- Bolt user object handling
- Query fields when opening web link

### Fixed
- Improved handling of url utils

## [0.0.2] - 2025-07-23

### Added
- Meta files to the project

### Fixed
- Install instructions with support for manifest.json import

## [0.0.1] - 2025-07-22

### Added
- Initial beta release of Bolt Unity SDK
- Basic integration support for Bolt Charge payments
- Web link implementation for payment flows
- Sample integration code