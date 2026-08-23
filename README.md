# Free Launcher Builder

A Windows WPF application that lets you **configure and generate a fully standalone game launcher** without writing a single line of code.

Instead of manually editing JSON configuration files, you fill in a graphical interface and the builder produces a **single portable EXE** that launches your game — configuration included.

## Features

- **Customizable launcher** — configure the window title, game name and display font.
- **Play button styling** — set the text, background, foreground and border colors, plus hover and pressed states (with a built-in color picker).
- **Background & icon support** — choose a background image and a launcher icon.
- **Game detection** — pick your game executable; the whole game folder is copied automatically.
- **Portable build** — all asset and game paths are converted to relative paths so the generated launcher works from any location.
- **Embedded configuration** — the `FreeLauncher.exe` template is extracted and the JSON configuration is injected directly into the generated EXE (`[FreeLauncher.exe][JSON][JSON length][marker]`).
- **Validation** — the minimum required fields are checked before generation.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (with WPF / Windows Forms support on Windows)
- Windows 10/11

## Build

```bash
dotnet restore
dotnet build -c Release
```

The project is configured to publish a **self-contained, single-file** executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Usage

1. Run the builder.
2. Fill in the launcher name, game name, and pick the game executable.
3. Optionally select a background image, icon, font and customize the Play button colors.
4. Click **Generate Launcher**, choose a destination, and the builder creates:
   - the standalone `Launcher.exe` (with embedded configuration),
   - an `Assets/` folder containing the background and icon,
   - the copied game folder next to it.

Everything is placed relative to the generated EXE, so the whole folder is **portable**: zip it, move it, share it.

## How the generated launcher works

- `Templates/FreeLauncher.exe` is the pre-built launcher runtime, embedded in this builder as a resource.
- When you generate a launcher, the builder:
  1. validates the input,
  2. copies the background and icon into `Assets/`,
  3. recursively copies the game folder,
  4. converts absolute paths to portable relative paths,
  5. extracts `FreeLauncher.exe`,
  6. appends the serialized configuration to the end of the EXE.

The resulting file layout:

```
Launcher.exe
Assets/
  background.png
  icon.ico
YourGame/
  YourGame.exe
  ...
```

## Project structure

```
FreeLauncherBuilder/
├── App.xaml / App.xaml.cs        # Application entry point
├── MainWindow.xaml               # Builder UI
├── MainWindow.xaml.cs            # Builder logic, config model & EXE generation
├── Templates/
│   └── FreeLauncher.exe          # Embedded launcher template (Git LFS)
└── FreeLauncherBuilder.csproj
```

## Configuration model

The generated JSON configuration (`LauncherConfig`) includes:

| Field | Description |
| --- | --- |
| `launcherName` | Text shown in the launcher title bar |
| `gameName` | Game name displayed in the launcher |
| `gameNameFontFamily` | Font used for the game name |
| `gameExecutable` | Relative path to the game executable |
| `backgroundPath` | Relative path to the background image |
| `iconPath` | Relative path to the launcher icon |
| `playButtonText` | Text inside the Play button |
| `playButtonBackground` / `Foreground` / `Border` | Normal button colors |
| `playButtonHoverBackground` / `Foreground` | Hover state colors |
| `playButtonPressedBackground` | Pressed state color |

## License

Personal project — no license specified.

---

🇫🇷 Read the documentation in French: [README.fr.md](README.fr.md)