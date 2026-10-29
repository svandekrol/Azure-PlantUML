# Contributing to Azure-PlantUML

Thanks for helping out! Bug reports, missing icons, and pull requests are all welcome.

## Reporting a problem or a missing icon

[Open an issue](https://github.com/plantuml-stdlib/Azure-PlantUML/issues/new). For a missing icon, include the service name, and a link to the icon if it is not in the [Microsoft Azure architecture icons](https://learn.microsoft.com/azure/architecture/icons/).

## How the repository is organized

| Path | What it is |
|---|---|
| `scripts/Config.yaml` | The list of services: which source icon becomes which PlantUML macro, by category |
| `scripts/` | The C# generator that turns SVG icons into sprites ([build docs](scripts/README.md)) |
| `source/official/` | The Microsoft icon pack. Not committed; download it as described in the [build docs](scripts/README.md) |
| `source/manual/` | Icons that are not in the Microsoft icon pack |
| `source/*.puml` | Hand-written common macros (`AzureCommon.puml`, `AzureC4Integration.puml`, …) copied into `dist/` |
| `dist/` | Generated output: sprites, PNGs, SVGs, `all.puml` per category, VS Code snippets |
| `AzureSymbols.md` | The table of all symbols, generated into `dist/table.md` |
| `samples/` | Example diagrams; CI renders all of them |

**Never edit files in `dist/` by hand.** Change `Config.yaml`, `source/`, or the generator, then regenerate.

## Adding or changing an icon

1. Find the icon in the Microsoft icon pack (`source/official/`). If it is not there, add an SVG to `source/manual/` as `<Name>_COLOR.svg`.
2. Add an entry to the right category in `scripts/Config.yaml`:

   ```yaml
   - Name: Web
     Services:
     - Source: Static Apps          # icon file name, without number prefix, dashes or .svg
       Target: AzureStaticWebApp    # macro, stereotype and file name
   ```

   `Source` is matched case-insensitively against `*-icon-service-<Source>.svg`, `<Source>_COLOR.svg` and `<Source>.svg`, with spaces turned into dashes. `source/official` is searched before `source/manual`.
3. Regenerate `dist/` and update `AzureSymbols.md` (see below).

### Naming rules

- **Never rename or remove an existing `Target`.** People's diagrams depend on it. When Microsoft renames a service, point the old entry at the new icon and add the new name as a second entry with the same `Source`:

  ```yaml
  - Source: Azure Active Directory
    Target: AzureActiveDirectory    # existing name, kept
  - Source: Azure Active Directory
    Target: EntraID                 # current name
  ```

- Use the singular, PascalCase, with an `Azure` prefix: `AzureContainerRegistry`, not `ContainerRegistries`. Intune services use `AzureIntune…`.
- Products that Microsoft no longer brands as Azure use their current brand instead: `Entra…`, `MicrosoftFoundry…`, `MicrosoftDefender…`.
- Each `Target` must be unique across all categories.

### Categories

Put each service in the category that matches what it does, following the folders of the Microsoft icon pack where they make sense. `General` is for generic, non-service icons only. Adding a new category is fine; it becomes a new folder in `dist/`.

### What we don't add

- Services that Microsoft has retired
- Microsoft-internal or unexplained icons from the icon pack

## Building and regenerating

The [build docs](scripts/README.md) cover the prerequisites (.NET 10, Java, PlantUML, Playwright) and settings. In short, from `scripts/`:

```bash
dotnet run -- --sourceFolderPath=../source --targetFolderPath=../dist --monochromeColorHex=#0072C6 --plantUmlPath=/path/to/plantuml.jar
```

The generator exits with a non-zero code when a service has no source icon or fails to generate, and lists those services at the end of the log.

Then replace the table in `AzureSymbols.md`, from the `Category | Macro (Name)` header line down, with the contents of `dist/table.md`.

Before opening a pull request, render a diagram that uses your new or changed macros, and check that the samples in `samples/` still render.

## Pull requests

- Fork the repository and work on a feature branch.
- Keep a pull request to one topic, for example "add these 5 services" or "fix the build on macOS". Large icon updates are easier to review when `Config.yaml` changes and the regenerated `dist/` are in separate commits.
- Commit the regenerated `dist/` and `AzureSymbols.md` together with the `Config.yaml` change that produced them.
- CI ([.github/workflows/build.yml](.github/workflows/build.yml)) builds the generator, downloads the icon pack, regenerates `dist/`, and renders every sample. It must pass before merging.
- Describe what changed and why. For icon changes, list the added macros and any renamed services.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
