; Inno Setup Script for KlayFix ERP v1.0.0
; Copyright (c) 2026 KlayFix Solutions
; To compile: iscc KlayFixSetup.iss

#define MyAppName "KlayFix ERP"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "KlayFix Solutions"
#define MyAppURL "https://klayfix.example.com"
#define MyAppExeName "KlayFix.Desktop.exe"

[Setup]
AppId={{B8C5E9A1-2D4F-4A8B-9C3E-7F1D2E5A6B0C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\
OutputBaseFilename=KlayFixSetup_v{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
DisableWelcomePage=no
DisableFinishedPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Files]
; Main application files (published Release build)
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Configuration file
Source: "..\Config\appsettings.json"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; Remove application data
Filename: "{cmd}"; Parameters: "/C rmdir /S /Q ""{localappdata}\KlayFix"""; Flags: runhidden

[Code]
function IsSQLServerRunning: Boolean;
var
  ResultCode: Integer;
begin
  Exec('sc', 'query MSSQLSERVER', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  if ResultCode = 0 then
    Result := True
  else
  begin
    Exec('sc', 'query MSSQL$SQLEXPRESS', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Result := ResultCode = 0;
  end;
end;

function InitializeSetup: Boolean;
begin
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    if not IsSQLServerRunning then
    begin
      MsgBox(
        'Warning: SQL Server Express does not appear to be running.'#13#13 +
        'Please ensure SQL Server is installed and running before launching KlayFix ERP.'#13#13 +
        'You can install SQL Server Express from: https://go.microsoft.com/fwlink/?linkid=866658',
        mbInformation, MB_OK);
    end;
  end;
end;
