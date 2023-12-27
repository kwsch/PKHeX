# Contributing to PKHeX

Thank you for taking the time to contribute to PKHeX's development. These are mostly guidelines, not rules. Use your best judgment, and feel free to propose changes to this document in a pull request.

PKHeX is a community driven project based off of many years of shared information and tools. Even if you can't write code, community documentation of in-game behaviors can go a long way to help implement new features.

PKHeX is written in C#, and tries to keep up to date with the latest standard language style.

### Issues

When submitting an issue, please use an issue template and ensure you are filing things correctly. If you are unsure, you can always create a thread on our forums instead (we monitor both locations)!

Have a question? Please contact us on the forums (or our IRC/Discord, link on forums) instead. Please be patient when expecting a response; this is free software and we aren't available at all times.

### Pull Requests

When submitting a pull request, please try to have everything ready for merging and passing all tests. Draft pull requests may be rejected as we value having a minimal amount of open issues/pull requests!

Please make sure your code is maintainable; provide comments and xmldoc when appropriate. If you are coding new GUI features, ensure any non-GUI logic is separate from the GUI specific logic; separating concerns is important for maintainability and portability!

If you are providing something that interacts with game data, try to model things to match the way the game interacts with the data. When done this way, it is easiest for others to replicate your research and expand upon your improvements.

### Formatting

Spaces instead of tabs. Please follow the standard C# formatting rules provided by Visual Studio.
