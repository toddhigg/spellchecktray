# Spell Check

Spell Check sits in the Windows system tray. Press Ctrl+Space, type a word or sentence, and misspellings get a red underline. Right-click a marked word for suggestions, the same way Notepad does.

The installer is a single EXE: `spellchecktray.exe`.

## Install

Spell Check needs 64-bit Windows 10 version 1809 or later, including Windows 11.

1. Run `spellchecktray.exe`.
2. Approve the administrator prompt. Setup installs Spell Check and the Windows App Runtime it needs.
3. Leave **Start Spell Check** checked if you want it running when setup finishes. A Start menu shortcut is created. A desktop shortcut is optional.

The app installs to `C:\Program Files\Spell Check`. The first time it runs, it turns on **Launch at startup**.

To remove it, open **Settings > Apps** and uninstall **Spell Check**. That also removes the startup entry.

## Use

The dictionary icon in the tray is Spell Check.

- Press **Ctrl+Space** to open the checker on the monitor under the pointer. Press it again to hide the window.
- Type a word or sentence. A misspelled word is underlined in red.
- Right-click an underlined word for spelling suggestions. You can also ignore the word or add it to the dictionary.
- Press **Escape**, or the window’s close button, to hide the checker. The text stays until you quit the app.
- Left-click the tray icon to open the same window.
- Right-click the tray icon to turn **Launch at startup** on or off, or to exit.

If Ctrl+Space is already used by another program, the tray icon shows a notice. Left-click the dictionary icon to open the checker.

## Build the installer

The setup program is an EXE produced by Inno Setup from `installer/SpellCheck.iss`.

From this folder, with the .NET 8 SDK and [Inno Setup 6](https://jrsoftware.org/isinfo.php):

1. Publish the app:

   ```
   dotnet publish SpellTray.csproj -c Release -r win-x64 --self-contained true -o installer-staging\app
   ```

2. Download the Windows App Runtime 1.6.9 x64 installer and save it as `installer-staging\WindowsAppRuntimeInstall-x64.exe`. The matching download is the 1.6.9 (1.6.250602001) x64 installer from the [Windows App SDK downloads](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads).

3. Compile the script with `ISCC.exe`:

   ```
   ISCC.exe installer\SpellCheck.iss
   ```

The script writes `spellchecktray.exe`. Give that file to someone else. They do not need the source, the .NET SDK, or Inno Setup.
