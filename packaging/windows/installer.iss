; Inno Setup script for PKHeX-Avalonia.
;
; Built automatically by .github/workflows/release.yml on the windows-latest
; runner via chocolatey's `innosetup` package. Invoked as:
;
;   ISCC.exe /DMyAppVersion=<version> /DSourceDir=<publish dir> /DOutputDir=<out dir> installer.iss
;
; The three defines above are supplied by CI; fallback values below let the
; script also run locally (e.g. `iscc packaging\windows\installer.iss`)
; against a `publish` folder produced by `dotnet publish`.

#ifndef MyAppVersion
#define MyAppVersion "0.0.0"
#endif
#ifndef SourceDir
#define SourceDir "..\..\publish"
#endif
#ifndef OutputDir
#define OutputDir "."
#endif

#define MyAppName "PKHeX-Avalonia"
#define MyAppPublisher "Patrik Lleshaj"
#define MyAppURL "https://github.com/doctorllll/PKHeX"
#define MyAppExeName "PKHeX.Avalonia.exe"

[Setup]
; Fixed GUID so upgrades/uninstalls track the same product across versions.
AppId={{B6C9F1B4-7B7B-4B7A-9C2E-8B6C9C7B7B7B}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
UninstallDisplayName={#MyAppName}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir={#OutputDir}
OutputBaseFilename=PKHeX-Avalonia-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=..\..\PKHeX.Avalonia\Assets\Icons\icon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
