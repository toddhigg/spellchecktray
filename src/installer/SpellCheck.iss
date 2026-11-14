; Spell Check Tray installer. Friends run spellchecktray.exe, which installs the tray app
; and the Windows App Runtime 1.6 it needs.

#define AppName "Spell Check Tray"
#define AppVersion "1.0.0"
#define AppExe "SpellTray.exe"
#define AppPublisher "Spell Check Tray"

[Setup]
AppId={{B7E4A1C2-6F0D-4E58-9A31-7C2D8F0E5B14}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\Spell Check Tray
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=..\..
OutputBaseFilename=spellchecktray
SetupIconFile=..\Assets\Dictionary.ico
UninstallDisplayIcon={app}\{#AppExe}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
CloseApplications=yes
RestartApplications=no
AppMutex=Local\SpellTray.SingleInstance

[Files]
Source: "..\installer-staging\app\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\installer-staging\WindowsAppRuntimeInstall-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "https://aka.ms/dotnet/8.0/windowsdesktop-runtime-win-x64.exe"; DestName: "windowsdesktop-runtime-win-x64.exe"; DestDir: "{tmp}"; Flags: external download ignoreversion deleteafterinstall; ExternalSize: 58431296

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExe}"; IconFilename: "{app}\{#AppExe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; IconFilename: "{app}\{#AppExe}"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{tmp}\windowsdesktop-runtime-win-x64.exe"; Parameters: "/install /quiet /norestart"; StatusMsg: "Installing .NET..."; Flags: waituntilterminated
Filename: "{tmp}\WindowsAppRuntimeInstall-x64.exe"; Parameters: "--quiet"; StatusMsg: "Installing required Windows components..."; Flags: waituntilterminated
Filename: "{app}\{#AppExe}"; Description: "Start {#AppName}"; Flags: nowait postinstall skipifsilent

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    RegDeleteValue(HKEY_CURRENT_USER, 'Software\Microsoft\Windows\CurrentVersion\Run', 'SpellTray');
    RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, 'Software\SpellTray');
  end;
end;
