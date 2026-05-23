# Window Minimizer (Logitech G HUB Workaround)

A lightweight, background Windows system tray application that minimizes the currently active window when a specific key (default is `F24`) is pressed.

This project was built as a workaround for Logitech G HUB, which removed the native "Minimize Window" command that existed in Logitech Options+. By binding a physical mouse button to `F24` in G HUB, this application catches the keystroke at the OS level and minimizes the active window.

## Features
* **Zero UI:** Runs completely in the background.
* **System Tray Integration:** Sits quietly next to your clock. Right-click to exit.
* **Low-Level Hook:** Intercepts the keystroke globally, regardless of which app is active.
* **Single Executable:** Compiles to a single, standalone `.exe` file.

## How to Build
A PowerShell build script is included. It automatically generates the application icon and compiles the project using the .NET 8 SDK.

You can download the .NET 8 SDK from https://dotnet.microsoft.com/en-us/download/dotnet/8.0

1. Open PowerShell and navigate to the project folder.
2. Run `.\build.ps1`
3. The compiled executable will be located in the `bin\Release\net8.0-windows\win-x64\publish\` directory.

## How to Use
1. Open Logitech G HUB (or your preferred macro software).
2. Bind your desired mouse button to the `F24` keystroke.
3. Run `WindowMinimizerTray.exe`.
4. (Optional) Press `Win + R`, type `shell:startup`, and place a shortcut to the `.exe` in this folder so it runs automatically when Windows starts.
