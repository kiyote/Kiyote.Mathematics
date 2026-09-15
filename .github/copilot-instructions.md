# Copilot Instructions

## Project Guidelines
- Prefer IDE-aware tools (the editor's edit/build/diagnostics tooling) over terminal commands when working in this workspace; only use the terminal for operations the IDE tooling cannot perform.
- Avoid using run_command_in_terminal for file rename/move operations (e.g., PowerShell Rename-Item/Move-Item), since these can hang waiting on an interactive parameter prompt. Prefer using the create_file and remove_file tools to recreate/delete files instead of shelling out to rename/move them.

## Code Style
- Prefer explicit interface implementations in C# classes. When a class needs to reuse logic across explicitly implemented members, factor that logic into a private method named `Do<MethodName>` and have the explicit implementations call it.
- Prefer C# collection expressions (e.g. [a, b]) over array creation syntax such as new[] { a, b }.
