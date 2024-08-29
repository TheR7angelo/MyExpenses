# v0.83.0		30/07/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=is%3Aissue+milestone%3A%22PowerToys+0.84%22
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=is%3Aissue+milestone%3A%22PowerToys+0.83%22
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.83.0/PowerToysUserSetup-0.83.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.83.0/PowerToysUserSetup-0.83.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.83.0/PowerToysSetup-0.83.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.83.0/PowerToysSetup-0.83.0-arm64.exe
>
> In the [v0.83 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.83.0-x64.exe][ptUserX64] | C78E24F21C611F2BD774D8460ADD4B9AC8519085CA1253941CB46129331AB8C8 |
> | Per user - ARM64     | [PowerToysUserSetup-0.83.0-arm64.exe][ptUserArm64] | BA1C16003D55587D523A41B960D4A03718123CA37577D5F2A75E151D7653E6D3 |
> | Machine wide - x64   | [PowerToysSetup-0.83.0-x64.exe][ptMachineX64] | 7EC435A10849187D21A383E56A69213C1FF110B7FECA65900D9319D2F8162F35 |
> | Machine wide - ARM64 | [PowerToysSetup-0.83.0-arm64.exe][ptMachineArm64] | 5E147424D1D12DFCA88DC4AA0657B7CC1F3B02812F1EBA3E564FAF691908D840 |
>
> ## Highlights
>
>  - Awake Quality of Life changes, including changing the tray icon to reflect the current mode. Thanks [@dend](https://github.com/dend)!
>  - Changes to general GPO policies and new policies for Mouse Without Borders. The names for some intune policy configuration sets might need to be updated as seen in https://github.com/MicrosoftDocs/windows-dev-docs/pull/5045/files . Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### General
>  - Reordered GPO policies, making it easier to find some policies. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Advanced Paste
>
>  - Fixed CSV parser to support double quotes and escape delimiters when pasting as JSON. Thanks [@GhostVaibhav](https://github.com/GhostVaibhav)!
>  - Improved double quote handling in the CSV parser when pasting as JSON. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Awake
>
>  - Different modes will now show different icons in the system tray. Thanks [@dend](https://github.com/dend), and [@niels9001](https://github.com/niels9001) for the icon design!
>  - Removed the dependency on Windows Forms and used native Win32 APIs instead for the tray icon. Thanks [@dend](https://github.com/dend) and [@BrianPeek](https://github.com/BrianPeek)!
>  - Fixed an issue where the UI would become non-responsive after selecting no time for the timed mode. Thanks [@dend](https://github.com/dend)!
>  - Refactored code for easier maintenance. Thanks [@dend](https://github.com/dend)!
>  - The tray icon will now be shown when running Awake standalone to signal mode. Thanks [@dend](https://github.com/dend)!
>  - The tray icon tooltip shows how much time is left on the timer. Thanks [@dend](https://github.com/dend)!
>  - Added DPI awareness to the tray icon context menu. Thanks [@dend](https://github.com/dend)!
>
> ### Color Picker
>
>  - Added support to using the mouse wheel to scroll through the color history. Thanks [@Fefedu973](https://github.com/Fefedu973)!
>
> ### File Explorer add-ons
>
>  - Allow copying from the right-click menu in Monaco and Markdown previewers.
>
> ### File Locksmith
>
>  - Fixed a crash when there were a big number of entries being shown by moving the opened files of a process to another dialog.
>
> ### Installer
>
>  - Fixed the path where DSC module files were installed for the user-scope installer. (This was a hotfix for 0.82)
>
> ### Mouse Without Borders
>
>  - Disabled non supported options in the old Mouse Without Borders UI. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Added new GPO policies to control the use of some features. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Peek
>
>  - Allow copying from the right-click menu in Dev files and Markdown previews.
>
> ### PowerToys Run
>
>  - Fixed a crash on Windows 11 build 22000. (This was a hotfix for 0.82)
>  - Blocked a transparency fix code from running on Windows 10, since it was causing graphical glitches. (This was a hotfix for 0.82)
>  - Accept speed abbreviations like kilometers per hour (kmph) in the Unit Converter plugin. Thanks [@GhostVaibhav](https://github.com/GhostVaibhav)!
>  - Added settings to configure behavior of the "First week of year" and "First day of week" calculations in the DateTime plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed wrong initial position of the PowerToys Run when switching between monitors with different dpi values.
>  - Started allowing interchangeable use of / and \ in the registry plugin paths.
>  - Added support to automatic sign-in after rebooting with the System plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Added suggested use example results to the Value Generator plugin. Thanks [@azlkiniue](https://github.com/azlkiniue)!
>
> ### Quick Accent
>
>  - Added support for the Bulgarian character set. Thanks [@octastylos-pseudodipteros](https://github.com/octastylos-pseudodipteros)!
>
> ### Runner
>
>  - Add code to handle release tags with an upper V when trying to detect new updates. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Settings
>
>  - Fixed the UI spacing in the "update available" card. Thanks [@Agnibaan](https://github.com/Agnibaan)!
>  - Fixed the information bars in the Mouse Without Borders settings page to hide when the module is disabled. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Improved consistency of the icons used in the Mouse Without Borders settings page. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Improved action keyword information bar padding in the PowerToys Run plugins section. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed a crash in the dashboard when Keyboard Manager Editor settings file became locked.
>
> ### Documentation
>
>  - Added the RDP plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@anthony81799](https://github.com/anthony81799)!
>  - Added the GitHubRepo and ProcessKiller plugins to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@8LWXpg](https://github.com/8LWXpg)!
>  - Fixed a typo in the 0.82.0 release notes in README. Thanks [@walex999](https://github.com/walex999)!
>
> ### Development
>
>  - Disabled FancyZone UI tests, to unblock PRs. We plan to bring them back in the future. (This was a hotfix for 0.82)
>  - Fixed an issue where flakiness in CI was causing the installer custom actions DLL from being signed. (This was a hotfix for 0.82)
>  - Upgraded the Microsoft.Windows.Compatibility dependency to 8.0.7.
>  - Upgraded the System.Text.Json dependency to 8.0.4.
>  - Upgraded the Microsoft.Data.Sqlite dependency to 8.0.7.
>  - Upgraded the MSBuildCache dependency to 0.1.283-preview. Thanks [@dfederm](https://github.com/dfederm)!
>  - Removed an unneeded /Zm compiler flag from Keyboard Manager Editor common build flags.
>  - Fixed the winget publish action to handle upper case V in the tag name. Thanks [@mdanish-kh](https://github.com/mdanish-kh)!
>  - Removed wildcard items from vcxproj files. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Removed the similar issues bot GitHub actions. Thanks [@craigloewen-msft](https://github.com/craigloewen-msft)!
>  - Fixed CODEOWNERS to better protect changes in some files.
>  - Switched machines being used in CI and pointed status badges in README to the new machines.
>  - Fixed NU1503 build warnings when building PowerToys. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Use the MSTest meta dependency for running the tests instead of the individual testing packages. Thanks [@stan-sz](https://github.com/stan-sz)!
>  - Added missing CppWinRT references.
>

___

# v0.82.1		12/07/2024

> This is a patch release to fix issues in v0.82.0 we deemed important for stability based on incoming rates. See [v0.82.0](https://github.com/microsoft/PowerToys/releases/tag/v0.82.0) for full release notes.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.1/PowerToysUserSetup-0.82.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.1/PowerToysUserSetup-0.82.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.1/PowerToysSetup-0.82.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.1/PowerToysSetup-0.82.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.82.1-x64.exe][ptUserX64] | B594C9A32125079186DCE776431E2DC77B896774D2AEE2ACF51BAB8791683485 |
> | Per user - ARM64     | [PowerToysUserSetup-0.82.1-arm64.exe][ptUserArm64] | 41C1D9C0E8FA7EFFCE6F605C92C143AE933F8C999A2933A4D9D1115B16F14F67 |
> | Machine wide - x64   | [PowerToysSetup-0.82.1-x64.exe][ptMachineX64] | B8FA7E7C8F88B69E070E234F561D32807634E2E9D782EDBB9DC35F3A454F2264 |
> | Machine wide - ARM64 | [PowerToysSetup-0.82.1-arm64.exe][ptMachineArm64] | 58F22306F22CF9878C6DDE6AC128388DF4DFF78B76165E38A695490E55B3C8C4 |
>
> ## Highlights
>
> - [#33504](https://github.com/microsoft/PowerToys/issues/33504) - Fixed a crash when starting PowerToys Run on Windows 11 SV1 (build number 22000).
> - [#33622](https://github.com/microsoft/PowerToys/issues/33622) - Fixed PowerToys Run appearing too bright on Windows 10.
> - [#33744](https://github.com/microsoft/PowerToys/issues/33744) - Fixed the installation location of DSC files on user-scoped installations, causing winget configure to not work correctly with PowerToys.
>

___

# v0.82.0		02/07/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=is%3Aissue+milestone%3A%22PowerToys+0.83%22
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=is%3Aissue+milestone%3A%22PowerToys+0.82%22
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.0/PowerToysUserSetup-0.82.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.0/PowerToysUserSetup-0.82.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.0/PowerToysSetup-0.82.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.82.0/PowerToysSetup-0.82.0-arm64.exe
>
> In the [v0.82 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.82.0-x64.exe][ptUserX64] | 295E2A4622C7E347D3E1BAEA6B36BECC328B566496678F1F87DE3F8A12A7F89A |
> | Per user - ARM64     | [PowerToysUserSetup-0.82.0-arm64.exe][ptUserArm64] | 55D25D068C6148F0A15C7806B9F813224ABA9C461943F42BB2A8B0E22D28240C |
> | Machine wide - x64   | [PowerToysSetup-0.82.0-x64.exe][ptMachineX64] | 01B59C00BB43C25BEFEF274755875717AB4DEAB57C0354AB96CF5B1DA4837C9A |
> | Machine wide - ARM64 | [PowerToysSetup-0.82.0-arm64.exe][ptMachineArm64] | 1F642B50962516127793C4D3556BF4FC24B9738BAC2F362CAA3BFF8B0C3AF97F |
>
> ## Highlights
>
>  - New feature added to PowerRename to allow using sequences of random characters and UUIDs when renaming files. Thanks [@jhirvioja](https://github.com/jhirvioja)!
>  - Improvements in the Paste As JSON feature to better handle other CSV delimiters and converting from ini files. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed UI issues that were reported after upgrading to WPF UI on Color Picker and PowerToys Run.
>  - Bug fixes and stability.
>
> ### Advanced Paste
>
>  - Fixed an issue causing external applications triggering Advanced Paste. (This was a hotfix for 0.81)
>  - Added a GPO rule to disallow using online models in Advanced Paste. (This was a hotfix for 0.81)
>  - Improved CSV delimiter handling and plain text parsing for the Paste as JSON feature. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Added support to convert from ini in the Paste as JSON feature. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed a memory leak caused by images not being properly cleaned out from clipboard history.
>  - Added an option to hide the UI when it loses focus. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved telemetry to get better data about token usage and if clipboard history is a popular feature. Thanks [@craigloewen-msft](https://github.com/craigloewen-msft)!
>
> ### Color Picker
>
>  - Fixed the opaque background corners in the picker that were introduced after the upgrade to WPFUI.
>
> ### Developer Files Preview (Monaco)
>
>  - Improved the syntax highlight for .gitignore files. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Checking for the sticky scroll option in code behind was being done twice. Removed one of the checks. Thanks [@downarowiczd](https://github.com/downarowiczd)!
>
> ### Environment Variables Editor
>
>  - Added clarity to the UI section tooltips. Thanks [@anson-poon](https://github.com/anson-poon)!
>
> ### File Explorer add-ons
>
>  - Fixed a crash when the preview handlers received a 64-bit handle from the OS. Thanks [@z4pf1sh](https://github.com/z4pf1sh)!
>  - Fixed a crash when trying to update window bounds and File Explorer already disposed the preview.
>
> ### Find My Mouse
>
>  - Added the option to have to use the Windows + Control keys to activate. Thanks [@Gentoli](https://github.com/Gentoli)!
>
> ### Hosts File Editor
>
>  - Improved spacing definitions in the UI so that hosts name are not hidden when resizing and icons are well aligned. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Changed the additional lines dialog to show the horizontal scrollbar instead of wrapping contents. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Improved the duplication check's logic to improve performance and take into account features that were introduced after it. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Installer
>
>  - Fixed the remaining install failures when the folders the DSC module is to be installed in isn't accessible by the WiX installer for user scope installations.
>  - Fixed an issue causing ARM 64 uninstall process to not correctly finding powershell 7 to run uninstall scripts.
>
> ### Peek
>
>  - Prevent activating Peek when the user is renaming a file. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added support to preview special folders like Recycle Bin and My PC instead of throwing an error.
>  - Fixed a crash caused by double releasing a COM object from the module interface.
>
> ### Power Rename
>
>  - Improved apostrophe character handling for the Capitalize and Titlecase renaming flags. Thanks [@anthonymonforte](https://github.com/anthonymonforte)!
>  - Added a feature to allow using sequences of random characters or UUIDs when renaming files. Thanks [@jhirvioja](https://github.com/jhirvioja)!
>
> ### PowerToys Run
>
>  - Improved the plugin descriptions for consistency in the UI. Thanks [@HydroH](https://github.com/HydroH)!
>  - Fixed UI scaling for different dpi scenarios.
>  - Fixed crash on a racing condition when updating UWP icon paths in the Program plugin. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed PowerToys Run hanging when trying to close an unresponsive window in the WindowWalker plugin. Thanks [@GhostVaibhav](https://github.com/GhostVaibhav)!
>  - Fixed the example in the UnitConverter description to reduce confusion with the inches abbreviation (now uses "to" instead of "in"). Thanks [@acekirkpatrick](https://github.com/acekirkpatrick)!
>  - Brought the acrylic background back and applied a proper fix to the titlebar accent showing through transparency.
>  - Fixed an issue causing the transparency from the UI disappearing after some time.
>
> ### Quick Accent
>
>  - Added support for the Crimean Tatar character set. Thanks [@cor-bee](https://github.com/cor-bee)!
>  - Added the Numero symbol and double acute accent character. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Added the International Phonetic Alphabet characters. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Fixed the character description center positioning. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Added feminine and masculine ordinal indicator characters to the Portuguese character set. Thanks [@octastylos-pseudodipteros](https://github.com/octastylos-pseudodipteros)!
>
> ### Screen Ruler
>
>  - Updated the default activation hotkey to Win+Control+Shift+M, in order to not conflict with the Windows shortcut that restores minimized windows (Win+Shift+M). Thanks [@nx-frost](https://github.com/nx-frost)!
>
> ### Settings
>
>  - Disabled the UI to enable/disable clipboard history in the Advanced Paste settings page when clipboard history is disabled by GPO in the system. (This was a hotfix for 0.81)
>  - Updated Advanced Paste's Settings and OOBE page to clarify that the AI use is optional and opt-in. (This was a hotfix for 0.81)
>  - Corrected a spelling fix in Advanced Paste's settings page. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Added localization support for the "Configure OpenAI Key" button in Advanced Paste's settings page. Thanks [@zetaloop](https://github.com/zetaloop)!
>  - Fixed extra GPO warnings being shown in Advanced Paste's settings page even if the module is disabled. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed a crash when a PowerToys Run plugin icon path is badly formed.
>  - Disabled the experimentation paths in code behind to improve performance, since there's no current experimentation going on.
>
> ### Documentation
>
>  - Adjusted the readme and release notes to clarify use of AI on Advanced Paste. (This was a hotfix for 0.81)
>  - Added the Edge Workspaces plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@quachpas](https://github.com/quachpas)!
>  - Removed the deprecated Guid plugin from PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@abduljawada](https://github.com/abduljawada)!
>  - Added the PowerHexInspector plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@NaroZeol](https://github.com/NaroZeol)!
>  - Fixed a broken link in the communication-with-modules.md file. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Updated COMMUNITY.md with missing and former members.
>
> ### Development
>
>  - Fixed ci UI tests to point to the correct Visual Studio vstest location after a Visual Studio upgrade. (This was a hotfix for 0.81)
>  - Updated System.Drawing.Common to 8.0.6 to fix CI builds after the .NET 8.0.6 upgrade was released.
>  - Removed an incorrect file reference to long removed documentation from the solution file. Thanks [@Kissaki](https://github.com/Kissaki)!
>  - Upgraded Windows App SDK to 1.5.3.
>  - Removed use of the BinaryFormatter API from Mouse Without Borders, which is expected to be deprecated in .NET 9.
>  - The user scope installer is now sent to the Microsoft store instead of the machine scope installer.
>  - Refactored Mouse Jump's internal code to allow for a future introduction of customizable appearance features. Thanks [@mikeclayton](https://github.com/mikeclayton)!
>  - Removed a noisy error from spell check ci runs.
>  - Improved the ci agent pool selection code.
>  - Updated Xamlstyler.console to 3.2404.2. Thanks [@Jvr2022](https://github.com/Jvr2022)!
>  - Updated UnitsNet to 5.50.0 Thanks [@Jvr2022](https://github.com/Jvr2022)!
>  - Replaced LPINPUT with std::vector of INPUT instances in Keyboard Manager internal code. Thanks [@masaru-iritani](https://github.com/masaru-iritani)!
>  - Improved the Microsoft Store submission ci action to use the proper cli and authentication.
>

___

# v0.81.1		28/05/2024

> This is a patch release to fix issues in v0.81.0 we deemed important for stability based on incoming rates. See [v0.81.0](https://github.com/microsoft/PowerToys/releases/tag/v0.81.0) for full release notes.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.1/PowerToysUserSetup-0.81.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.1/PowerToysUserSetup-0.81.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.1/PowerToysSetup-0.81.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.1/PowerToysSetup-0.81.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.81.1-x64.exe][ptUserX64] | 412E0CB2043E280A841DBF726764B273CCBFD9357E760CDC2DEE1BC0018C3B58 |
> | Per user - ARM64     | [PowerToysUserSetup-0.81.1-arm64.exe][ptUserArm64] | 73305AFF8B088DC623CCE5014CD8A7E65FF2818D4779829EA4EEBF6E5E8E9812 |
> | Machine wide - x64   | [PowerToysSetup-0.81.1-x64.exe][ptMachineX64] | 9FF86D15203FBCC85666921DE4E4EBBC66C6BA41814EFF4E3313BE81831B2183 |
> | Machine wide - ARM64 | [PowerToysSetup-0.81.1-arm64.exe][ptMachineArm64] | 75483B697947A3FC73E332C35687C487AE7E1103C8F564B342B8E5D578962359 |
>
> ## Highlights
>
> - [#32971](https://github.com/microsoft/PowerToys/issues/32971) - Fixed Advanced Paste was being triggered by some external software when some of the hotkeys were not set. Slack's tray icon was the most common example in the issues that were opened.
> - [#32947](https://github.com/microsoft/PowerToys/issues/32947) - Added a GPO rule for Advanced Paste to disable AI online models usage and prevent users from entering the API key.
> - [#33006](https://github.com/microsoft/PowerToys/issues/33006) - Improved descriptions on Advanced Paste Settings and OOBE pages to clarify that usage of AI is opt-in and that it can be used without AI.
> - [#32945](https://github.com/microsoft/PowerToys/issues/32945) - Fixed the Advanced Paste settings page UX showing that it could enable/disable clipboard history when that feature is not allowed by GPO.
> - [#30206](https://github.com/microsoft/PowerToys/issues/30206) - Fixed PowerToys Run showing the accent color on the title bar when that option is turned on in Windows Settings.
>

___

# v0.81.0		21/05/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=is%3Aissue+milestone%3A%22PowerToys+0.82%22
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F54
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.0/PowerToysUserSetup-0.81.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.0/PowerToysUserSetup-0.81.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.0/PowerToysSetup-0.81.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.81.0/PowerToysSetup-0.81.0-arm64.exe
>
> In the [v0.81 release cycle][github-current-release-work], we focused on stability, improvements and new features.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.81.0-x64.exe][ptUserX64] | E62B1EE81954A75355C04E7567B1C9AAD6034AA0C61AD22587F8746D0DC488C8 |
> | Per user - ARM64     | [PowerToysUserSetup-0.81.0-arm64.exe][ptUserArm64] | 75330A2DB4F9EF9B548B3B58F8BF3262C8C67E680042639BBBBC87EA244F24E2 |
> | Machine wide - x64   | [PowerToysSetup-0.81.0-x64.exe][ptMachineX64] | 29F151B01FE3C94D4FD75F2D6E8F09A6C0F0962385B83A5A733F6717312F639D |
> | Machine wide - ARM64 | [PowerToysSetup-0.81.0-arm64.exe][ptMachineArm64] | FCE636220E1FB854771258D9558E07B7532728AD4C722A7920338DEE60DEECF7 |
>
> ## Highlights
>
>  - New utility: Advanced Paste - This is an evolution based on feedback of the Paste As Plain Text utility to do more. It can paste as plain text, markdown, or json directly with the new UX or with a direct keystroke invoke.  These are fully locally executed. In addition, it now has an AI powered option as well if you wish with the free form text box.  The AI feature is 100% opt-in and requires an Open AI key. This new system will allow us to have more freedom in the future to quickly add in new features like pasting an image directly to a file or handle additional meta data types past just text.
>     - Thanks [@craigloewen-msft](https://github.com/craigloewen-msft) for the core functionality and [@niels9001](https://github.com/niels9001) for the UI/UX design!
>  - Command Not Found now uses the PowerShell Gallery release and now supports ARM64. Thanks [@carlos-zamora](https://github.com/carlos-zamora)!
>  - Fixed most accessibility issues opened after the latest accessibility review.
>  - Refactored, packaged and released the main Environment Variables Editor, Hosts File Editor and Registry Preview utilities functionality as controls to be integrated into DevHome. Thanks [@dabhattimsft](https://github.com/dabhattimsft) for validating and integrating into DevHome!
>
> ### General
>
>  - Fixed crashes on older CPUS by updating .NET to 8.0.4. (This was a hotfix for 0.80)
>
> ### Advanced Paste
>
>  - New utility: Advanced Paste - This is an evolution based on feedback of the Paste As Plain Text utility to do more. It can paste as plain text, markdown, or json directly with the new UX or with a direct keystroke invoke.  These are fully locally executed. In addition, it now has an AI powered option as well if you wish with the free form text box.  The AI feature is 100% opt-in and requires an Open AI key. This new system will allow us to have more freedom in the future to quickly add in new features like pasting an image directly to a file or handle additional meta data types past just text.
>     - Thanks [@craigloewen-msft](https://github.com/craigloewen-msft) for the core functionality and [@niels9001](https://github.com/niels9001) for the UI/UX design!
>
> ### AlwaysOnTop
>
>  - Enable border anti-aliasing. Thanks [@ewancg](https://github.com/ewancg)!
>
> ### Color Picker
>
>  - Improved accessibility by making the Settings and Copy to clipboard buttons focusable.
>  - Improved accessibility by supporting picking a color using the keyboard.
>
> ### Command Not Found
>
>  - Upgraded the Command Not Found to use the new PowerShell Gallery release and support ARM64. Thanks [@carlos-zamora](https://github.com/carlos-zamora)!
>
> ### Environment Variables Editor
>
>  - Refactored, packaged and released the main Environment Variables Editor functionality as a control to be integrated into DevHome. Thanks [@dabhattimsft](https://github.com/dabhattimsft) for validating and integrating into DevHome!
>
> ### FancyZones
>
>  - Fixed window wrap around behavior when overriding Windows key and arrow shortcuts on single monitor scenarios. Thanks [@DanRosenberry](https://github.com/DanRosenberry)!
>  - Improved accessibility of the editor by listing the keyboard shortcuts in the Canvas Editor.
>
> ### File Explorer add-ons
>
>  - Updated Monaco to 0.47 and added the new sticky scroll setting for DevFiles viewer. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Added the new font size setting for DevFiles viewer. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Added support for .srt (subtitle) file previewing in DevFiles viewer. Thanks [@PesBandi](https://github.com/PesBandi)!
>
> ### Hosts File Editor
>
>  - Refactored, packaged and released the main Hosts File Editor functionality as a control to be integrated into DevHome. Thanks [@dabhattimsft](https://github.com/dabhattimsft) for validating and integrating into DevHome!
>
> ### Image Resizer
>
>  - Supported narrator announcing the checkboxes in the UI and the sizes combobox. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved accessibility by increasing contrast in the text color of combobox items.
>
> ### Installer
>
>  - Fixed some install failures when the folders the DSC module is to be installed in isn't accessible by the WiX installer. (This was a hotfix for 0.80)
>  - Detecting install location for DSC now uses registry instead of WMI to improve performance. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed an error causing the machine scope installer to not install correctly in machines where the documents folder is in a UNC network path. We're still working in a fix for the user scope installer.
>
> ### Keyboard Manager
>
>  - Fixed startup crashes in the editor when the Visual C++ Redistributable wasn't installed. (This was a hotfix for 0.80)
>  - Fixed an accessibility issue where the first button wasn't focused after adding a new row in the editor.
>  - Environment Variables are now expanded in arguments of programs started through a shortcut. Thanks [@HydroH](https://github.com/HydroH)!
>
> ### Paste as Plain Text
>
>  - Paste as Plain Text was removed as a separate utility, since its functionality is now part of the Advanced Paste utility.
>
> ### Peek
>
>  - Updated icons, tweaked UI and refactored internal code. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Updated Monaco to 0.47 and added the new sticky scroll setting for DevFiles viewer. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Added the new font size setting for DevFiles viewer. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Upgrade the SharpCompress dependency to 0.37.2 and fixed archive parsing. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed aliasing in the image viewer.
>  - Added support for .srt (subtitle) file previewing in DevFiles viewer. Thanks [@PesBandi](https://github.com/PesBandi)!
>
> ### Power Rename
>
>  - Fixed the descriptions that were mixed up in the regex helper (\S and \w).
>
> ### PowerToys Run
>
>  - Added support for UNC paths starting with // in the Folder plugin. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed the plugin load failed message to list the failed plugins. Thanks [@belkiss](https://github.com/belkiss)!
>  - Icons for MSIX packages are now updated when a package update is detected. Thanks [@HydroH](https://github.com/HydroH)!
>  - Use Mica backdrop instead of Acrylic to fix random crashes caused by the Windows composition being momentarily turned off.
>  - Improved accessibility in the results list action buttons by improving contrast of hovered/focused buttons.
>
> ### Quick Accent
>
>  - Added support for the Esperanto character set. Thanks [@salutontalk](https://github.com/salutontalk) and [@ccmywish](https://github.com/ccmywish)!
>  - Added the ǽ and ϑ characters. Thanks [@PesBandi](https://github.com/PesBandi)!
>
> ### Registry Preview
>
>  - Refactored, packaged and released the main Registry Preview functionality as a control to be integrated into DevHome. Thanks [@dabhattimsft](https://github.com/dabhattimsft) for validating and integrating into DevHome!
>
> ### Text Extractor
>
>  - Fixed an issue causing the Settings page to not be opened when clicking the Settings button in Text Extractor's overlay. (This was a hotfix for 0.80)
>
> ### Settings
>
>  - Improved UI ordering of the File Explorer add-ons. Thanks [@niels9001](https://github.com/niels9001)!
>  - Applied fixes to theme overriding and cleaned up unneeded code. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed misspells in references to the Hosts File Editor utility. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved accessibility of the Select Folder button in the Settings Backup UI.
>  - Improved accessibility by improving focus and tab navigation in the ColorPicker page. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added a description to the fallback encoder setting in the Image Resizer page. Thanks [@Kissaki](https://github.com/Kissaki)!
>  - Refactored and improved performance in the PowerToys Run plugins UI in the Settings page. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed a crash when a user cleared the contents of a Number Box in the PowerToys Run plugins additional options. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Update the PATH environment variables with the user scope PATH when entering the Command Not Found page to improve PowerShell detection.
>
> ### Documentation
>
>  - Added the WebSearchShortcut plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@Daydreamer-riri](https://github.com/Daydreamer-riri)!
>  - Updated COMMUNITY.md with the project managers that are part of the core team.
>  - Improved the DSC samples.
>  - Added the 1Password plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@KairuDeibisu](https://github.com/KairuDeibisu)!
>  - Added the UnicodeInput plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@nathancartlidge](https://github.com/nathancartlidge)!
>
> ### Development
>
>  - Updated System.Drawing.Common to 8.0.5 to fix CI builds after the .NET 8.0.5 upgrade was released.
>  - Fixed file permissions when doing a build using cache on PR CI. Thanks [@dfederm](https://github.com/dfederm)!
>  - Removed the Test SDK reference on ARM64 to fix local building for ARM64. Thanks [@dfederm](https://github.com/dfederm)!
>  - Replaced make_pair with RemapBufferRow in Keyboard Manager internal code. Thanks [@masaru-iritani](https://github.com/masaru-iritani)!
>  - Added CODEOWNERS file to protect sensitive parts of the repo. Thanks [@htcfreek](https://github.com/htcfreek) for the help in figuring out how to make the spellcheck folder an exception!
>  - Added comments in code. to make it clear what the error badge in PowerToys Run plugin list in Settings means. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Enabled caching by default in the PR CI pipelines. Thanks [@dfederm](https://github.com/dfederm)!
>  - Disabled caching for PR started from forks, since those were failing. Thanks [@dfederm](https://github.com/dfederm)!
>  - Removed baseline files for policy checking and turned on the "TSA" process in the release pipelines instead.
>  - Added caching of nuget packages in the PR CI pipelines. Thanks [@dfederm](https://github.com/dfederm)!
>  - Updated the release CI pipelines TouchdownBuildTask to v3.
>  - Moved the release CI pipelines to ESRPv5.
>  - Added a policy for GitHub Copilot Workspaces for the repo on GitHub. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>

___

# v0.80.1		10/04/2024

> This is a patch release to fix issues in v0.80.0 we deemed important for stability based on incoming rates. See [v0.80.0](https://github.com/microsoft/PowerToys/releases/tag/v0.80.0) for full release notes.
>
> The next release is planned to be released during Microsoft Build 2024 (late May).
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.1/PowerToysUserSetup-0.80.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.1/PowerToysUserSetup-0.80.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.1/PowerToysSetup-0.80.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.1/PowerToysSetup-0.80.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.80.1-x64.exe][ptUserX64] | 23E35F7B33C6F24237BCA3D5E8EDF9B3BD4802DD656C402B40A4FC82670F8BE3 |
> | Per user - ARM64     | [PowerToysUserSetup-0.80.1-arm64.exe][ptUserArm64] | C5EECF0D9D23AB8C14307F91CA28D2CF4DA5932D705F07AE93576C259F74B4D1 |
> | Machine wide - x64   | [PowerToysSetup-0.80.1-x64.exe][ptMachineX64] | 62373A08BB8E1C1173D047509F3EA5DCC0BE1845787E07BCDA3F6A09DA2A0C17 |
> | Machine wide - ARM64 | [PowerToysSetup-0.80.1-arm64.exe][ptMachineArm64] | 061EF8D1B10D68E69D04F98A2D8E1D8047436174C757770778ED23E01CC3B06C |
>
> ## Highlights
>
> - [#32315](https://github.com/microsoft/PowerToys/issues/32315) - Updated .NET to 8.0.4 to fix crashes on the startup of many utilities when running on older CPUs.
> - [#32341](https://github.com/microsoft/PowerToys/issues/32341) - Fixed install failures that occurred when the folders we want to install the DSC module in aren't accessible by the WiX installer.
> - [#31708](https://github.com/microsoft/PowerToys/issues/31708) - Fixed Keyboard Manager Editor crashes on startup when Visual C++ Redistributable was not installed.
> - [#32352](https://github.com/microsoft/PowerToys/issues/32352) - Fixed Text Extractor settings page not opening when pressing the button directly in the utility's overlay.
>

___

# v0.80.0		04/04/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F54
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F53
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.0/PowerToysUserSetup-0.80.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.0/PowerToysUserSetup-0.80.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.0/PowerToysSetup-0.80.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.80.0/PowerToysSetup-0.80.0-arm64.exe
>
> In the [v0.80 release cycle][github-current-release-work], we focused on stability, improvements and new features.
>
> The next release is planned to be released during Microsoft Build 2024 (late May).
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.80.0-x64.exe][ptUserX64] | 4D20EB01C4035BB41F57D43AED2A546547E1FAA660FE29DC1CC699F1916DE1CC |
> | Per user - ARM64     | [PowerToysUserSetup-0.80.0-arm64.exe][ptUserArm64] | 1B85E95B0EC7D8CE1EE51B987449DA9A36DAAA4C27DF8EE4796001848EA2CBD1 |
> | Machine wide - x64   | [PowerToysSetup-0.80.0-x64.exe][ptMachineX64] | 2D17C1920D970332D93449184D7C2470052686FD4B3EB8ED49EF6475D1D1D62F |
> | Machine wide - ARM64 | [PowerToysSetup-0.80.0-arm64.exe][ptMachineArm64] | 0DD40B7A31E35472688A55A8E1ECE58847EA423F3F19FD7B8C557F1271F73F24 |
>
> ## Highlights
>
>  - New feature: Desired State Configuration support, allowing the use of winget configure for PowerToys. Check the [DSC documentation](https://aka.ms/powertoys-docs-dsc-configure) for more information.
>  - The Windows App SDK dependency was updated to 1.5.1, fixing many underlying UI issues.
>  - WebP/WebM files support was added to Peek. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Audio files support was added to Peek. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Automated UI testing for FancyZones Editor was added to CI.
>
> ### General
>
>  - Added a Quick Access entry to access the flyout from PowerToys' tray icon right click menu. Thanks [@pekvasnovsky](https://github.com/pekvasnovsky)!
>  - Added support for Desired State Configuration in PowerToys, allowing the use of winget configure to configure many settings.
>
> ### Awake
>
>  - Fix an issue causing the "Keep screen on" option to disable after Awake deactivated itself.
>
> ### Color Picker
>
>  - Fixed a UI issue causing the color picker modal to hide part of the color bar. Thanks [@TheChilledBuffalo](https://github.com/TheChilledBuffalo)!
>
> ### Command Not Found
>
>  - Now tries to find a preview version of PowerShell if no stable version is found.
>
> ### FancyZones
>
>  - Fixed a crash loading the editor when there's a layout with an empty name in the configuration file.
>  - Refactored layout internal data structures and common code to allow for automated testing.
>  - The pressing of the shift key is now detected through raw input to fix an issue causing the shift key to be locked for some users.
>
> ### File Explorer add-ons
>
>  - Fixed a crash occurring in the Monaco previewer when a file being previewed isn't found by the code behind.
>  - Fixed an issue in the Markdown previewer adding a leading space to code blocks. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Fixed wrong location and scaling of preview results on screens with different DPIs.
>  - Added better clean up code to thumbnail handlers to prevent locking files.
>
> ### File Locksmith
>
>  - Allow multiple lines to wrap when viewing the modal with selected file paths. Thanks [@sanidhyas3s](https://github.com/sanidhyas3s)!
>
> ### Installer
>
>  - Fixed the final directory name of the PowerToys Run VSCode Workspaces plugin in the installation directory to match the plugin name. Thanks [@zetaloop](https://github.com/zetaloop)!
>  - Used more generic names for the bootstrap steps, so that "Installing PowerToys" is not shown when uninstalling.
>
> ### Keyboard Manager
>
>  - Fixed an issue that would clear out KBM mappings when certain numpad keys were used as the second key of a chord.
>  - Added a comment in localization files so that translators won't translate "Text" as "SMS".
>
> ### Peek
>
>  - Added support to .WebP/.WebM files in the image/video previewer. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added support for audio files. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed an issue causing the open file button in the title bar to be un-clickable. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed an issue when previewing a folder with a dot in the name that caused Peek to try to preview it as a file. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### PowerToys Run
>
>  - Added a setting to the Windows Search plugin to exclude files and patterns from the results. Thanks [@HydroH](https://github.com/HydroH)!
>  - Fixed an issue showing thumbnails caused by a hash collision between similar images.
>  - Added the "checkbox and multiline text box" additional property type for plugins and improved multiline text handling. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Quick Accent
>
>  - Added the Schwa character to the Italian character set. Thanks [@damantioworks](https://github.com/damantioworks)!
>
> ### Registry Preview
>
>  - Allow alternative valid names for the root keys. Thanks [@e-t-l](https://github.com/e-t-l)!
>  - Fixed an issue causing many pick file windows to be opened simultaneously. Thanks [@randyrants](https://github.com/randyrants)!
>
> ### Screen Ruler
>
>  - Updated the measure icons for clarity. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker) and [@niels9001](https://github.com/niels9001)!
>
> ### Shortcut Guide
>
>  - Updated the Emoji shortcut that is shown to the new Windows key + period (.) hotkey.
>
> ### Text Extractor
>
>  - Fixed issues creating the extract layout on certain monitor configurations.
>
> ### Video Conference Mute
>
>  - Added enable/disable telemetry to get usage data.
>
> ### Settings
>
>  - Added locks to some terms (like the name of some utilities) so that they aren't localized.
>  - Fixed some shortcuts not being shown properly in the Flyout and Dashboard. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Updated image for Color Picker and outdated animations for utilities in OOBE. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Documentation
>
>  - Added FastWeb plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@CCcat8059](https://github.com/CCcat8059)!
>  - Removed the old security link to MSRC from the create new issue page, since security.md is already linked there.
>  - Added clarity regarding unofficial plugins to the PowerToys Run thirdPartyRunPlugins.md docs.
>
> ### Development
>
>  - Updated System.Drawing.Common to 8.0.3 to fix CI builds after the .NET 8.0.3 upgrade was released.
>  - Adjusted the GitHub action names for releasing to winget and Microsoft Store so they're clearer in the UI.
>  - Upgraded WinAppSDK to 1.5.1, fixing many related issues.
>  - Consolidate the WebView2 version used by WinUI 2 in the Keyboard Manager Editor.
>  - Unified the use of Precompiled Headers when building on CI. Thanks [@dfederm](https://github.com/dfederm)!
>  - Added UI tests for FancyZones Editor in CI.
>  - Added a GitHub bot to identify possible duplicates when a new issue is created. Thanks [@craigloewen-msft](https://github.com/craigloewen-msft)!
>  - Updated the WiX installer dependency to 3.14.1 to fix possible security issues.
>  - Changed the pipelines to use pipeline artifacts instead of build artifacts. Thanks [@dfederm](https://github.com/dfederm)!
>  - Added the -graph parameter for pipelines. Thanks [@dfederm](https://github.com/dfederm)!
>  - Tests in the pipelines now run as part of the build step to save on CI time. Thanks [@dfederm](https://github.com/dfederm)!
>

___

# v0.79.0		04/03/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F53
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F52
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.79.0/PowerToysUserSetup-0.79.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.79.0/PowerToysUserSetup-0.79.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.79.0/PowerToysSetup-0.79.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.79.0/PowerToysSetup-0.79.0-arm64.exe
>
> In the [v0.79 release cycle][github-current-release-work], we focused on stability, improvements and new features.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.79.0-x64.exe][ptUserX64] | CF1C715F952A34416CDBE5D06D24FFF47790DDA1D4CA3F81BCAD9D28FF0039A1 |
> | Per user - ARM64     | [PowerToysUserSetup-0.79.0-arm64.exe][ptUserArm64] | ADE572B6F1B59DCDC60A2550D9FD00B8CC7C78BE9330F534691CE4B056ED76F1 |
> | Machine wide - x64   | [PowerToysSetup-0.79.0-x64.exe][ptMachineX64] | 3FD2A6BD9C8F8973BFBBF5DB9236C3D8AF3AE57E5AEC275DDEB5EF31581F80FE |
> | Machine wide - ARM64 | [PowerToysSetup-0.79.0-arm64.exe][ptMachineArm64] | B93017C2A5CFB0DEF708DB412570AA39828E91D85E800EFD22481B46F0DC6852 |
>
> ## Highlights
>
>  - New feature: Keyboard Manager allows mapping shortcuts to start applications or opening URIs. Thanks [@jefflord](https://github.com/jefflord)!
>  - New feature: Keyboard Manager allows shortcuts with chords. Thanks [@jefflord](https://github.com/jefflord)!
>  - Modernized Color Picker with Fluent UX. Thanks [@niels9001](https://github.com/niels9001)!
>  - Peek now is able to preview drives. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - File Locksmith has now an entry in the Windows 11 tier 1 context menu.
>
> ### General
>
>  - Refactored code so that English is used as a fallback language when a localized resource cannot be found.
>
> ### Awake
>
>  - The setting now reverts to "Keep using the current power plan" after Awake deactivates itself after any of the timed modes has expired.
>
> ### Color Picker
>
>  - Now uses WPFUI and the UI was updated to follow Fluent UX principles. Thanks [@niels9001](https://github.com/niels9001)!
>  - Added enable and disable telemetry to align it with the other utilities.
>
> ### Command Not Found
>
>  - Added telemetry for when a module instance is created in PowerShell.
>
> ### FancyZones
>
>  - Fixed a memory leak occurring on work area changes.
>
> ### File Explorer add-ons
>
>  - Added support to the .ksh, .zsh, .bsh and .env file types to Monaco previewer. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Re-enabled the RendererAppContainer feature in WebView2, since the associated crash has been fixed in the latest WebView2 releases.
>
> ### File Locksmith
>
>  - Added as an entry in the Windows 11 tier 1 context menu.
>
> ### Hosts File Editor
>
>  - Tweaked filter button style to indicate if filters are applied.
>  - Added an error indicator to each input field to indicate why a new entry can't be created.
>  - Added an in-line delete button for each entry.
>
> ### Image Resizer
>
>  - Units and resize modes are now localized.
>  - Tweaked and improved UI. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>
> ### Keyboard Manager
>
>  - Added a feature that allows remapping a shortcut to starting an application. Thanks [@jefflord](https://github.com/jefflord)!
>  - Added a feature that allows remapping a shortcut to open a URI. Thanks [@jefflord](https://github.com/jefflord)!
>  - Added chords to shortcuts. Thanks [@jefflord](https://github.com/jefflord)!
>  - Send telemetry about the key/shortcut to key/shortcut remappings that are set. This doesn't include remap to text, application or URI since those might contain personal information.
>  - Added telemetry to send a daily event that at least a key/shortcut to key/shortcut remapping was used.
>  - Tweaked and fixed the chords code to better follow conventions when trying to call the same chord multiple times.
>
> ### Mouse Without Borders
>  - Fixed an issue causing the target path string to be corrupted when registering as a service.
>
> ### Paste as Plain Text
>
>  - Prevent the start menu from activating when the Windows key is part of the activation shortcut and is released sooner.
>
> ### Peek
>
>  - Fixed a title bar issue after maximizing Peek's window. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed a crash when trying to use Peek in File Explorer alternatives.
>  - Added a previewer for drives. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - The folder previewer will now asynchronously calculate size, similar to the Properties screen in File Explorer. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added support to the .ksh, .zsh, .bsh and .env file types to Monaco previewer. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>
> ### PowerRename
>
>  - PowerRename context menu accelerator key readded.
>  - Tweaked PowerRename apply button style. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### PowerToys Run
>
>  - Fixed an issue causing win32 application icons to not appear correctly in the Programs plugin.
>  - Unified phrasing in the plugin descriptions.
>  - Fixed an issue causing the PowerToys Run plugin settings to be cleared with each upgrade.
>  - Fixed an issue causing VSCodeWorkspaces plugin to not find WSL workspaces.
>  - Fixed results tooltip closing fast. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved the Registry plugin tooltip spacing. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Allow pressing '=' to replace the query with the current result when using the calculator plugin. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Optimized the code that gathers results from the plugin to reduce CPU consumption.
>  - Optimized memory usage in the Window Walker plugin.
>  - Fixed crashes and improved error handling when saving json configuration files.
>  - The Program plugin will now correctly get the icon for a newly installed packaged application. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Quick Accent
>
>  - Added support for the Slovenian character set. Thanks [@aklemen](https://github.com/aklemen)!
>
> ### Registry Preview
>
>  - Fixed a crash when closing the application and the editor's right click menu is opened.
>
> ### Settings
>
>  - Fixed an alignment issue in the flyout icons causing some icons to be centered when they shouldn't. Thanks [@niels9001](https://github.com/niels9001)!
>  - Added the mention that Monaco supports .txt files. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Fixed an issue causing the Settings window to lose its previous maximized state. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Documentation
>
>  - Fixed broken links in doc/devdocs/readme.md. Thanks [@jem-experience](https://github.com/jem-experience)!
>
> ### Development
>
>  - Updated Microsoft.MSBuildCache to 0.1.258-preview. Thanks [@dfederm](https://github.com/dfederm)!
>  - Fixed CI to point VCToolsVersion to VC.CRT instead of VC.Redist version. Thanks [@snickler](https://github.com/snickler)!
>  - Updated MSTest adapter and framework to 3.2.
>  - Fixed CI by pointing WiX 3.14 urls and hashes to the latest release on GitHub.
>  - Added Pro and Enterprise editions of Visual Studio to the repository's development configuration DSC scripts.
>  - Updated CppWinRT to 2.0.240111.5.
>  - Updated System.Drawing.Common to 8.0.2 to fix CI builds after the .NET 8.0.2 upgrade was released.
>  - Updated WPFUI version to 3.0.0. Thanks [@niels9001](https://github.com/niels9001)!
>  - XAML Styler is now fully tested in the solution when CI runs. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed a faulty XAML binding in the Text Extractor settings page.
>  - Updated Microsoft.Web.WebView2 to 1.0.2365.46.
>

___

# v0.78.0		30/01/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F52
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F51
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.78.0/PowerToysUserSetup-0.78.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.78.0/PowerToysUserSetup-0.78.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.78.0/PowerToysSetup-0.78.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.78.0/PowerToysSetup-0.78.0-arm64.exe
>
> In the [v0.78 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.78.0-x64.exe][ptUserX64] | 120B1CEFC94D76EC593A61D717BBB2E12AF195D19E04C811F519D3F9B9B3B5C0 |
> | Per user - ARM64     | [PowerToysUserSetup-0.78.0-arm64.exe][ptUserArm64] | 3C3C8A8A549ABDD1C5E5DA7DC22D254F7BBD0F9DC05DA17E51020B153662F083 |
> | Machine wide - x64   | [PowerToysSetup-0.78.0-x64.exe][ptMachineX64] | 19E025381588ABAEC209CDD0A18BB779EE58FC24646D898C2A7C38A4858EAEDB |
> | Machine wide - ARM64 | [PowerToysSetup-0.78.0-arm64.exe][ptMachineArm64] | 5C70054A8991885A958F066B00D7FAFE608C730FC7A99178D6C64A1F03A3C109 |
>
> ## Highlights
>
>  - New languages added: Arabic (Saudi Arabia), Hebrew, Persian and Ukrainian. We are going to assume we have some bugs. We want to identify & fix them and are open for community help.
>  - Many dependencies updated, aiming for security and stability.
>  - Fixed commonly reported PowerToys Run startup crashes after an upgrade.
>  - New settings and GPO policies to help control behavior after an upgrade. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### General
>
>  - Added Arabic (Saudi Arabia) translation.
>  - Added Hebrew translation.
>  - Added Persian translation.
>  - Added Ukrainian translation.
>  - Improved the file watcher used across many utilities to consume less resources. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### AlwaysOnTop
>
>  - Fixed an invisible border issue when the border color was set to the black color.
>  - Added the AlwayOnTop icon to the base application executable. Thanks [@ckirby19](https://github.com/ckirby19)!
>
> ### Command Not Found
>
>  - Signed the PowerShell scripts used by the Command Not Found installation process.
>
> ### File Explorer add-ons
>
>  - Fixed an issue causing SVG Thumbnail generation to hang when trying to preview SVG files at the same time.
>
> ### File Locksmith
>
>  - Improved the context menu entry caption. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Find My Mouse
>
>  - Added more settings to tune shake detection when activating through mouse shake.
>
> ### Hosts File Editor
>
>  - Added a feature to duplicate an entry. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Installer
>
>  - Included the new languages localization files in the installer.
>
> ### Image Resizer
>
>  - Improved the context menu entry caption. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Peek
>
>  - Added a missing tooltip for the file size. Thanks [@HydroH](https://github.com/HydroH)!
>
> ### PowerRename
>
>  - Improved and added localization to the context menu entry caption. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### PowerToys Run
>
>  - Removed references to unused settings from the code, which were causing crashes on some machines. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed an issue causing a scrollbar to be out of view. Thanks [@niels9001](https://github.com/niels9001)!
>  - Added logic to try and detect running games to full screen detection. Thanks [@anaisbetts](https://github.com/anaisbetts)!
>  - Added support for converting negative values in the Unit Converter plugin. Thanks [@Dub1shu](https://github.com/Dub1shu)!
>  - Fixed stale results in the Visual Studio Code Workspaces plugin by checking if files still exist. Thanks [@anderspk](https://github.com/anderspk)!
>  - Fixed an activation crash that occurred after 0.77 on some configurations.
>  - Fixed a startup crash that occurred when saving the new version of settings after an upgrade.
>  - You can now calculate bigger hexadecimal numbers in the Calculator plugin.
>  - The "max results to show before scrolling" setting can now also be applied to the initial plugin hint listing.
>
> ### Quick Accent
>
>  - Added the ellipses character to all languages. Thanks [@HydroH](https://github.com/HydroH)!
>  - Added an option to not activate when playing a game. Thanks [@HydroH](https://github.com/HydroH)!
>  - Added the E with breve and pilcrow characters to all languages. Thanks [@PesBandi](https://github.com/PesBandi)!
>
> ### Settings
>
>  - Removed the Command Not Found listing from the Settings dashboard and flyout, since it can't really be enabled or disabled from there.
>  - Added a settings and GPO rule to disable opening the What's New OOBE page after an update. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Added a settings and GPO rule to disable toast notifications about new updates being available. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed an issue causing the Settings window to not be brought to the foreground after activating through the system tray icon.
>  - Standardized accent brush and corner radius on the dashboard page.
>  - Improved UI and messages for GPO locked settings. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed an issue causing the OOBE window to maximize and hide the system taskbar.
>  - Reworked the update settings in the General page. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Tweaked UI for the update settings in the General page. Thanks [@niels9001](https://github.com/niels9001)!
>  - Updated the modules images in the Settings and OOBE screens. Thanks [@niels9001](https://github.com/niels9001)!
>  - Updated OOBE descriptions to take into account the changes in context menu captions. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Documentation
>
>  - Added Spotify plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@waaverecords](https://github.com/waaverecords)!
>  - Added InputTyper and ClipboardManager plugins to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@CoreyHayward](https://github.com/CoreyHayward)!
>  - Added CurrencyConverter plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@Advaith3600](https://github.com/Advaith3600)!
>  - Updated and cleaned up the new PowerToys plugin checklist documentation. Thanks [@Parvezkhan0](https://github.com/Parvezkhan0) and [@hlaueriksson](https://github.com/hlaueriksson)!
>  - Added a documentation page to describe status code colors for Mouse Without Borders. Thanks [@ckirby19](https://github.com/ckirby19)!
>
> ### Development
>
>  - Fixed dependency issues on upgrading .NET from 8.0.0 to 8.0.1.
>  - Upgraded Microsoft.Extensions.ObjectPool from .NET 5 to .NET 8.
>  - Upgraded the Windows SDK Build Tools to 10.0.22621.2428.
>  - Upgraded the Windows Implementation Library to 1.0.231216.1.
>  - Upgraded NLog.Schema to 5.2.8 and NLog.Extensions.Logging to 5.3.8.
>  - Upgraded Markdig.Signed to 0.34.0.
>  - Upgraded Microsoft.NET.Test.Sdk to 17.8.
>  - Upgraded CommunityToolkit.WinUI dependencies to 8.0.240109.
>  - Upgraded CommunityToolkit.Mvvm to 8.2.2. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Upgraded Windows App SDK to 1.4.4. Thanks [@snickler](https://github.com/snickler)!
>  - Upgraded WPFUI version to 3.0.0-preview.13. Thanks [@niels9001](https://github.com/niels9001)!
>  - Upgraded StyleCop.Analyzers to 1.2.0-beta.556. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Upgraded Microsoft.Windows.Compatibility to 8.0.1.
>  - Upgraded System.Data.SqlClient to 4.8.6.
>  - Consolidate XAML Namespaces across the solutions. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Removed the toolkit labs package source reference, since the controls we were using made it to the generally available community toolkit.
>  - Added Microsoft.MSBuildCache to experiment with build caching to reduce pipeline runs duration. Thanks [@dfederm](https://github.com/dfederm)!
>  - Configured the release CI to follow the latest 1ES pipeline release version again.
>  - Removed the copyright year from assembly information. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Added the Command Not Found entry to the GitHub templates.
>  - Removed unused code for a GPO policy to control auto updating of PowerToys. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Improved code behind for getting the localization of context menu entries.
>  - Locked some terms in resource files to avoid localization.
>

___

# v0.77.0		09/01/2024

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F51
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F50
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.77.0/PowerToysUserSetup-0.77.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.77.0/PowerToysUserSetup-0.77.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.77.0/PowerToysSetup-0.77.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.77.0/PowerToysSetup-0.77.0-arm64.exe
>
> In the [v0.77 release cycle][github-current-release-work], we focused on new features, stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.77.0-x64.exe][ptUserX64] | 3485D3F45A3DE6ED7FA151A4CE9D6F941491C30E83AB51FD59B4ADCD20611F1A |
> | Per user - ARM64     | [PowerToysUserSetup-0.77.0-arm64.exe][ptUserArm64] | 762DF383A01006A20C0BAB2D321667E855236EBA7108CDD475E4E2A8AB752E0E |
> | Machine wide - x64   | [PowerToysSetup-0.77.0-x64.exe][ptMachineX64] | 1B6D4247313C289B07A3BF3531E215B3F9BEDBE9254919637F2AC502B4773C31 |
> | Machine wide - ARM64 | [PowerToysSetup-0.77.0-arm64.exe][ptMachineArm64] | CF740B3AC0EB5C23E18B07ACC2D0C6EC5F4CE4B3A2EDC67C2C9FDF6EF78F0352 |
>
> ## Highlights
>
>  - New utility: Command Not Found PowerShell 7.4 module - adds the ability to detect failed commands in PowerShell 7.4 and suggest a package to install using winget. Thanks [@carlos-zamora](https://github.com/carlos-zamora)!
>  - Keyboard manager does not register low level hook if there are no remappings anymore.
>  - Added support for QOI file type in Peek. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>  - Added support for loading 3rd-party plugins with additional dependencies in PowerToys Run. Thanks [@coreyH](https://github.com/CoreyHayward)!
>
> ### Known issues
>
> - There are some incompatibilities between Command Not Found and some PowerShell configurations. You can find about those in the [#30818](https://github.com/microsoft/PowerToys/issues/30818) issue.
>
> ### General
>
>  - Bump WPF-UI package version to fix crashes related to theme changes. (This was a hotfix for 0.76)
>  - Fixed typo in version change notification. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Code improvements and fixed silenced warnings introduced by upgrade to .NET 8.
>  - Update copyright year for 2024.
>  - Added setting to disable warning notifications about detecting an application running as Administrator.
>
> ### AlwaysOnTop
>
>  - Show notification when elevated app is in the foreground but AlwaysOnTop is running non-elevated.
>
> ### Command Not Found
>
>  - Added a new utility: A Command Not Found PowerShell 7.4 module. It adds the ability to detect failed commands in PowerShell 7.4 and suggest a package to install using winget. Thanks [@carlos-zamora](https://github.com/carlos-zamora)!
>
> ### Environment Variables
>
>  - Fixed issue causing Environment Variables window not to appear as a foreground window.
>
> ### FancyZones
>
>  - Fixed snapping specific apps (e.g. Facebook messenger). (This was a hotfix for 0.76)
>  - Fixed behavior of Move newly created windows to current active monitor setting to keep maximize state on moving. Thanks [@quyenvsp](https://github.com/quyenvsp)!
>  - Fixed issue causing FancyZones Editor layout window to be zoned.
>
> ### File Explorer add-ons
>
>  - Fixed WebView2 based previewers issue caused by the latest WebView update. (This was a hotfix for 0.76)
>
> ### Hosts File Editor
>
>  - Fixed issue causing settings not to be preserved on update.
>
> ### Image Resizer
>
>  - Fixed crash caused by WpfUI ThemeWatcher. (This was a hotfix for 0.76)
>
> ### Keyboard Manager
>
>  - Do not register low level hook if there are no remappings.
>
> ### Peek
>
>  - Improved icon and title showing for previewed files. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added QOI file type support. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>
> ### PowerToys Run
>
>  - Fixed results list UI element height for different maximum number of results value. (This was a hotfix for 0.76)
>  - Fixed icon extraction for .lnk files. (This was a hotfix for 0.76)
>  - Fixed search box UI glitch when FlowDirection is RightToLeft. (This was a hotfix for 0.76)
>  - Fixed theme setting. (This was a hotfix for 0.76)
>  - Fixed error reporting window UI issue. Thanks [@niels9001](https://github.com/niels9001)!
>  - UI improvements and ability to show/hide plugins overview panel. Thanks [@niels9001](https://github.com/niels9001)!
>  - Allow interaction with plugin hints. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Switch to WPF-UI theme manager. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed issue causing 3rd party plugin's dependencies dll not being loaded properly. Thanks [@coreyH](https://github.com/CoreyHayward)!
>  - Added configurable font sizes. Thanks [@niels9001](https://github.com/niels9001)!
>  - Changed the text color of plugin hints to improve the contrast when light theme is used. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fix scientific notation errors in Calculator plugin. Thanks [@viggyd](https://github.com/viggyd)!
>  - Add URI/URL features to Value generator plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Quick Accent
>
>  - Moved Greek specific characters from All language set to Greek. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Add more mathematical symbols. Thanks [@kevinfu2](https://github.com/kevinfu2)!
>
> ### Settings
>
>  - Fixed exception occurring on theme change.
>  - Fix "What's new" icon. Thanks [@niels9001](https://github.com/niels9001)!
>  - Remove obsolete UI Font icon properties. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - OOBE UI improvements. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - XAML Binding improvements. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Fixed crash caused by ThemeListener constructor exceptions.
>
> ### Documentation
>
>  - Improved docs for adding new languages to monaco. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Update README.md to directly state x64 & ARM processor in requirements.
>  - Added Scoop plugin to PowerToys Run thirdPartyRunPlugins.md docs. Thanks [@Quriz](https://github.com/Quriz)!
>
> ### Development
>
>  - Adopted XamlStyler for PowerToys Run source code. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Consolidate Microsoft.Windows.SDK.BuildTools across solution.
>  - Upgraded Boost's lib to v1.84.
>  - Upgraded HelixToolkit packages to the latest versions.
>  - Updated sdl baselines.
>

___

# v0.76.2		13/12/2023

> This is a patch release to fix issues in v0.76.1 we deemed important for stability based on incoming rates. See [v0.76.0](https://github.com/microsoft/PowerToys/releases/tag/v0.76.0) and [v0.76.1](https://github.com/microsoft/PowerToys/releases/tag/v0.76.1) for full release notes.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.2/PowerToysUserSetup-0.76.2-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.2/PowerToysUserSetup-0.76.2-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.2/PowerToysSetup-0.76.2-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.2/PowerToysSetup-0.76.2-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.76.2-x64.exe][ptUserX64] | 73D734FC34B3F9D7484081EC0F0B6ACD4789A55203A185904CC5C62ABD02AF16 |
> | Per user - ARM64     | [PowerToysUserSetup-0.76.2-arm64.exe][ptUserArm64] | 5284CC5DA399DC37858A2FD260C30F20C484BA1B5616D0EB67CD90A8A286CB8B |
> | Machine wide - x64   | [PowerToysSetup-0.76.2-x64.exe][ptMachineX64] | 72B87381C9E5C7447FB59D7CE478B3102C9CEE3C6EB3A6BC7EC7EC7D9EFAB2A0 |
> | Machine wide - ARM64 | [PowerToysSetup-0.76.2-arm64.exe][ptMachineArm64] | F28C7DA377C25309775AB052B2B19A299C26C41582D05B95C3492A4A8C952BFE |
>
> ## Highlights
>
> - [#30357](https://github.com/microsoft/PowerToys/issues/30357) - Fixed issue causing Wpf apps to crash or report error dialogs by using latest WPF-UI package version.
> - [#30370](https://github.com/microsoft/PowerToys/issues/30370) - Workaround for WebView2 based preview and thumbnail handlers issue caused by WebView2 runtime latest update.
>
>

___

# v0.76.1		07/12/2023

> This is a patch release to fix issues in v0.76.0 we deemed important for stability based on incoming rates. See [v0.76.0](https://github.com/microsoft/PowerToys/releases/tag/v0.76.0) for full release notes.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.1/PowerToysUserSetup-0.76.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.1/PowerToysUserSetup-0.76.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.1/PowerToysSetup-0.76.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.1/PowerToysSetup-0.76.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.76.1-x64.exe][ptUserX64] | 876FB94098A50E5954D8FF6D1E8D5372AFD5AFA0C5456AD6E5DF2F6BCD4FC49B |
> | Per user - ARM64     | [PowerToysUserSetup-0.76.1-arm64.exe][ptUserArm64] | 3D0E943A8F147EE5C794FF17FB98A490A8E7E72F4B312B441332CF0C65823420 |
> | Machine wide - x64   | [PowerToysSetup-0.76.1-x64.exe][ptMachineX64] | A31F89B1E697C77D09337FD4B4A69DCA304944702A03D664846756ED98342F9A |
> | Machine wide - ARM64 | [PowerToysSetup-0.76.1-arm64.exe][ptMachineArm64] | CFA2E1F2BC38E1A5B9B0F91060391DE98F42F286FFCA9185D9EA26552F16D38A |
>
> ## Highlights
>
> - [#30148](https://github.com/microsoft/PowerToys/issues/30148) - Fixed issue causing FancyZones not to be able to zone specific apps (e.g. Facebook messenger).
> - [#30224](https://github.com/microsoft/PowerToys/issues/30224) - Fixed results list height when changing maximum number of results to be shown setting for PowerToys Run.
> - [#30225](https://github.com/microsoft/PowerToys/issues/30225) - Fixed WPF apps crash caused by SystemThemeWatcher.
> - [#30228](https://github.com/microsoft/PowerToys/issues/30228) - Fixed loading .lnk files icons in PowerToys Run.
> - [#30237](https://github.com/microsoft/PowerToys/issues/30237) - Fixed PowerToys Run theme setting.
> - [#30251](https://github.com/microsoft/PowerToys/issues/30251) - Fixed suggestion text margin when text direction is right to left in PowerToys Run.
>
>

___

# v0.76.0		04/12/2023

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F50
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F49
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.0/PowerToysUserSetup-0.76.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.0/PowerToysUserSetup-0.76.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.0/PowerToysSetup-0.76.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.76.0/PowerToysSetup-0.76.0-arm64.exe
>
> In the [v0.76 release cycle][github-current-release-work], we focused on new features, stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.76.0-x64.exe][ptUserX64] | 627F60BF3F04583A2ECE7ACE7E6F09ABDE92493E1FFFAC5705CC83009781CD8D |
> | Per user - ARM64     | [PowerToysUserSetup-0.76.0-arm64.exe][ptUserArm64] | 79D11CDDBDD87DF8A69A5B2FC28869B3085392E5F45FEE6560D278E48F8B0673 |
> | Machine wide - x64   | [PowerToysSetup-0.76.0-x64.exe][ptMachineX64] | 4F24A288AC92DD0AB74EF52CFE3D66FA744BDC6889CB4E3ED088763D9134C06D |
> | Machine wide - ARM64 | [PowerToysSetup-0.76.0-arm64.exe][ptMachineArm64] | 1BD9CD9C696D8898AAEE5A6D6A7122F053202CF1865C511BFC91FCFD0D767864 |
>
> ## Highlights
>
>  - Upgrade to .NET 8. Thanks [@snickler](https://github.com/snickler)!
>  - Keyboard Manager can now remap keys and shortcuts to send sequences of unicode text.
>  - Modernized the Keyboard Manager Editor UI. Thanks [@dillydylann](https://github.com/dillydylann)!
>  - Modernized the PowerToys Run, Quick Accent and Text Extractor UIs. Thanks [@niels9001](https://github.com/niels9001)!
>  - New File Explorer Add-ons: QOI image Preview Handler and Thumbnail Provider. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>
> ### General
>  - Updated the WebView 2 dependency to 1.0.2088.41. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed unreadable color brushes used across WinUI3 applications for improved accessibility. Thanks [@niels9001](https://github.com/niels9001)!
>  - Flyouts used across WinUI3 applications are no longer constrained to the application's bounds. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Upgraded the WPF-UI dependency to preview.9 and then preview.11. Thanks [@niels9001](https://github.com/niels9001) and [@pomianowski](https://github.com/pomianowski)!
>  - Upgraded to .NET 8. Thanks [@snickler](https://github.com/snickler)!
>  - Updated the WinAppSDK dependency to 1.4.3.
>
> ### Awake
>
>  - Added localization to the tray icon context menu.
>
> ### Crop And Lock
>
>  - Fixed restoring windows that were reparented while maximized.
>
> ### Environment Variables
>
>  - Fixed crash caused by WinAppSDK version bump by replacing ListView elements with ItemsControl.
>
> ### FancyZones
>
>  - Reverted a change that caused some applications, like the Windows Calculator, to not snap correctly. (This was a hotfix for 0.75)
>  - FancyZones Editor will no longer apply a layout to the current monitor after editing it.
>  - Fixed and refactored the code that detected if a window can be snapped. Added tests to it with known application window styles to avoid regressions in the future.
>
> ### File Explorer add-ons
>
>  - Solved an issue incorrectly detecting encoding when previewing code files preview.
>  - Fixed the background color for Gcode preview handler on dark theme. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>  - New utilities: Preview Handler and Thumbnail Provider for QOI image files. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>  - GCode Thumbnails are now in the 32 bit ARGB format. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>  - Added the perceived type to SVG and QOI file thumbnails. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>
> ### GPO
>
>  - Added the missing Environment Variables utility policy to the .admx and .adml files. (This was a hotfix for 0.75)
>  - Fixed some typos and text improvements in the .adml file. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Hosts File Editor
>
>  - Added a proper warning when the hosts file is read-only and a button to make it writable. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Image Resizer
>
>  - Fixed a WPF-UI issue regarding the application's background brushes. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Installer
>
>  - Included the Text Extractor and Awake localization files in the install process.
>
> ### Keyboard Manager
>
>  - Modernized the UI with the Fluent design. Thanks [@dillydylann](https://github.com/dillydylann)!
>  - Added the feature to remap keys and shortcuts to arbitrary unicode text sequences.
>
> ### Mouse Without Borders
>
>  - Removed Thread.Suspend calls when exiting the utility. That call is deprecated, unneeded and was causing a silent crash.
>
> ### Peek
>
>  - Added the possibility to pause/resume videos with the space bar. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed high CPU usage when idle before initializing the main window. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Implemented Ctrl+W as a shortcut to close Peek. Thanks [@Physalis2](https://github.com/Physalis2)!
>  - Solved an issue incorrectly detecting encoding when previewing code files.
>  - Fixed background issues when peeking into HTML files after the WebView 2 upgrade.
>
> ### PowerToys Run
>
>  - Moved to WPF-UI and redesigned according to Fluent UX principles. Thanks [@niels9001](https://github.com/niels9001)!
>  - Fixed an issue causing 3rd party plugins to not have their custom settings correctly initialized with default values. (This was a hotfix for 0.75) Thanks [@waaverecords](https://github.com/waaverecords)!
>  - Fixed a crash in the VSCode plugin when the VSCode path had trailing backspaces. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed a crash when trying to load invalid image icons.
>  - Fixed a crash in the Programs plugin when getting images for some .lnk files.
>  - Fixed a rare startup initialization error and removed cold start operations that were no longer needed. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved calculations for Windows File Time and Unix Epoch Time in the DateTime plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed a crash when trying to get the icon for a link that pointed to no file.
>  - Cleaned up code in the WindowWalker plugin improving the logic. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Quick Accent
>
>  - Moved from ModernWPF to WPF-UI. Thanks [@niels9001](https://github.com/niels9001)!
>  - Added support to the Finnish language character set. Thanks [@davidtlascelles](https://github.com/davidtlascelles)!
>  - Added currency symbols for Croatian, Gaeilge, Gàidhlig and Welsh. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Added a missing Latin letter ꝡ. Thanks [@cubedhuang](https://github.com/cubedhuang)!
>  - Added fraction characters. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Added support to the Danish language character set. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Added the Kazakhstani Tenge character to the Currencies characters set. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Renamed Slovakian to Slovak, which is the correct term. Thanks [@PesBandi](https://github.com/PesBandi)!
>  - Added the Greek language character set. Thanks [@mcbabo](https://github.com/mcbabo)!
>
> ### Settings
>
>  - When clicking a module's name on the Dashboard, it will navigate to that module's page.
>  - Fixed the clipping of information in the Backup and Restore section of the General Settings page. Thanks [@niels9001](https://github.com/niels9001)!
>  - Updated the File Explorer Add-ons fluent icon. Thanks [@niels9001](https://github.com/niels9001)!
>  - Added a warning when trying to set a shortcut that might conflict with "Alt Gr" key combinations.
>  - Added a direct link to the OOBE's "What's New page" from the main Settings window. Thanks [@iakrayna](https://github.com/iakrayna)!
>  - Changed mentions from Microsoft Docs to Microsoft Learn.
>  - Fixed the slow reaction to system theme changes.
>
> ### Text Extractor
>
>  - Move to WPF-UI, localization and light theme support. Thanks [@niels9001](https://github.com/niels9001)!
>  - Disabled by default on Windows 11, with a information box on Settings to prefer using the Windows Snipping Tool, which now supports OCR.
>
> ### Documentation
>
>  - Fixed some typos in the README. Thanks [@Asymtode712](https://github.com/Asymtode712)!
>  - Reworked the gpo docs on learn.microsoft.com, adding .admx, registry and Intune information. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Development
>
>  - Updated the check-spelling ci action to 0.22. Thanks [@jsoref](https://github.com/jsoref)!
>  - Refactored the modules data model used between the Settings Dashboard and Flyout.
>  - Fixed a flaky interop test that was causing automated CI to hang occasionally.
>  - Increased the WebView 2 loading timeout to reduce flakiness in those tests. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added support for building with the Dev Drive CopyOnWrite feature, increasing build speed. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>  - Addressed the C# static analyzers suggestions. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Addressed the C++ static analyzers suggestions.
>  - PRs that only contain Markdown or text files changes no longer trigger the full CI. Thanks [@snickler](https://github.com/snickler)!
>  - Updated the Microsoft.Windows.CsWinRT to 2.0.4 to fix building with the official Visual Studio 17.8 release.
>  - Fixed new code quality issues caught by the official Visual Studio 17.8 release.
>  - Added a bot trigger to point contributors to the main new contribution issue on GitHub. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Removed unneeded entries from expect.txt.
>  - Turned off a new feature from Visual Studio that was adding the commit hash to the binary files Product Version.
>  - Refactored and reviewed the spellcheck entries into different files. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Added Spectre mitigation and SHA256 hash creation for some DLLs.
>  - Reverted the release pipeline template to a previous release that's stable for shipping PowerToys.
>

___

# v0.75.1		31/10/2023

> This is a patch release to fix issues in v0.75.0 to fix some bugs we deemed important for stability based on incoming rates. See [v0.75.0](https://github.com/microsoft/PowerToys/releases/tag/v0.75.0) for full release notes.
>
> **2023/11/02** update: The "GroupPolicyObjectsFiles-0.75.1.zip" file was updated to be based on the [d105d67b34fa5958b1a23fdfd1c0ffd209db15af](https://github.com/microsoft/PowerToys/commit/d105d67b34fa5958b1a23fdfd1c0ffd209db15af) commit, including the policy definitions for controlling the Environment Variables Editor enabled state.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.1/PowerToysUserSetup-0.75.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.1/PowerToysUserSetup-0.75.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.1/PowerToysSetup-0.75.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.1/PowerToysSetup-0.75.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.75.1-x64.exe][ptUserX64] | CFDAE52607689A695F4E4DDD7C1FE68400359AEF0D2B23C86122835E9D32A20F |
> | Per user - ARM64     | [PowerToysUserSetup-0.75.1-arm64.exe][ptUserArm64] | 9BAD3EF71DEDE70445416AC7369D115FAE095152722BC4F23EE393D8A10F45CA |
> | Machine wide - x64   | [PowerToysSetup-0.75.1-x64.exe][ptMachineX64] | 18FEB9377B0BA45189FFF4F89627B152DD794CCC15F005592B34A40A3EA62EA8 |
> | Machine wide - ARM64 | [PowerToysSetup-0.75.1-arm64.exe][ptMachineArm64] | F5CDF5A35876A0B581F446BF728B7AC52B6B701C0850D9CEA9A1874523745CFD |
>
> ## Highlights
>
> - [#29491](https://github.com/microsoft/PowerToys/issues/29491) - The generalization of a fix for snapping popup windows was causing many applications to not be snappable by FancyZones. We're reverted that change for this patch and we'll investigate a better fix for a future release.
> - [#29465](https://github.com/microsoft/PowerToys/issues/29465) - We've recently included more ways for PowerToys Run plugin developers to include different data types as additional options for their plugins, but these were not being initialized correctly. Now they should be working. Thanks [@waaverecords](https://github.com/waaverecords)!
>

___

# v0.75.0		30/10/2023

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F49
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F48
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.0/PowerToysUserSetup-0.75.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.0/PowerToysUserSetup-0.75.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.0/PowerToysSetup-0.75.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.75.0/PowerToysSetup-0.75.0-arm64.exe
>
> In the [v0.75 release cycle][github-current-release-work], we focused on new features, stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.75.0-x64.exe][ptUserX64] | 26C746D5690472082460FED3866627B80A2B50961D77DF683C5E271054C9C97B |
> | Per user - ARM64     | [PowerToysUserSetup-0.75.0-arm64.exe][ptUserArm64] | 8919FF49F3F0C4716D6F58871C9AABEB321292FDAFFB4DCCC9A8FA326D3646D2 |
> | Machine wide - x64   | [PowerToysSetup-0.75.0-x64.exe][ptMachineX64] | C9C7DAB9E002CE7920107132BC98A1A89BFBA8898FC35186A6559587A8ED3C38 |
> | Machine wide - ARM64 | [PowerToysSetup-0.75.0-arm64.exe][ptMachineArm64] | 28D9D9E9B69821155F870AF895DEE22866060C1CED08E2117CCBD1853C051698 |
>
> ## Highlights
>
>  - New utility: An environment variables editor with the functionality to configure profiles that can be enabled/disabled. Thanks [@niels9001](https://github.com/niels9001) for the design and UI work that made this possible!
>  - Settings has a new Dashboard home page, with quick access for enabling modules, short descriptions and activation methods. Thanks [@niels9001](https://github.com/niels9001) for the design and UI work that made this possible!
>  - Added a previewer to Peek that hosts File Explorer previewers to support every file type that a machine is currently able to preview. For example, this means that if Microsoft Office handlers are installed, Peek can preview Office files. Thanks [@dillydylann](https://github.com/dillydylann)!
>
> ### General
>
>  - Many typo fixes through the projects and documentation. Thanks [@brianteeman](https://github.com/brianteeman)!
>  - Refactored and improved the logic across utilities for bringing a window to the foreground after activation.
>
> ### Color Picker
>
>  - After activating Color Picker, it's now possible to cancel the session by clicking the right mouse button. Thanks [@fredso90](https://github.com/fredso90)!
>
> ### Environment Variables
>  - Added a new utility: An environment variables editor that has the functionality to configure profiles that can be enabled/disabled. Thanks [@niels9001](https://github.com/niels9001) for the design and UI work that made this possible!
>  - Shows in the title bar if it's running as an administrator. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### FancyZones
>
>  - Fixed an issue causing context menu pop-ups from some apps to automatically snap to a zone. (This was a hotfix for 0.74)
>  - Applied the fix for the context menu pop-ups to the logic that decides which windows can be snapped.
>  - Reworked the "Keep windows in their zones" option to include the work area and turn it on by default, fixing an incompatibility with the Copilot flyout.
>  - Fixed an issue causing windows to be snapped while moving to a different virtual desktop.
>
> ### File Explorer add-ons
>
>  - Fixed an issue blocking some SVG files from being previewed correctly. (This was a hotfix for 0.74)
>  - Fixed crashes on invalid files in the STL Thumbnail generator.
>
> ### GPO
>
>  - Added a global GPO rule that applies for all utilities unless it's overridden. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Added GPO rules to control which PowerToys Run plugins should be enabled/disabled by policy. Thanks [@htcfreek](https://github.com/htcfreek)!
>    * All plugins have to provide its plugin ID as static property in its Main method.
>
> ### Image Resizer
>
>  - Fixed wrong .bmp file association in the registry. Thanks [@meitinger](https://github.com/meitinger)!
>
> ### Keyboard Manager
>
>  - Visually distinguish between the Numpad and regular period characters in the UI.
>  - This utility is now disabled by default on new installations, since it requires user configuration to affect keyboard behavior.
>  - Fixed a typo in the Numpad Subtract key in the editor.
>
> ### Mouse Highlighter
>
>  - Removed the lower limit of fade delay and duration, to allow better signaling of doing a double click. Thanks [@fredso90](https://github.com/fredso90)!
>
> ### Mouse Jump
>
>  - The process now runs in the background, for a faster activation time. Thanks [@mikeclayton](https://github.com/mikeclayton)!
>
> ### Peek
>
>  - Reported file sizes will now more closely match what's reported by File Explorer. Thanks [@Deepak-Sangle](https://github.com/Deepak-Sangle)!
>  - Added a previewer that hosts File Explorer previewers to support every file type that a machine is currently able to preview. Thanks [@dillydylann](https://github.com/dillydylann)!
>  - Fixed an issue causing the preview of the first file to be stuck loading. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed showing the previously previewed video file when invoking Peek with a new file. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added the wrap and file formatting options to the Monaco previewer. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### PowerRename
>
>  - Save data from the last run in a different file to avoid conflicting with changing settings in the Settings application.
>
> ### PowerToys Run
>
>  - Fixed a case where the query wasn't being cleared after invoking a result action through the keyboard. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved the shell selection option for Windows Terminal in the Shell plugin and improved the backend code for adding combo box options to plugins. Thanks [@htcfreek](https://github.com/htcfreek)!
>    * The implementation of the combo box items has changed amd isn't backward compatible. (Old plugins won't crash, but the combo box setting isn't shown in settings ui anymore.)
>  - Added Unix time in milliseconds, fixed negative unix time input and improved error messages in the TimeDate plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - The PowerToys plugin allows calling the new Environment Variables utility. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Refactored and added support to VSCodium Stable, VSCodium Insider and Remote Tunnels workspaces. Thanks [@eternalphane](https://github.com/eternalphane)!
>
> ### Quick Accent
>
>  - Fixed characters that were removed from "All languages" because they were not in any single language. (This was a hotfix for 0.74)
>  - Added Asturian characters to the Spanish character set. Thanks [@blakestack](https://github.com/blakestack)!
>  - Added Greek characters with tonos. Thanks [@PesBandi](https://github.com/PesBandi)!
>
> ### Registry Preview
>
>  - Fixed a parsing error that crashed the Application. (This was a hotfix for 0.74)
>  - Fixed opening file names with non-ASCII characters. Thanks [@randyrants](https://github.com/randyrants)!
>  - Fixed wrong parsing when the file contained an assignment with spaces around the equals sign. Thanks [@randyrants](https://github.com/randyrants)!
>  - Fixed key transversal issues when a key was a substring of a parent key. Thanks [@randyrants](https://github.com/randyrants)!
>
> ### Runner
>
>  - Fixed the update notification toast to show an Unicode arrow. Thanks [@TheJoeFin](https://github.com/TheJoeFin)!
>
> ### Settings
>
>  - Added a new Dashboard home page, with quick access for enabling modules, short descriptions and activation methods. Thanks [@niels9001](https://github.com/niels9001) for the design and UI work that made this possible!
>  - Fixed a typo in the Hosts File Editor page. Thanks [@Deepak-Sangle](https://github.com/Deepak-Sangle)!
>  - Added a lock icon to the flyout listing of all modules when its enabled state is controlled by policy.
>  - The "All apps" list in the flyout will now list all apps even if their enabled state is controlled by policy.
>
> ### Video Conference Mute
>
>  - Added an option to allow for the toolbar to hide after some time passed. Thanks [@quyenvsp](https://github.com/quyenvsp)!
>  - Added an option to select to mute or unmute at startup. Thanks [@quyenvsp](https://github.com/quyenvsp)!
>  - Fixed an issue causing a cascade of mute/unmute triggers.
>
> ### Documentation
>
>  - Updated the Group Policy documentation on learn.microsoft.com, removed the Group Policy documentation from the repository and linked to the published documentation on learn.microsoft.com instead.
>
> ### Development
>
>  - Added project dependencies to the version project and headers to avoid building errors. Thanks [@johnterickson](https://github.com/johnterickson)!
>  - Enabled Control Flow Guard in the C++ projects. Thanks [@DHowett](https://github.com/DHowett)!
>  - Switched the release pipeline to the 1ES governed template. Thanks [@DHowett](https://github.com/DHowett)!
>  - Styled XAML files and added a XAML Style checker to the solution, with a CI action to check if code being contributed is compliant. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Suppressed redundant midl file warnings in PowerRename.
>  - Add unit tests to FancyZones Editor. Thanks [@garv5014](https://github.com/garv5014), [@andrewbengordon](https://github.com/andrewbengordon) and [@Cwighty](https://github.com/Cwighty)!
>  - Improved the Default Layouts internal structure in FancyZones Editor. Thanks [@garv5014](https://github.com/garv5014)!
>  - Fixed code issues to allow building in Visual Studio 17.8 Preview 4.
>

___

# v0.74.1		03/10/2023

> This is a patch release to fix issues in v0.74.0 to fix some bugs we deemed important for stability based on incoming rates. See [v0.74.0](https://github.com/microsoft/PowerToys/releases/tag/v0.74.0) for full release notes.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.1/PowerToysUserSetup-0.74.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.1/PowerToysUserSetup-0.74.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.1/PowerToysSetup-0.74.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.1/PowerToysSetup-0.74.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.74.1-x64.exe][ptUserX64] | 748BF7BA33913237D36D6F48E3839D0C8035967305137A17DEFF39D775735C81 |
> | Per user - ARM64     | [PowerToysUserSetup-0.74.1-arm64.exe][ptUserArm64] | F5DAA89A9CF3A2805E121085AFD056A890F241A170FAB5007AA58E2755C88C54 |
> | Machine wide - x64   | [PowerToysSetup-0.74.1-x64.exe][ptMachineX64] | 298C6F4E4391BDC06E128BED86A303C3300A68EAF754B4630AF7542C78C0944A |
> | Machine wide - ARM64 | [PowerToysSetup-0.74.1-arm64.exe][ptMachineArm64] | A65F3C300A48F9F81312B7FC7B306382CB87F591612D0CEC7E5C0E47E868904B |
>
> ## Highlights
>
> - #28785 - After trying to calculate the All Languages charset in Quick Accent by joining the charsets of all languages, we missed that some characters were not in any of the languages. This adds those characters to All Languages again.
> - #28798 - Fixes a parsing issues when converting binary data from the registry files in Registry Preview.
> - #28914 - Fixes FancyZones automatically snapping context menus on some applications.
> - #28819 - Fixes SVG Preview ignoring some files with comments after we optimized the file parsing code.
>
>

___

# v0.74.0		26/09/2023

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F48
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F47
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.0/PowerToysUserSetup-0.74.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.0/PowerToysUserSetup-0.74.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.0/PowerToysSetup-0.74.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.74.0/PowerToysSetup-0.74.0-arm64.exe
>
> In the [v0.74 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.74.0-x64.exe][ptUserX64] | 1C4ECE9F11488BAFFAE6B76D2B0504FA18BFFEA11EBC38BCC87F5D86AEA87C7C |
> | Per user - ARM64     | [PowerToysUserSetup-0.74.0-arm64.exe][ptUserArm64] | 4F3842FAB0839A361A15A06B7720BA8A0FE7F9AF98EA94245C08DEF37678CA4A |
> | Machine wide - x64   | [PowerToysSetup-0.74.0-x64.exe][ptMachineX64] | 648992E8CEA08F3C63C7CCBD554ADDF500ECBC4560187310BC12E6CB9C2F38E3 |
> | Machine wide - ARM64 | [PowerToysSetup-0.74.0-arm64.exe][ptMachineArm64] | 2B6D92F1A0EA688C7EE882050AC9B030C8B3A18765163FB6D67E5E694A4D4FE3 |
>
> ## Highlights
>
>  - Upgraded to Windows App SDK 1.4.1, increasing stability of WinUI3 utilities. Thanks [@dongle-the-gadget](https://github.com/dongle-the-gadget) for starting the upgrade!
>  - Text Extractor was upgraded to its version 2.0, with a new overlay, table mode and more Quality of Life improvements. Thanks [@TheJoeFin](https://github.com/TheJoeFin)!
>  - Improved FancyZones stability, fixing some layout resets and improving handling of newly created windows on Windows 11.
>  - Fixed many silent crashes that were reported to Watson and the user's event viewer.
>
> ### General
>
>  - Turning animations off in Windows Settings will now also turn them off in PowerToys.
>  - Upgraded the Windows App SDK dependency to 1.4.1. Thanks [@dongle-the-gadget](https://github.com/dongle-the-gadget) for the original 1.4.0 upgrade!
>  - Show in the thumbnail label and application titles when running as administrator. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Upgraded the Win UI Community Toolkit dependency to 8.0. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Awake
>
>  - Added down-sampled variants to the application's icon. Thanks [@morriscurtis](https://github.com/morriscurtis)!
>
> ### Color Picker
>
>  - After adding a new color in the editor, the history will scroll the new color into view. Thanks [@peerpalo](https://github.com/peerpalo)!
>
> ### Crop and Lock
>  - Fixed a Crop and Lock crash that would occur when trying to reparent a window crashes the target application. An error message is shown instead.
>
> ### FancyZones
>
>  - Set the process and main thread priority to normal.
>  - Fixed handling newly created windows on Windows 11.
>  - Fixed scenarios where opening the FancyZones Editor would reset the layouts.
>
> ### File Explorer add-ons
>
>  - Optimized CPU usage for generating SVG thumbnails.
>  - Improved handling of Gcode Thumbnails, including JPG and QOI formats. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>  - Better handled errors when sending telemetry, which were causing reported crashes.
>  - Fixed some thumbnails not being shown centered like before the optimization.
>
> ### File Locksmith
>
>  - Shows files opened by processes with PID greater than 65535. Thanks [@poke30744](https://github.com/poke30744)!
>  - Fixed a GDI object leak in the context menu which would crash Explorer.
>
> ### Find My Mouse
>
>  - Added new activation methods, including by hotkey. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Hosts File Editor
>
>  - Ignore the default ACME sample entries in the hosts file. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Improved save error handling and added better error messages. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Corrected a check for an error when signaling the application to start as administrator.
>  - Refactored the context menu. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed dialogs overlapping the title bar after the upgrade to Windows App SDK 1.4. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Keyboard Manager
>
>  - Distinguish between the regular minus key and the numpad minus key.
>
> ### Mouse Without Borders
>
>  - Fixed a crash when trying to restart the application.
>
> ### Peek
>
>  - Using Peek on HTML files will show a white background by default, similar to a browser's default behavior.
>  - Fix a white flash on Dark theme when switching file and improved the development file preview detection and adjustments.
>
> ### PowerRename
>
>  - Fixed a crash caused by big counter values on the new enumeration method.
>
> ### PowerToys Run
>
>  - It's now possible to select which shell is used by the Shell plugin.
>  - A combobox option type was added to the plugin options.
>  - Fixed a bug in the Calculator plugin that was causing decimal numbers to be misinterpreted on locales where the dot (`.`) character isn't used as a decimal or digit separator.
>  - Improved the Program plugin stability when it fails to load a program's thumbnail at startup.
>  - The use of Pinyin for querying some plugins can now be turned on in Settings. Thanks [@ChaseKnowlden](https://github.com/ChaseKnowlden)!
>  - Refactored option types for plugin and added number, string and composite types to be used in the future. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed the entry for searching for Windows updates in the Settings plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Quick Accent
>
>  - The "All languages" character set is now calculated by programmatically querying the characters for every available language. Thanks [@dannysummerlin](https://github.com/dannysummerlin)!
>  - Added é to the Norwegian and Swedish languages. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Added a runtime cache to the "All languages" character set, to only calculate accents once per key.
>
> ### Registry Preview
>
>  - Fixed focusing issues at startup.
>  - Improved the data visualization to show data in a similar way to the Windows Registry Editor. Thanks [@dillydylann](https://github.com/dillydylann)!
>
> ### Runner
>
>  - Fixed hanging when a bug report was generated from the flyout. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Settings
>
>  - Improved the way the OOBE window reacts to Windows theme change.
>  - Fixed an issue that made it impossible to change the "Switch between windows in the current zone" "Next window" shortcut for FancyZones.
>  - Fixed a crash when entering a duplicate name for a color in the Color Picker page and improved clean up when cancelling a color edit. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Text Extractor
>
>  - Text Extractor 2.0, with a new overlay, table mode and more Quality of Life improvements. Thanks [@TheJoeFin](https://github.com/TheJoeFin)!
>
> ### Documentation
>
>  - SECURITY.md was updated from 0.0.2 to 0.0.9. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Improved the README and main development document for clarity and completeness. Thanks [@codeofdusk](https://github.com/codeofdusk) and [@aprilbbrockhoeft](https://github.com/aprilbbrockhoeft)!
>
> ### Development
>
>  - Fixed PowerToys Run DateTime plugin tests that were failing depending on locale, so that they can be run correctly on all dev machines.
>  - Fixed PowerToys Run System plugin tests that were failing for certain network interfaces, so that they can be run correctly on all dev machines. Thanks [@snickler](https://github.com/snickler)!
>  - Fixed a markdown bug on the GitHub /helped command.
>  - Switched build pipelines to a new agent pool. Thanks [@DHowett](https://github.com/DHowett)!
>  - New .cs files created in Visual Studio get the header added automatically. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>

___

# v0.73.0		31/08/2023

> [github-next-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F47
> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F46
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.73.0/PowerToysUserSetup-0.73.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.73.0/PowerToysUserSetup-0.73.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.73.0/PowerToysSetup-0.73.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.73.0/PowerToysSetup-0.73.0-arm64.exe
>
> In the [v0.73 release cycle][github-current-release-work], we focused on new features, stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.73.0-x64.exe][ptUserX64] | BA55D245BDD734FD6F19803DD706A3AB8E0ABC491591195534997CF2122D3B7E |
> | Per user - ARM64     | [PowerToysUserSetup-0.73.0-arm64.exe][ptUserArm64] | FBFA40EA5FFA05236A7CCDD05E5142EE0C93D7485B965784196ED9B086BFEBF4 |
> | Machine wide - x64   | [PowerToysSetup-0.73.0-x64.exe][ptMachineX64] | 7FDA06292C7C2E6DA5AEF88D8E9D3DE89D331E9E356A232289F9B37CE4503894 |
> | Machine wide - ARM64 | [PowerToysSetup-0.73.0-arm64.exe][ptMachineArm64] | 4260AA30A1F52F194EE07E9E7ECD9E9F4CF35289267F213BC933F7A5191AC17C |
>
> ## Highlights
>
>  -  Keyboard manager now supports Numpad. Note, with previously bound hotkeys stored in settings.json would only react to non-Numpad keys now. If a user wishes to restore the previous behavior, it could be done by manually adding another binding for the Numpad variant.
>  - New utility: Crop And Lock allows you to crop a current application into a smaller window or just create a thumbnail. Focus the target window and press the shortcut to start cropping.
>  - FancyZones code improvements and refactor.
>  - Modernized ImageResizer UX.
>  - PowerRename advanced counter functionality.
>
>  ### General
>
>  - Added missing CoUninitialize call in elevation logic. Thanks [@sredna](https://github.com/sredna)!
>  - New utility: Crop And Lock. Thanks [@robmikh](https://github.com/robmikh)! and [@kevinguo305](https://github.com/kevinguo305)!
>  - Added new /helped fabric bot command to GitHub repo. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Fixed crashes caused by invalid settings. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Always On Top
>
>  - Added border transparency.
>
> ### FancyZones
>
>  - Fixed issue causing canvas zones being drawn only when dragging in zone area.
>  - Fixed user-defined default layout highlighting issue.
>  - Refactored and improved code quality.
>  - Fixed issue causing wrong layout to be applied when duplicating non-selected layout.
>
> ### File Locksmith
>
>  - Icon update. Thanks [@jmaraujouy](https://github.com/jmaraujouy)!
>
> ### File Explorer add-ons
>
>  - Fixed issue causing thumbnail previewers to lock files.
>  - Open URIs from developer files in default browser. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Installer
>
>  - Fixed PowerToys autorun after installing as SYSTEM user.
>  - Removed CreateScheduledTask custom action to handle task creation only from runner code.
>
> ### Image Resizer
>
>  -  Moved from ModernWPF to WpfUI to refresh and modernize UI/UX. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Keyboard Manager
>
>  - Rephrased labels to enhance clarity. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>  - Keyboard manager now supports Numpad. Note, with previously bound hotkeys stored in settings.json would only react to non-Numpad keys now. If a user wishes to restore the previous behavior, it could be done by manually adding another binding for the Numpad variant.
>
> ### Mouse Highlighter
>
>  - Fixed highlighter being invisible issue for Always on Top windows.
>  - Added settings for automatic activation on startup. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Mouse Pointer Crosshairs
>
>  - Added settings for automatic activation on startup. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Peek
>
>  - Show correct file type for shortcuts. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed issue causing wrong file size to be displayed. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Show 1 byte instead of 1 bytes file size. Thanks [@Deepak-Sangle](https://github.com/Deepak-Sangle)!
>  - Open URIs from developer files in default browser. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Show thumbnail and fallback to icon for unsupported files. Thanks [@pedrolamas](https://github.com/pedrolamas)!
>
> ### PowerRename
>
>  - Updated OOBE gif. Thanks [@ChaseKnowlden](https://github.com/ChaseKnowlden)!
>  - Localized renamed parts combo box.
>  - Introduced advanced counter functionality.
>  - Added remember last window size logic and optimized items sorting.
>  - Enable "Enumerate items" option by default.
>
> ### PowerToys Run
>
>  - Fixed issue causing original search to be abandoned when cycling through results.
>  - Updated device and bluetooth results for Settings plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed InvalidOperationException exception thrown. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Add Base64 Decoding function to the Value Generator plugin. Thanks [@LeagueOfPoro](https://github.com/LeagueOfPoro)!
>  - Added Keep shell open option for Shell plugin.
>  - Added Crop And Lock to PowerToys plugin. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Registry Preview
>
>  - Updated AppBarButtons to use an explicit AppBarButton.Icon. Thanks [@randyrants](https://github.com/randyrants)!
>  - Fixed crash on clicking Save As button.
>
> ### Runner
>
>  - Removed unneeded RegisterWindowMessage from tray icon logic. Thanks [@sredna](https://github.com/sredna)!
>  - Fixed startup looping issue.
>  - Improved old logs and installers cleanup logic. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Screen Ruler
>
>  - Use proper resources file.
>
> ### Settings
>
>  - Fixed issue causing problems with modifier keys and ShortcutControl. Thanks [@sh0ckj0ckey](https://github.com/sh0ckj0ckey)!
>  - Fixed crash when clicking "Windows color settings" link.
>  - Added support for launching Settings app directly.
>  - Fixed issue causing DisplayDescription not showing for PowerToys Run PluginAdditionalOption.
>  - Fixed issue causing FileLocksmith 'Show File Locksmith in' setting not showing correct value.
>  - Fixed issue causing Awake on/off toggle in Settings flyout not to work when Settings Awake page is opened.
>
> ### Documentation
>
>  - Added documentation for PowerToys Run third-party plugins. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed broken links in keyboardmanagerui.md. Thanks [@shubhsardana29](https://github.com/shubhsardana29)!
>  - Updated core team in COMMUNITY.md.
>  - Fixed broken links in ui-architecture.md. Thanks [@SamB](https://github.com/SamB)!
>  - Updated community.valuegenerator.md with Base64DecodeRequest description.
>
> ### Development
>
>  - Updated test packages and StyleCop. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Condense NuGet Restore into MSBuild Tasks. Thanks [@snickler](https://github.com/snickler)!
>
>

___

# v0.72.0		01/08/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=project%3Amicrosoft%2FPowerToys%2F45
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.72.0/PowerToysUserSetup-0.72.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.72.0/PowerToysUserSetup-0.72.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.72.0/PowerToysSetup-0.72.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.72.0/PowerToysSetup-0.72.0-arm64.exe
>
> In the [v0.72 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.72.0-x64.exe][ptUserX64] | 9925894D797458C78A8C3DF6FE4BD748580638B01BB43680477763662915109A |
> | Per user - ARM64     | [PowerToysUserSetup-0.72.0-arm64.exe][ptUserArm64] | 2E68139C22C56648E64514E4E8E0A0D12882F6CF30B48EB20ECC66B4CCDD5909 |
> | Machine wide - x64   | [PowerToysSetup-0.72.0-x64.exe][ptMachineX64] | 788EE4D828169F092737A739030B218CEFEC79583E42858BB8F9F036B701BE6F |
> | Machine wide - ARM64 | [PowerToysSetup-0.72.0-arm64.exe][ptMachineArm64] | 39C1D430A538B0F3D7869D39DF7F636A64AAFAD8DFB3C82059A97F4EBD3369C4 |
>
> ## Highlights
>
>  - Greatly reduced the PowerToys installed space by having utilities share the same installed path. When compared to 0.71, the 0.72 x64 machine installed version of PowerToys reduces the size reported in the Installed Apps screen from 1.15GB to 785 MB and the size in File Explorer properties for the installation folder from 3.10GB to 554 MB.
>  - Value Generator - A new PowerToys Run plugin that generates hashes and GUID values. Thanks [@IHorvalds](https://github.com/IHorvalds)!
>  - Mouse Highlighter has a new feature to have a highlight always follow the mouse pointer. Thanks [@hayatogh](https://github.com/hayatogh)!
>  - PowerRename was reworked to support a bigger number of files without crashing.
>
> ### Known issues
>
>  - Due to changing paths in the installation folder, the Mouse Without Borders service might be pointing to the wrong place. Users not running as admin will have to enable service mode again after install. A toast notification will appear if Mouse Without Borders is unable to start the service correctly.
>  - File Explorer extensions changed paths might not be loaded correctly until File Explorer and Preview Host processes are restarted, so we advise restarting the computer when possible after updating PowerToys.
>
>  ### General
>
>  - Shared dependencies between applications in order to greatly reduce the installed size.
>  - Added missing icons and icon sizes. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### FancyZones
>
>  - Fixed an issue where FancyZones wouldn't register a change to the "Switch between windows in the current zone" setting.
>  - Added a Setting to enable the behavior of clicking the middle mouse button to toggle multiple zone spanning.
>
> ### File Locksmith
>
>  - Fixed a File Explorer crash when deleting a file, updating PowerToys and then trying to right-click the background of a folder in File Explorer.
>  - UI tweaks. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>
> ### File Explorer add-ons
>
>  - Updated the Monaco dependency for Developer Files Preview, supporting new file extensions and fixing issues. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>
> ### Hosts File Editor
>
>  - Consolidated the way the Hosts application is launched. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - UI tweaks. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>
> ### Installer
>
>  - Refactored the Monaco dependency inclusion. What to install is now being generated automatically.
>  - Removed hardlinks and simplified the installer files, now that many utilities use the same paths.
>
> ### Mouse Highlighter
>
>  - Added a feature so that a highlight follows the mouse even if no mouse button is being pressed. Thanks [@hayatogh](https://github.com/hayatogh)!
>
> ### Mouse Pointer Crosshairs
>
>  - Added a setting to hide the crosshairs when the mouse pointer is also hidden. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added a setting to select a fixed length for the crosshairs, which also spans across screens. Thanks [@Epp-code](https://github.com/Epp-code)!
>
> ### Mouse Without Borders
>
>  - Switched to a UWP mouse input API to fix mouse pointer lag issues that were reported.
>  - A toast notification will appear when the service can't be started and Mouse Without Borders will try to start in non-service mode instead.
>  - Fixed a bug where the service path wouldn't update to the new binary path when trying to re-enable service mode.
>  - Fixed some grammar errors in the Mouse Without Borders user facing strings. Thanks [@KhurramJalil](https://github.com/KhurramJalil)!
>  - Allow changing the shortcuts in the same way as other utilities and changed them to better defaults to avoid conflicting with Alt Gr+letter combos on international layouts.
>
> ### Peek
>
>  - Also benefits from the Monaco dependency update when peeking into files supported by the Developer Files Preview. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Fixed a flash on PowerToys starting due to the Peek window activating and hiding right away. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Updated icon design. Thanks [@niels9001](https://github.com/niels9001)!
>  - Fixed flipped content issues on systems with RTL languages.
>
> ### PowerRename
>
>  - Reworked the UI and resource consumption to fix crashes and hangs when trying to rename a huge number of files.
>  - Added the Mica background material and some UI tweaks. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### PowerToys Run
>
>  - New plugin: Value Generator - generates values like hashes and GUIDs. Thanks [@IHorvalds](https://github.com/IHorvalds)!
>  - The default input smoothing values were changed to the recommended values. Thanks [@SamMercer172](https://github.com/SamMercer172)!
>  - Fixed tab navigation issues when using Shift+Tab to go backwards. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed a crash caused by images not being found in the image cache due to racing conditions.
>  - Fixed synchronization issues in the WindowWalker plugin. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed a synchronization crash when getting localized system paths.
>  - The PowerToys plugin is now activated by default. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Quick Accent
>  - Added the ("ḍ", U+1E0D) unicode character. Thanks [@SamMercer172](https://github.com/SamMercer172)!
>  - Fixed an issue causing the left and right keys being discarded even when Quick Accent didn't activate.
>
> ### Registry Preview
>
>  - Fixed a bug causing DWORD values to not be shown correctly. Thanks [@randyrants](https://github.com/randyrants)!
>  - UI tweaks. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>
> ### Runner
>
>  - Show a warning asking the user to restart the computer after updating the PowerToys version.
>
> ### Screen Ruler
>
>  - UI tweaks. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>
> ### Settings
>
>  - Fix an unused Expander in the File Locksmith settings page.
>  - Added an info box to better explain what the extended context menu is.
>
> ### Development
>
>  - Projects were restructured to allow sharing the same folder and dependencies and to avoid resource name conflicts.
>  - Added scripts to CI to guard against applications having conflicting resources.
>  - Added scripts to CI to guard against depending on different versions of the same dependency.
>  - Test projects now build to a separate path.
>  - Dependencies updated across the solution to ensure every project is using the same dependencies.
>
>

___

# v0.71.0		05/07/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F44
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.71.0/PowerToysUserSetup-0.71.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.71.0/PowerToysUserSetup-0.71.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.71.0/PowerToysSetup-0.71.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.71.0/PowerToysSetup-0.71.0-arm64.exe
>
> In the [v0.71 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.71.0-x64.exe][ptUserX64] | 4C6CCB3055E3838DA50FF529A670BAAD129570F4BFABF497B5D92259D3052794 |
> | Per user - ARM64     | [PowerToysUserSetup-0.71.0-arm64.exe][ptUserArm64] | 48633758DFBB99DE34BA2D3E3F294A60EF7E01015296D29A884251068B6FE3F6 |
> | Machine wide - x64   | [PowerToysSetup-0.71.0-x64.exe][ptMachineX64] | 44F092DFAC002536A27ABC701750D8C78FF30F8879768990BC4A0AFD0D5119F1 |
> | Machine wide - ARM64 | [PowerToysSetup-0.71.0-arm64.exe][ptMachineArm64] | 283A67539EDA5D3AD88735C7B0150852ECB57D569BAC80396F942C60D6ACB33F |
>
> ## Highlights
>
>  - Support previewing archive files with Peek. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed PT Run crash caused by missing App.Dark.png file.
>  - Added setting to set Registry Preview as default app for opening .reg files. Thanks [@randyrants](https://github.com/randyrants)!
>  - Modernized Settings app title bar and styling (Mica background material) to be inline with Windows 11 guidelines. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### General
>
>  - Fixed infinite loop issue caused by global event not being reset. (This was a hotfix for 0.70)
>  - Bump CommunityToolkit.Mvvm package version to 8.2.0. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed issue causing runner lag by moving check for updates and running bug report logic to the background thread.
>  - Bump WinUIEx package version to 2.2. Thanks [@niels9001](https://github.com/niels9001)!
>  - Fixed issue causing Settings app crash when launching a second app process. Thanks [@BLM16](https://github.com/BLM16)!
>  - Fixed network errors when checking for updates on virtual machines.
>  - Bump Microsoft.CodeAnalysis.NetAnalyzers package version to 7.0.3. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Bump Microsoft.Windows.Compatibility package version to 7.0.3.
>  - Bump System.Management package version to 7.0.2.
>  - Fixed issue causing PowerToys to start with Below Normal priority on startup.
>
> ### ColorPicker
>
>  - Store color history in a separated file.
>
> ### FancyZones
>
>  - Added feature to use middle click to toggle multiple zones spanning. Thanks [@BasitAli](https://github.com/BasitAli)!
>  - Fixed issue causing zoning not to happen until the cursor is moved.
>  - Improved monitor identification logic to mitigate issues causing layout reset.
>  - Fixed issue where default layout was applied instead of blank layout.
>
> ### File Locksmith
>
>  - Added setting to show only in extended context menu.
>
> ### File Explorer add-ons
>
>  - Developer files preview support for .vsconfig, .sln, .vcproj, .vbproj, .fsproj and .vcxproj files. (This was a hotfix for 0.70)
>  - Developer files preview support .vbs, .inf, .gitconfig, .gitattributes and .editorconfig files. (This was a hotfix for 0.70) Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Changed order of developer files preview` context menu items. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Developer files preview support for .gitignore files. (This was a hotfix for 0.70) Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Fixed issue causing preview pane flickering on file selection and resizing. Thanks [@tanchekwei](https://github.com/tanchekwei)!
>
> ### Hosts
>
>  - Improved UX by adding keyboard shortcuts. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added setting to select the file encoding. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed parsing of commented lines with an address and host in the middle of the comment. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed issue on adding first entry and improve empty hosts list UI. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added logic to handle more than 9 hosts per entry (Windows limitation) by splitting them into separate entries. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### ImageResizer
>
>  - Added Enter key event handler when setting width/height of the new custom size.
>
> ### Installer
>
>  - Fixed PowerToys Plugin installation. (This was a hotfix for 0.70) Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fixed issue causing missing Mouse Without Borders service after upgrade. (This was a hotfix for 0.70)
>  - Removed unneeded PT Run registry entries.
>
> ### Mouse Without Borders
>
>  - Added Name2IP setting. (This was a hotfix for 0.70)
>  - Fixed device layout issues. (This was a hotfix for 0.70)
>  - Fixed hiding cursor at the top of the screen when "Hide mouse at the screen edge" is enabled. (This was a hotfix for 0.70)
>  - Fixed issue that was preventing OS going to sleep mode. (This was a hotfix for 0.70)
>  - Remove shortcut for deprecated VKMap functionality. (This was a hotfix for 0.70) Thanks [@dtaylor84](https://github.com/dtaylor84)!
>  - Make MWB work without service if service doesn't start properly. (This was a hotfix for 0.70)
>  - Fixed focus issue causing "Hide mouse at the screen edge" not to work properly. (This was a hotfix for 0.70)
>  - Fixed issue causing app to hijack shortcut keys if they are only partially matched.
>
> ### Peek
>
>  - Consume Ctrl+Space shortcut only if Desktop or Shell are in the foreground. (This was a hotfix for 0.70)
>  - Added feature to hide window with Esc key. (This was a hotfix for 0.70) Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Added a setting to always run not-elevated (enabled by default). (This was a hotfix for 0.70)
>  - Support .vsconfig, .sln, .vcproj, .vbproj, .fsproj and .vcxproj files. (This was a hotfix for 0.70)
>  - Fixed blinking issue while loading developer files. (This was a hotfix for 0.70)
>  - Reset preview Source on Peek window close. (This was a hotfix for 0.70)
>  - Center Peek window on File Explorer activated monitor. (This was a hotfix for 0.70) Thanks [@SamChaps](https://github.com/SamChaps)!
>  - Fix previewing unsupported file types by using effective pixels. (This was a hotfix for 0.70) Thanks [@SamChaps](https://github.com/SamChaps)!
>  - Support .vbs, .inf, .gitconfig, .gitattributes and .editorconfig files. (This was a hotfix for 0.70) Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Fixed memory leak by clearing generated thumbnails. (This was a hotfix for 0.70)
>  - Added setting to close on focus lost. (This was a hotfix for 0.70)
>  - Fixed crash when triggering Peek with no files being selected. (This was a hotfix for 0.70)
>  - Fixed setting Peek window as a foreground window. (This was a hotfix for 0.70)
>  - Fixed race condition causing low quality preview to be displayed even if high quality preview is present. (This was a hotfix for 0.70)
>  - Added support for .htm files.
>  - Fixed issue where title bar button colors were not update on Windows theme change.
>  - Added up/down arrow key item navigation. Thanks [@DanWiseProgramming](https://github.com/DanWiseProgramming)!
>  - Improved UX by defining minimum window size and adding tooltips for shown text. Thanks [@htcfreek](https://github.com/htcfreek)!
>  - Fixed crash on previewing internet shortcuts files.
>  - Support previewing archive files. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### PowerToys Run
>
>  - Fixed crash caused by non thread-safe Results update.
>  - Fixed crash caused by missing App.Dark.png
>  - Code cleanup and fixed possible crash caused by missing VS Code instance in VS Code plugin. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>  - Fix environment helper for nested environment variables. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Quick Accent
>
>  - Added multiplication and division signs. Thanks [@ailintom](https://github.com/ailintom)!
>  - Added opening exclamation mark to Catalan and Spanish language. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>  - Added the section sign ("§", U+00A7). Thanks [@EikeJoo](https://github.com/EikeJoo)!
>  - Added accent units and more additional signs. Thanks [@WilkoLu](https://github.com/WilkoLu)!
>
> ### Registry Preview
>
>  - Added setting to set the app as default app for opening .reg files. Thanks [@randyrants](https://github.com/randyrants)!
>  - Merge settings to single folder.
>  - Fixed issue of saving files without truncation. Thanks [@qwerty472123](https://github.com/qwerty472123)!
>
> ### Text Extractor
>
>  - Various improvements and fixes. Thanks [@TheJoeFin](https://github.com/TheJoeFin)!
>
> ### Settings
>
>  - Styling fixes for Peek and Mouse Without Borders pages. (This was a hotfix for 0.70) Thanks [@niels9001](https://github.com/niels9001)!
>  - Fixed Mouse Without Borders machine connection status styling. (This was a hotfix for 0.70)
>  - Improved Mouse Without Border page Uninstall service UX when it is inaccessible. (This was a hotfix for 0.70)
>  - Updated File Explorer module screenshots and instructions to reflect the Windows 11 File Explorer. Thanks [@infinitepower18](https://github.com/infinitepower18)!
>  - Modernized the app title bar and styling (Mica background material) to be inline with Windows11 guidelines. Thanks [@niels9001](https://github.com/niels9001)!
>  - Improved error handling on settings backup failure.
>  - Added Reset button to shortcut control to reset activation shortcut to default value. Thanks [@Svenlaa](https://github.com/Svenlaa)!
>  - Improved Exclude apps setting for all modules to also detect apps by title.
>
> ### Development
>
>  - Added Peek and Mouse Without Borders to GitHub templates. (This was a hotfix for 0.70)
>  - Fixed the CI release pipelines winget package submission. (This was a hotfix for 0.70)
>  - Fixed process report and termination lists for Peek and Mouse Without Borders. (This was a hotfix for 0.70)
>  - Added Winget configuration file. (This was a hotfix for 0.70) Thanks [@ryfu-msft](https://github.com/ryfu-msft)!
>  - Fixed tests localization issues.
>  - Added Microsoft.VisualStudio.Component.VC.ATL library to .vsconfig.
>  - Onboarding to GitOps.ResourceManagement.

___

# v0.70.1		01/06/2023

> This is a patch release to fix issues in v0.70.0 to fix some bugs we deemed important for stability based on incoming rates. See [v0.70.0](https://github.com/microsoft/PowerToys/releases/tag/v0.70.0) for full release notes.
>
> *Warning:* Service mode in Mouse Without Borders might be disabled after upgrading and will need to be activated again.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.1/PowerToysUserSetup-0.70.1-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.1/PowerToysUserSetup-0.70.1-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.1/PowerToysSetup-0.70.1-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.1/PowerToysSetup-0.70.1-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.70.1-x64.exe][ptUserX64] | B8FD209310B9847DA3AC35C2C5A89F99CE5EA91F456D9D3595DD2840D62A1AC1 |
> | Per user - ARM64     | [PowerToysUserSetup-0.70.1-arm64.exe][ptUserArm64] | 9F267B7AD91E5FAE86ED5050A08A24756CE3EA9875FFCFDE195F1F4F299F0933 |
> | Machine wide - x64   | [PowerToysSetup-0.70.1-x64.exe][ptMachineX64] | 1BE4760558765EF363E12126282F1E3340A8ADFF657C5C51714F7E096F86EE50 |
> | Machine wide - ARM64 | [PowerToysSetup-0.70.1-arm64.exe][ptMachineArm64] | 5155EA186230876EF1DA6F49DC33E40D552B2BFFA0E03F66FBA71FBEB8713594 |
>
> ## Highlights
>
> - #26142, #26439 and #26525 - UX improvements in the Peek and Mouse Without Borders settings pages.
> - #26152 - The new PowerToys Run plugin for opening other PowerToys was missing some files in the installer and should work properly now.
> - #26235 - Peek and Mouse Without Borders process crashes in event viewer should now be reported correctly on Bug Reports.
> - #26150 - The Peek shortcut is now only captured if the active window is the Desktop, Explorer or Peek.
> - #26192 - Users can now use the Settings to specify host name and IP correlation in Mouse Without Borders, to account for VPN scenarios.
> - #24047 - Fixed a cause for possible leaks and/or infinite cycles in C# utilities that use events.
> - #26137 - The Peek windows can be closed using the Escape key.
> - #26181 - Created a setting for Peek to always run not elevated, so that it handles files in network shares correctly.
> - #26318, #26373 and #26431 - Peek and dev file preview now support showing Visual Studio project common files and .ini compatible files.
> - #26419 - Fixed a blinking issue when loading code files in Peek.
> - #26160 - Fixed a playback notification that would stick when using Peek on video files.
> - #26243 - Peek now tries to appear on the same monitor as the File Explorer window that triggers it.
> - #26133 - Fixed small Peek UI on high DPIs.
> - #26361, #26162 and #26478 - Reduced Peek memory usage and possible leaks.
> - #26246 - Fixed Mouse Without Borders layout always resetting to one row after some time.
> - #26366 - Added a setting to close the Peek window after it loses focus.
> - #26338 - Fixed the mouse activating thumbnails on top of the screen when switching to another machine in Mouse Without Borders.
> - #26470 - Fixed a silent Peek crash when trying to open it from File Explorer with no files selected.
> - #26261 - Fixed an issue causing Mouse Without Borders to prevent other connected machines from going to sleep.
> - #26454 - Disabled a deprecated shortcut in Mouse Without Borders that was interfering with other software.
> - #26517 - Don't remove the Mouse Without Borders service on upgrade. This issue will still affect users upgrading from 0.70.0 to a newer version, but it's fixed going forward.
> - #26521 - When Mouse Without Borders detects the service doesn't exist, it will still try to operate in the "no service" mode.
> - #26524 - Fixed a bug causing Mouse Without Borders to click a window on the current machine when switching to another machine.
> - #26259 - Added a winget-cli configuration file for PowerToys.
>

___

# v0.70.0		23/05/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F43
>
> In the [v0.70 release cycle][github-current-release-work], we focused on new features, stability and improvements.
>
> ## Installer Hashes
>
> [ptUserX64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.0/PowerToysUserSetup-0.70.0-x64.exe
> [ptUserArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.0/PowerToysUserSetup-0.70.0-arm64.exe
> [ptMachineX64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.0/PowerToysSetup-0.70.0-x64.exe
> [ptMachineArm64]: https://github.com/microsoft/PowerToys/releases/download/v0.70.0/PowerToysSetup-0.70.0-arm64.exe
>
> |  Description   | Filename | sha256 hash |
> |----------------|----------|-------------|
> | Per user - x64       | [PowerToysUserSetup-0.70.0-x64.exe][ptUserX64] | A2C45CC2EA953FF07246C85C7E106A403EAA5ED51E7C777452238EE280C9F6EA |
> | Per user - ARM64     | [PowerToysUserSetup-0.70.0-arm64.exe][ptUserArm64] | 2D9591A4752EFA7603C56EBDDC9CB4F70C5E938A4EF66ADC46B72FC60779E82C |
> | Machine wide - x64   | [PowerToysSetup-0.70.0-x64.exe][ptMachineX64] | 2DBE97E599AA4ED5363F2A15D581113BBF13035FE503D9E9ED71650B1CEE7771 |
> | Machine wide - ARM64 | [PowerToysSetup-0.70.0-arm64.exe][ptMachineArm64] | B0FA955D34C84F3FBF39335887F3BEB417A2094F7A06B9EEEB92B9A1000B258E |
>
> ## Highlights
>
> - New utility: Mouse Without Borders enables you to interact with other computers from the same keyboard and mouse and share clipboard and files between the machines. We’ve upgraded it to .NET 7 and made a few small adjustments to fit inside the PowerToys model. Thanks [@truong2d](https://github.com/truong2d) and the rest of the contributors from the Microsoft Garage!
> - New utility: Peek is a utility that shows a quick preview of files selected in File Explorer when you press a shortcut (`Ctrl`+`Space` by default). Thanks [@SamChaps](https://github.com/SamChaps)!
> - Registry preview Quality of Life improvements. Thanks [@randyrants](https://github.com/randyrants)!
> - Awake Quality of Life improvements. Thanks [@dend](https://github.com/dend)!
> - Mouse Jump Quality of Life improvements. Thanks [@mikeclayton](https://github.com/mikeclayton)!
>
> ### General
> - New utility: Mouse Without Borders. Thanks [@truong2d](https://github.com/truong2d) and [other original contributors](https://github.com/microsoft/PowerToys/blob/main/COMMUNITY.md#mouse-without-borders-original-contributors)!
> - New utility: Peek. Thanks [@SamChaps](https://github.com/SamChaps)!
> - Fixed a bug causing saved settings to clear sometimes when upgrading PowerToys.
> - Font, icon and corner radius adjustments in the UI across utilities. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
>
> ### Awake
>
> - Update to command line output to match the arguments. Thanks [@rpr69](https://github.com/rpr69) for creating a PR to help fix this.
> - Fix crash happening when setting an expiration date on time zones with a negative offset relative to UTC (This was a hotfix for 0.69).
> - Fix missing logging file when installing (This was a hotfix for 0.69).
> - Upgraded Awake to a new version, with Quality of Life improvements and fixing many issues regarding Awake not resetting or not keeping the computer awake when expected. Thanks [@dend](https://github.com/dend)!
>
> ### FancyZones
>
> - Fixed accessibility issues on the Editor.
>
> ### File Locksmith
>
> - Fixed tooltips having a transparent background (This was a hotfix for 0.69).
>
> ### File Explorer add-ons
>
> - Add a Setting to select a background for the SVG Preview. Thanks [@zanseb](https://github.com/zanseb)!
>
> ### Installer
>
> - Added more utilities to terminate when installing to help prevent files that sometimes are leftover from uninstall.
>
> ### Keyboard Manager
>
> - Fixed an issue causing mapping to media keys to type additional characters.
>
> ### Measure Tool
>
> - Created a setting to specify the default measure tool. Thanks [@zanseb](https://github.com/zanseb)!
>
> ### Mouse Jump
>
> - Reduced dependency on WinForms utility classes. Thanks [@mikeclayton](https://github.com/mikeclayton)!
> - Improved popup responsiveness. Thanks [@mikeclayton](https://github.com/mikeclayton)!
> - Added a setting to set a custom sized window. Thanks [@mikeclayton](https://github.com/mikeclayton)!
> - Added some shortcuts for screen navigation. Thanks [@mikeclayton](https://github.com/mikeclayton)!
>
> ### Peek
> - New utility: Peek. Thanks [@SamChaps](https://github.com/SamChaps), who drove the effort! Many thanks for all the contributors who made it possible: [@danielchau](https://github.com/danielchau), [@estebanm123](https://github.com/estebanm123), [@Joanna-Zhou](https://github.com/Joanna-Zhou), [@jth-ms](https://github.com/jth-ms), [@miksalmon](https://github.com/miksalmon), [@niels9001](https://github.com/niels9001), [@RobsonPontin](https://github.com/RobsonPontin), [@sujessie](https://github.com/sujessie), and [@Sytta](https://github.com/Sytta)!
>
> ### PowerToys Run
>
> - Add a plugin to start other PowerToys. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Added code to the Shell plugin to use Windows Terminal. Currently accessible only through manipulating the settings file directly. Thanks [@phoenix172](https://github.com/phoenix172)!
>
> ### Quick Accent
> - Added a missing character to the Welsh language.
>
> ### Registry Preview
>
> - Specify minimum size / position values for the UI (This was a hotfix for 0.69). Thanks [@randyrants](https://github.com/randyrants)!
> - Fixes in the UI command bar (This was a hotfix for 0.69). Thanks [@randyrants](https://github.com/randyrants)!
> - Fix crash on opening a file picker when running elevated (This was a hotfix for 0.69). Thanks [@randyrants](https://github.com/randyrants)!
> - Fixed tooltips having a transparent background (This was a hotfix for 0.69).
> - Fixed a file size limit typo. Thanks [@idma88](https://github.com/idma88)!
> - Improve hexadecimal value parsing. Thanks [@randyrants](https://github.com/randyrants)!
> - Added a button to open the Registry Editor at a selected key. Thanks [@randyrants](https://github.com/randyrants)!
> - Improve key and value parsing. Thanks [@randyrants](https://github.com/randyrants)!
> - Better theme support for caption bar. Thanks [@randyrants](https://github.com/randyrants)!
> - Fix an issue handling empty DWORD and QWORD values. Thanks [@randyrants](https://github.com/randyrants)!
>
> ### Settings
>
> - Update the What's New screen to hide the installer hashes in the new format (This was a hotfix for 0.69).
> - Fix crashes happening when using the Shortcut Control (This was a hotfix for 0.69).
> - The Settings window now has a minimum width. Thanks [@niels9001](https://github.com/niels9001)!
> - Prevent a second Settings instance from being opened on upgrade.
> - Fix accessibility issues on many pages. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Documentation
>
> - Fix a dead link in documentation that was pointing to the wrong settings specification. Thanks [@zanseb](https://github.com/zanseb)!
> - Added some missing contributors to COMMUNITY.md
>
> ### Development
>
> - Fixed the CI release pipelines hash generation (This was a hotfix for 0.69).
> - Added per-user installers to the winget package submission script.
> - Upgraded the Community Toolkit Labs dependency. Thanks [@niels9001](https://github.com/niels9001)!
> - Fixed building with Visual Studio 17.6. Thanks [@snickler](https://github.com/snickler)!
> - Upgraded the WebView 2 dependency.
> - Upgraded the WinAppSDK dependency to 1.3.1.
> - Fixed a typo preventing the clean up script to run. Thanks [@Sajad-Lx](https://github.com/Sajad-Lx)!
> - Fixed encoding on a test file to fix running tests in some configurations. Thanks [@VisualBasist](https://github.com/VisualBasist)!
> - Made the GPO release assets come named with a version in the build CI output.

___

# v0.69.1		12/04/2023

> This is a patch release to fix issues in v0.69.0 to fix some bugs we deemed important for stability based on incoming rates. See [v0.69.0](https://github.com/microsoft/PowerToys/releases/tag/v0.69.0) for full release notes.
>
> ## Installer Hashes
>
> ### Per user - x64 Installer Hash - PowerToysUserSetup-0.69.1-x64.exe
>
> 662E4082A788DF808BFB34C8D922C8B15835632808CE8C93DB7E13D9F2B984BA
>
> ### Per user - ARM64 Installer Hash - PowerToysUserSetup-0.69.1-arm64.exe
>
> CBD35E2F0DCEF16E902C6F5110224618E0E272B8AFDBA3468F7AD32978603A51
>
> ### Machine wide - x64 Installer Hash - PowerToysSetup-0.69.1-x64.exe
>
> 058A382779270FEFD262A55B573E0D6A8501771C7DC52F41993FC440B5DDE5FF
>
> ### Machine wide - ARM64 Installer Hash - PowerToysSetup-0.69.1-arm64.exe
>
> 02A3AFA2BB90D46BDFF93E0A6E855751455BB831A31A93A6B05DB8F0B6893E57
>
> ## Highlights
>
> - #25254 - Fixed an issue causing the Registry Preview window to adopt a size too big for the screen after opening a big file.
> - #25334 - Fixed a crash on the Settings application after selecting the new "Keep awake until experiation" option of Awake on timezones with a negative offset relative to UTC.
> - #25253 - Fix wrong menu and tooltip transparent background in Registry Preview.
> - #25273 - Add Ctrl+S and Shift+Ctrl+S as shortcuts for saving in Registry Preview.
> - #25284 - Fix Registry Preview icons on some systems.
> - #25252 - Hide the installer hashes of release notes in the What's New page.
> - #25398 - Add code to avoid crashes due to the recent Shortcut Control changes.
> - #25250 - Fix a crash when trying to open or save a file when Registry Preview is running with administrative privileges.
> - #25395 - Fix command line functionality for Awake.
>

___

# v0.69.0		06/04/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F42
>
> In the [v0.69 release cycle][github-current-release-work], we focused on new features, stability and improvements.  Early notice for v0.70, we will be releasing it later in May 2023.
>
> ## Installer Hashes
>
> ### Per user - x64 Installer Hash - PowerToysUserSetup-0.69.0-x64.exe
>
> D05FC31F137718516C1D792765AAFEC51551FC172064BC3EE170911E455053AC
>
> ### Per user - ARM64 Installer Hash - PowerToysUserSetup-0.69.0-arm64.exe
>
> 0B113EAF17EEEA86299DEF7F73E7AF15A9CBFB8033C7F9218E33BDA276F37727
>
> ### Machine wide - x64 Installer Hash - PowerToysSetup-0.69.0-x64.exe
>
> ADE1E56DF8CC52839FBD9DB6A0BCF34E38B88AAA52544EEAD3C1BB023FDAF29A
>
> ### Machine wide - ARM64 Installer Hash - PowerToysSetup-0.69.0-arm64.exe
>
> CF13F75F93462BE68CA3E4C90B23F3828BBCAA44E7AA987503E73652DCB9FDF8
>
> ## Highlights
>
> - New utility: Registry Preview is a utility to visualize and edit Windows Registry files. Thanks [@randyrants](https://github.com/randyrants)!
> - Support per-user scope installation.
> - Awake: Quality-of-life improvements and introduced keeping system awake until expiration time and date. Thanks [@dend](https://github.com/dend)!
> - PowerToys Run: Fix crashing issue caused by thumbnail image loading.
>
> ### General
>
> - New utility: Registry Preview. Thanks [@randyrants](https://github.com/randyrants)!
> - Fix issue causing folders to not be removed on uninstall.
> - Support per-user scope installation.
>    - Companies can control this using the new GPO.
>
> ### Awake
>
> - Quality-of-life improvements and introduced keeping system awake until expiration time and date. Thanks [@dend](https://github.com/dend)!
>
> ### Color Picker
>
> - Fix issue sampling timing and grid issue causing Color Picker to sample the color of its own grid. Thanks [@IHorvalds](https://github.com/IHorvalds)!
>
> ### FancyZones
>
> - Fix window cycling on multiple monitors issue.
>
> ### File Locksmith
>
> - Add context menu icon. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Mouse Utils
>
> - Mouse Jump - Simulate mouse input event on mouse jump in addition to cursor move.
> - Mouse Jump - Improve performance of screenshot generation. Thanks [@mikeclayton](https://github.com/mikeclayton)!
>
> ### Paste as Plain Text
>
> - Support Ctrl+V as activation shortcut. (This was a hotfix for 0.67)
> - Repress modifier keys after plain paste. (This was a hotfix for 0.67) Thanks [@UnderKoen](https://github.com/UnderKoen)!
> - Set default shortcut to Ctrl+Win+Alt+V. (This was a hotfix for 0.67)
> - Update icons. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### PowerRename
>
> - Show PowerRename in directory background context menu.
> - Fix the crash on clicking Select/UnselectAll checkbox while showing only files to be renamed.
> - Improve performance on populating Renamed items when many items are being renamed.
>
> ### PowerToys Run
>
> - Add setting to disable thumbnails generation for files. (This was a hotfix for 0.67)
> - Calculator plugin - handle implied multiplication expressions. Thanks [@jjavierdguezas](https://github.com/jjavierdguezas)!
> - Fix Calculator plugin unit tests to respect decimal separator locale. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Fix crashing caused by thumbnail image loading.
> - Date & Time plugin - Add filename-compatible date & time format. Thanks [@Picazsoo](https://github.com/Picazsoo)!
> - Improved the error message shown on plugin loading error. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Quick Accent
>
> - Fix existing and add missing Hebrew and Pinyin characters. Thanks [@stevenlele](https://github.com/stevenlele)!
>
> ### Registry Preview
>
> - Added a new utility: Registry Preview.
> - Thanks [@htcfreek](https://github.com/htcfreek)! for the help shipping this utility!
> - Thanks [@niels9001](https://github.com/niels9001) for the help on the UI!
>
> ### Video Conference Mute
>
> - Add toolbar DPI scaling support.
> - Fix selecting overlay image when Settings app is running elevated.
> - Add push-to-talk (and push-to-reverse) feature. Thanks [@pajawojciech](https://github.com/pajawojciech)!
>
> ### Settings
>
> - Fix Experiment bitmap icon rendering on theme change and bump CommunityToolkit.Labs.WinUI.SettingsControls package version. Thanks [@niels9001](https://github.com/niels9001)!
> - Video Conference Mute page improvements. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
> - Add warning that PowerToys Run might get no focus if "Use centralized keyboard hook" settings is enabled. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Fix ShortcutControl issues related to keyboard input focus, theme change and missing error badge when invalid key is pressed. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Add warning when Ctrl+V and Ctrl+Shift+V is used as an activation shortcut for Paste as Plain Text. Thanks [@htcfreek](https://github.com/htcfreek)!
>
> ### Documentation
>
> - Update CONTRIBUTING.md with information about localization issues. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Remove localization from URLs. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
> - Add dev docs for tools. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
>
> ### Development
>
> - Ignore spellcheck for MouseJumpUI/MainForm.resx file. (This was a hotfix for 0.67)
> - Optimize versionAndSignCheck.ps1 script. Thanks [@snickler](https://github.com/snickler)!
> - Upgraded NetAnalyzers to 7.0.1. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Move all DLL imports in Settings project to NativeMethods.cs file.
> - Fix FancyZones tools build issues. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Centralize Logger used in C# projects. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Add missing project references. Thanks [@ACGNnsj](https://github.com/ACGNnsj)!
>

___

# v0.68.1		07/03/2023

> This is a patch release to fix issues in v.068.0 to fix some bugs we deemed important for stability based on incoming rates. See [v0.68.0](https://github.com/microsoft/PowerToys/releases/tag/v0.68.0) for full release notes.
>
> - #24446 - Support Ctrl+V as activation shortcut for Paste As Plain Text, as requested by some users. **Warning**: Overriding a default system shortcut might have unintended consequences.
> - #24437 - Paste As Plain Text - Support pasting multiple times as Ctrl+V does (on pressing activation key while holding modification keys)
> - #24491 - Set Paste As Plain Text default shortcut to Ctrl+Win+Alt+V to avoid conflicting with new Windows volume mixer shortcut.
> - #24600 - Add PowerToys Run setting to disable thumbnails generation for files in order to narrow down the root cause of the crash.
>
> ## Installer Hashes
>
> ### x64 Installer Hash
>
> ECBEED67EFA864E558403F719B7FFD6F0192E77C36579B2FF9C2A0B6DD305752
>
> ### ARM64 Installer Hash
>
> 3F568BF24E78D855B529D25CAB510A2589B8371D76B5BE3BCEBBD4C7B9043D99

___

# v0.68.0		01/03/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F41
>
> In the [v0.68 release cycle][github-current-release-work], we focused on releasing new features and improvements.
>
> ## Installer Hashes
>
> ### x64 Installer Hash
>
> 44E4441EAB29D9FB9B5FD3EC4BAA63F8D425C8AC9622FAA432B17C1216FF64ED
>
> ### ARM64 Installer Hash
>
> 59D4AE125F7B8E937B4E68786107DF0C479D107BEDEF8B0434C7A28969F86FC0
>
> ## Highlights
>
> - New utility: Paste as Plain Text allows pasting the text contents of your clipboard without formatting. Note: the formatted text in the clipboard is replaced with the unformatted text. Thanks [@carlos-zamora](https://github.com/carlos-zamora)!
> - New utility: Mouse Jump allows to quickly move the mouse pointer long distances on a single screen or across multiple screens. Thanks [@mikeclayton](https://github.com/mikeclayton)!
> - Add new GPO policies for automatic update downloads and update toast notifications. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Support MSC and CPL files in "Run command" results of PowerToys Run Program plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Add support for log2 and log10 in PowerToys Run Calculator plugin. Thanks [@RickLuiken](https://github.com/RickLuiken)!
> - Added experimentation to PowerToys first run experience.  There are current page which says "welcome" and a variant with direct instructions on how to use some of the utilities.  We want to see if directly showing how to use PowerToys leads to more people using the features :)
>
> ### General
>
> - Improve metered network detection in runner. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Update PowerToys logo used by installer. Thanks [@ChaseKnowlden](https://github.com/ChaseKnowlden1)!
> - Add new GPO policies for automatic update downloads and update toast notifications. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Update copyright year to 2023. Thanks [@ChaseKnowlden](https://github.com/ChaseKnowlden)!
>
> ### FancyZones
>
> - Refactored and improved code quality.
> - Fix crashing on moving window between monitors with Win + arrows. (This was a hotfix for 0.67)
> - Fix issue causing window attributes to not be reset properly. (This was a hotfix for 0.67)
> - Fix issue causing window to not be adjusted when layout is changed. (This was a hotfix for 0.67)
> - Fix issue causing window not to be unsnapped on drag started. (This was a hotfix for 0.67)
> - Fix issue causing layouts not to be applied to new virtual desktops. (This was a hotfix for 0.67)
> - Fix issues causing windows not to be restored correctly to their last known zone.
>
> ### File explorer add-ons
>
> - Add Developer files previewer option to set max file size and fix styling issue. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Improve Developer files previewer exception handling and printing of error messages.
> - Fix crash when generating PDF and Gcode file thumbnails. (This was a hotfix for 0.67)
>
> ### Hosts file editor
>
> - Improve hosts file loading. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Improved duplicate hosts finding. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Keyboard Manager
>
> - Fix typo in Keyboard Manager Editor. Thanks [@ChristianLW](https://github.com/ChristianLW)!
>
> ### Mouse Utils
>
> - Resolve grammatical error in Mouse Highlighter description. Thanks [@WordlessSafe1](https://github.com/WordlessSafe1)!
> - New utility: Mouse Jump allows to quickly move the mouse pointer long distances on single or across screens. Thanks [@mikeclayton](https://github.com/mikeclayton)!
>
> ### Paste as Plain Text
>
> - New utility: Paste as Plain Text allows pasting the text contents of your clipboard without formatting. Note: the formatted text in the clipboard is replaced with the unformatted text. Thanks [@carlos-zamora](https://github.com/carlos-zamora)!
>
> ### PowerToys Run
>
> - Show Steam (steam://open/) shortcuts in the Program plugin.
> - Localize paths of Program plugin results. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Improved stability of the code used to get the localized names and paths. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Support MSC and CPL files in "Run command" results of Program plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Added missing MSC and CPL settings to the results of Windows Settings plugin. Thanks [@htcfreek](https://github.com/htcfreek)!
> - System plugin: Setting for separate "Open/Empty Recycle bin" results or single result with context menu. (This was implemented based on user feedback for a change in the last build.) Thanks [@htcfreek](https://github.com/htcfreek)!
> - Add support for log2 and log10 in Calculator plugin. Thanks [@RickLuiken](https://github.com/RickLuiken)!
> - Removed the TimeZone plugin.
> - Fix the crash when loading thumbnail for PDF files. (This was a hotfix for 0.67)
>
> ### Shortcut Guide
>
> - Added: Dismiss Shortcut Guide with mouse click. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Quick Accent
>
> - Added Lithuanian characters. Thanks [@saulens22](https://github.com/saulens22)!
> - Added additional (Chinese) characters. Thanks [@ChaseKnowlden](https://github.com/ChaseKnowlden)!
>
>
> ### Settings
>
> - Add missing flyout borders on Windows 10. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Add experimentation for oobe landing page. Thanks [@chenss3](https://github.com/chenss3)!
> - Show icons of user-installed PowerToys Run plugins. Thanks [@al2me6](https://github.com/al2me6)!
> - Fixed crash when clicking Browse for backup and restore location while running elevated.
> - Respect taskbar position when showing system tray flyout. (This was a hotfix for 0.67)
> - Show correct Hosts module image. (This was a hotfix for 0.67)
>
> ### Development
>
> - Turned on C++ code analysis and incrementally fixing warnings.
> - Centralize .NET NuGet packages versions. Thanks [@snickler](https://github.com/snickler)!
> - Separate PowerToys installer logs and MSI logs to different files.
> - Added new GPO rules to the reporting tool.
> - Move PowerToys registry entries back to HKLM to fix context menu entries not working on some configurations. (This was a hotfix for 0.67)

___

# v0.67.1		07/02/2023

> This is a patch release to fix issues in v.67.0 to fix some bugs we deemed important for stability based on incoming rates. See [v0.67.0](https://github.com/microsoft/PowerToys/releases/tag/v0.67.0) for full release notes.
>
> - #23733 - Fix FancyZones crashing on moving window between monitors with win + arrows
> - #23818 - Fix FancyZones issue causing window attributes not being reset properly
> - #23749 - Move PowerToys registry entries back to HKLM to fix context menu entries not working on some configurations
> - #23737 - Respect taskbar position when showing system tray flyout
> - #18166 - Fix Power Toys Run crashing when loading thumbnail for PDF files
> - #23801 - Show correct Hosts module image in PowerToys Settings app
> - #23820 - Fix FancyZones issue causing window not being adjusted when layout is changed
> - #23926 - Fix an issue causing issues not to be unsnapped when dragging started
> - #23927 - Fix layouts not being applied to new virtual desktops
> - #23579, #23811 - Fix for thumbnail crashes
>
> ## Installer Hashes
>
> ### x64 Installer Hash
>
> A5AD6BCA1DF8A9399A56334A96CCF49B1E55005B369493B2081388AE7BDC1FB4
>
> ### ARM64 Installer Hash
>
> E6F4E41C4B35B2F1F9262E4D954254D64769D68BDDFDDEC67B5626E58DCE3842
>

___

# v0.67.0		01/02/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F40
>
> In the [v0.67 release cycle][github-current-release-work], we focused on releasing new features and improvements.
>
> ## Installer Hashes
>
> ### x64 Installer Hash
>
> F653DEC43F4CE09F3C14C682BA18E3A18D34D2A2244ECBFDF3A879897A237383
>
> ### ARM64 Installer Hash
>
> 80CA4B9DE6B34E214F9E3F5063086E161608193FACA15CB24924660F818C4400
>
> ## Highlights
>
> - Added an option for PowerToys Run to tab through results instead of context buttons. Thanks [@maws6502](https://github.com/maws6502)!
> - All PowerToys registry entries are moved from machine scope (HKLM) to user scope (HKCU).
> - Quick access system tray launcher. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Awake
>
> - Disable instead of hiding "Keep screen on" option.
>
> ### FancyZones
>
> - Refactored and improved code quality.
>
> ### File explorer add-ons
>
> - Fixed escaping HTML-sensitive characters when previewing developer files. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Image Resizer
>
> - Improved code quality around a silent crash that was being reported to Microsoft servers.
>
> ### PowerToys Run
>
> - Add option to tab through results only. Thanks [@maws6502](https://github.com/maws6502)!
> - System plugin - Updated Recycle Bin command to allow opening the Recycle Bin. Thanks [@htcfreek](https://github.com/htcfreek)!
> - System plugin - Improved Recycle Bin command to not block PT Run while the deletion is running. Thanks [@htcfreek](https://github.com/htcfreek)!
> - System plugin - Small other changes to improve the usability of the Recycle Bin command. Thanks [@htcfreek](https://github.com/htcfreek)!
> - WindowWalker plugin - Show all open windows with action keyword. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Quick Accent
>
> - Added dashes characters. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Added Estonian characters. Thanks [@jovark](https://github.com/jovark)!
> - Added Hebrew characters. Thanks [@Evyatar-E](https://github.com/Evyatar-E)!
> - Added diacritical marks. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Added Norwegian characters. Thanks [@norwayman22](https://github.com/norwayman22)!
>
> ### Settings
>
> - Fixed URL click crash on the "What's New" screen.
> - Added quick access system tray launcher. Thanks [@niels9001](https://github.com/niels9001)!
>
> ### Documentation
>
> - Added PowerToys [disk usage footprint document](doc/devdocs/disk-usage-footprint.md).
> - Fixed some grammar issues on main readme / Wiki.  Thanks [@CanePlayz](https://github.com/CanePlayz)!
>
> ### Development
>
> - Verify notice.md file and used NuGet packages are synced.
> - Turned on C++ code analysis and incrementally fixing warnings.
> - Automatically add list of .NET Runtime deps to Installer during build. Thanks [@snickler](https://github.com/snickler)!
> - Move all installer registry entries to HKCU.
> - Refactor installer - extract module related stuff from Product.wxs to per-module .wxs file.
> - Enhance ARM64 build configuration verification. Thanks [@snickler](https://github.com/snickler)!
> - Helped identify a WebView2 issue affecting PowerToys File explorer add-ons, which has been fixed upstream and released as an update through the usual Windows Update channels.

___

# v0.66.0		05/01/2023

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F36
>
> In the [v0.66 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> ### x64 Installer Hash
>
> EE3F76F056A0611F69A203BA6D2B36FF81014B1FA29D1F46ECDCC6D312724FC6
>
> ### ARM64 Installer Hash
>
> 19DA79D497CAD28CC80A9C9954459EE93FE23FFC560DE7732DD806A38E862210
>
> ## Highlights
>
> - PowerToy utilities now ship with self-contained .NET 7, meaning it's not necessary to install .NET as part of the installer and it's easier to keep up to date.
> - It's possible to pick which of the installed OCR languages is used by Text Extractor by selecting it in the right-click context menu.
> - Added a setting to sort the order of the accented characters by usage frequency in Quick Accent.
>
> ### General
>
> - Reduced resource consumption caused by logging. A thread for each logger was being created even for disabled utilities.
> - The .NET 7 dependency is now shipped self-contained within the utilities, using deep links to reduce storage space usage.
>
> ### Color Picker
>
> - Fixed an issue where the custom color formats were not working when picking colors without using the editor.
> - Fixed a crash when using duplicated names for color formats.
> - Added two decimal formats, to distinguish between RGB and BGR.
> - Fixed color name localization, which was not working correctly on 0.65.
>
> ### FancyZones
>
> - Fixed an editor crash caused by deleting a zone while trying to move it.
> - Reduce the time it takes the tooltip for layout shortcut setting to appear in the editor.
>
> ### File Locksmith
>
> - Fixed an issue causing File Locksmith to hang when looking for open handles in some machines.
>
> ### Hosts File Editor
>
> - Added a warning when duplicated entries are detected. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### PowerToys Run
>
> - Support drag and dropping for file results. Thanks [@daniel-richter](https://github.com/daniel-richter)!
>
> ### Quick Accent
>
> - Added support for dark theme. Thanks [@niels9001](https://github.com/niels9001)!
> - Increased default input delay to improve out of the box experience.
> - Fixed a bug causing the first character to not be selected when opening the overlay. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Fixed the positioning of the overlay when showing near the horizontal edges of the screen.
> - Added additional Pinyin characters. Thanks [@char-46](https://github.com/char-46)!
> - Added Macedonian characters. Thanks [@ad-mca-mk](https://github.com/ad-mca-mk)!
> - Added a setting to sort characters by usage frequency.
> - Added a setting to always start selection in the first character, even when using the arrow keys as the activation method.
>
> ### Settings
>
> - Fixed an error that hid the option to keep the display on when using the "Indefinitely Awake" mode.
> - Fixed an accessibility issue causing the navigation bar to not work with narrator in scan mode.
> - Fixed an accessibility issue where the name for the shortcut control was not being read correctly.
> - Tweaked the Color Picker custom color format UI. Thanks [@niels9001](https://github.com/niels9001)!
> - Improved the shortcut control visibility and accessibility. Thanks [@niels9001](https://github.com/niels9001)!
> - Fixed an issue causing the Settings to not be saved correctly on scenarios where the admin user would be different then the user running PowerToys.
> - Added a setting to pick which language should be used by default when using Text Extractor.
>
> ### Text Extractor
>
> - Improve behavior for CJK languages by not adding spaces for some characters that don't need them. Thanks [@AO2233](https://github.com/AO2233)!
> - OCR language can now be picked in the right-click context menu.
>
> ### Video Conference Mute
>
> - Reduced resource consumption by not starting the File Watchers when the utility is disabled.
>
> ### Documentation
>
> - Updated the development setup documentation.
> - Improved the Markdown documentation lists numbering in many docs. Thanks [@sanidhyas3s](https://github.com/sanidhyas3s)!
>
> ### Development
>
> - Turned on C++ code analysis and incrementally fixing warnings.
> - C++ code analysis no longer runs on release CI to speed up building release candidates. It still runs on GitHub CI and when building locally to maintain code quality.
> - Cleaned up "to-do" comments referring to disposing memory on C#. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Added a fabric bot rule for localization issues.
> - Fixed a CI build error after a .NET tools update.
> - Update the Windows App SDK dependency version to 1.2.
> - When building for arm64, the arm64 build tools are now preferred when building on an arm64 device. Thanks [@snickler](https://github.com/snickler)!
> - Updated the C# test framework and removed unused Newtonsoft.Json package references.
> - Updated StyleCop and fixed/enabled more warnings. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Fixed a language typo in the code. Thanks [@eltociear](https://github.com/eltociear)!
> - Improved code quality around some silent crashes that were being reported to Microsoft servers.
> - Moved the GPO asset files to source instead of docs in the repo.
> - Upgraded the unit test NuGet packages.

___

# v0.65.0		06/12/2022

> [github-current-release-work]: https://github.com/microsoft/PowerToys/issues?q=issue+project%3Amicrosoft%2FPowerToys%2F35
>
> In the [v0.65 release cycle][github-current-release-work], we focused on stability and improvements.
>
> ## Installer Hashes
>
> ### x64 Installer Hash
>
> 8151675EE508CD3C91DF3088C52192C58080A15A4EDF958ADA99EBDA19C31CF1
>
> ### ARM64 Installer Hash
>
> 8780CF5E82F21BAAF3234F630A89175782A26D22EB7E32FD23F78FB3E4C336A2
>
> ## Highlights
>
> - The codebase was upgraded to work with .NET 7. Thanks [@snickler](https://github.com/snickler)!
> - Quick Accent can now show a description of the selected character. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - ColorPicker now supports adding custom formats.
>
> ### Known issues
>
> - The Text Extractor utility [fails to recognize text in some cases on ARM64 devices running Windows 10](https://github.com/microsoft/PowerToys/issues/20278).
> - After installing PowerToys, [the new Windows 11 context menu entries for PowerRename and Image Resizer might not appear before a system restart](https://github.com/microsoft/PowerToys/issues/19124).
> - There are reports of users who are [unable to open the Settings window](https://github.com/microsoft/PowerToys/issues/18015). This is being caused by incompatibilities with some applications (RTSS RivaTuner Statistics Server is a known examples of this). If you're affected by this, please check the  linked issue to verify if any of the presented solutions works for you.
>
> ### General
>
> - Downgraded the ModernWPF dependency to 0.9.4 to avoid issues on certain virtualization technologies. (This was a hotfix for 0.64)
> - Upgraded and fixed the code to work with .NET 7. Thanks [@snickler](https://github.com/snickler)!
>
> ### Always on Top
>
> - Added telemetry for the pinning/unpinning events.
>
> ### Awake
>
> - Added telemetry.
> - Removed exiting Awake from the tray icon when starting from the runner. Utilities started from the runner should be disabled in the Settings to avoid discrepancies.
>
> ### Color Picker
>
> - Fixed an infinite loop due to a looping UI refresh. (This was a hotfix for 0.64)
> - Added a feature to allow users to create their own color formats.
>
> ### FancyZones
>
> - Fixed an issue that caused turning off spaces between zones to not apply correctly. (This was a hotfix for 0.64)
> - Prevent the shift key press from trickling down to the focused window. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Fixed a bug causing FancyZones to try resizing hidden windows.
> - Fixed the focus layout preview being empty on first run in the editor.
> - Fixed UI margin in the "Create new layout" dialog.
> - Fixed window positioning issues when switching between virtual desktops.
> - Fixed snapping by hotkey in single zone layouts.
>
> ### File explorer add-ons
>
> - Added .log file support to the Monaco preview handler. Thanks [@Eagle3386](https://github.com/Eagle3386)!
>
> ### File Locksmith
>
> - Query system and other users processes when elevated. (This was a hotfix for 0.64)
> - Icon and UI fixes. Thanks [@niels9001](https://github.com/niels9001)! (This was a hotfix for 0.64)
>
> ### Group Policy Objects
>
> - Removed a obsolete dependency from the admx file to fix importing on Intune. Thanks [@htcfreek](https://github.com/htcfreek)! (This was a hotfix for 0.64)
>
> ### Hosts File Editor
>
> - Added a scrollbar to the additional lines dialog. Thanks [@davidegiacometti](https://github.com/davidegiacometti)! (This was a hotfix for 0.64)
> - Updated the plus icon. Thanks [@niels9001](https://github.com/niels9001)! (This was a hotfix for 0.64)
> - Prevent the new entry content dialog from overlapping the title bar.
> - Updated the name for the additional lines feature. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Added a workaround for an issue causing the context menu not opening on right-click. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
>
> ### Image Resizer
> - Fixed a silent crash when trying to show the tier 1 context menu on Windows 11.
>
> ### PowerToys Run
>
> - Added pinyin support to the search. Thanks [@frg2089](https://github.com/frg2089)!
> - Fixed an error in the TimeZone plugin preventing searching for standard time zones. Thanks [@Tantalus13A98B5F](https://github.com/Tantalus13A98B5F)!
> - Added the English abbreviations as fallbacks in the UnitConverter plugin. Thanks [@Tantalus13A98B5F](https://github.com/Tantalus13A98B5F)!
>
> ### Quick Accent
>
> - Added mappings for the mu, omicron, upsilon and thorn characters.
> - Added a setting to exclude apps from activating Quick Accent.
> - Fixed an issue causing the selector to trigger when leaving the lock screen. Thanks [@damienleroy](https://github.com/damienleroy)!
> - Added the Croatian, Netherlands, Swedish and Welsh character sets. Thanks [@damienleroy](https://github.com/damienleroy)!
> - Added support for more unicode characters. Thanks [@char-46](https://github.com/char-46)!
> - Shift-space can now navigate backwards in the selector. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Added the Catalan accented characters. Thanks [@ivocarbajo](https://github.com/ivocarbajo) and [@codingneko](https://github.com/codingneko)!
> - Added the Kurdish accented characters.
> - Added the Serbian accented characters. Thanks [@damienleroy](https://github.com/damienleroy)!
> - Added the Irish and Scottish accented characters.
> - Added the description for the currently selected character in the selector.
> - Fixed a bug causing the selector window to appear blank.
>
> ### Runner
>
> - Fixed a crash on a racing condition accessing the IPC communication with Settings.
>
> ### Settings
>
> - Fixed settings name in the QuickAccent page. Thanks [@htcfreek](https://github.com/htcfreek)!
> - Added a message indicating there's no network available when looking for updates.
> - Fixed an error causing the backup/restore feature to not find the backup file. Thanks [@jefflord](https://github.com/jefflord)!
> - Fixed localization for the "All apps" expression in the keyboard manager page.
> - UI refactoring, clean-up and bringing in modern controls. Thanks [@niels9001](https://github.com/niels9001)!
> - Improved settings/OOBE screens text. Thanks [@Jay-o-Way](https://github.com/Jay-o-Way)!
> - The backup/restore feature also backs up FancyZones layouts.
>
> ### Shortcut Guide
> - Added a setting to make the shortcuts and taskbar icons have different configurable response times. Thanks [@OkamiWong](https://github.com/OkamiWong)!
>
> ### Video Conference Mute
>
> - Changed the warning about deprecating Video Conference Mute to saying it's going to go into legacy mode, thanks to community feedback.  (This was a hotfix for 0.64)
>
> ### Documentation
>
> - Added the core team to COMMUNITY.md
>
> ### Development
>
> - Fixed some errors in the GitHub issue templates. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!
> - Updated the Windows implementation library. Thanks [@AtariDreams](https://github.com/AtariDreams)!
> - Added Hosts File Editor to the issue templates. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Turned on C++ code analysis and incrementally fixing warnings.
> - Cleaned up unused dependencies. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Fixed building on the latest MSVC.
> - Fixed multi-processor build on the latest MSBuild.
> - Added a message to suggest the feedback hub to the fabric bot triggers.
> - Optimized every png file with the zopfli algorithm. Thanks [@pea-sys](https://github.com/pea-sys)!
> - Updated the .vsconfig file for a quicker development setup. Thanks [@ChaseKnowlden](https://github.com/ChaseKnowlden)!
> - Fixed a language typo in the code. Thanks [@eltociear](https://github.com/eltociear)!
> - Fixed wrong x86 target in the solution file.
> - Added a script to fail building when the nuget packages aren't consolidated. Thanks [@davidegiacometti](https://github.com/davidegiacometti)!
> - Upgraded the Vanara.Invoke dependencies.
> - Upgraded and brought back the spell-checker. Thanks [@jsoref](https://github.com/jsoref)!
> - Added a new dependencies feed and fixed release CI. Thanks [@Aaron-Junker](https://github.com/Aaron-Junker)!