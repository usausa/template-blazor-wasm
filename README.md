# Template project for Blazor WebAssembly

English | [日本語](README.ja.md)

## E2E tests

The E2E tests (`tests/Template.BlazorWasm.E2ETests`) run with Playwright. They are not part of the daily runs, and are run when needed and before releases. Run the commands in PowerShell at the repository root.

```powershell
# Get the browsers (first time. Uses the script output by the build)
dotnet build tests/Template.BlazorWasm.E2ETests
pwsh tests/Template.BlazorWasm.E2ETests/bin/Debug/net10.0/playwright.ps1 install

# Run (Chromium and headless by default)
dotnet run --project tests/Template.BlazorWasm.E2ETests

# Run with the browser shown
$env:HEADED = "1"; dotnet run --project tests/Template.BlazorWasm.E2ETests; Remove-Item Env:HEADED

# Switch the browser (chromium / firefox / webkit)
$env:BROWSER = "firefox"; dotnet run --project tests/Template.BlazorWasm.E2ETests; Remove-Item Env:BROWSER

# Run step by step with Playwright Inspector
$env:PWDEBUG = "1"; dotnet run --project tests/Template.BlazorWasm.E2ETests; Remove-Item Env:PWDEBUG
```

- Only for failed tests, a trace that records the screen, DOM, network, and console of each action is saved to `tests/Template.BlazorWasm.E2ETests/bin/Debug/net10.0/playwright-traces/<test display name>.zip`. Open it with `pwsh tests/Template.BlazorWasm.E2ETests/bin/Debug/net10.0/playwright.ps1 show-trace <zip>` or https://trace.playwright.dev
