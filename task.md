Task: Implement Playwright E2E test and add project to solution (QA)

Deliverable (exact):
1) Create feature branch from team1/QA named: team1/QA/playwright-e2e
2) Create a root-level xUnit test project named TetrisBlazor.PlaywrightTests and add it to the solution:
   - dotnet new xunit -n TetrisBlazor.PlaywrightTests
   - dotnet sln add TetrisBlazor.PlaywrightTests/TetrisBlazor.PlaywrightTests.csproj
3) Add dependency and install browsers:
   - dotnet add TetrisBlazor.PlaywrightTests package Microsoft.Playwright
   - Run: playwright install (or document npm alternative) and provide a small script scripts/install-playwright.sh
4) Implement E2E test(s):
   - Start the server in test setup: dotnet run --project ./TetrisBlazor.Server --urls http://localhost:5000 (background) OR use WebApplicationFactory/TestServer
   - Poll http://localhost:5000/ or /health until ready (timeout configurable)
   - Use Microsoft.Playwright to launch Chromium, navigate to http://localhost:5000 and assert a stable UI selector (e.g., text=Start or h1)
   - Ensure server process is terminated in teardown and avoid orphan processes
5) Ensure project TargetFramework matches repo (e.g., net6.0) and tests run via: dotnet test
6) Commit to team1/QA/playwright-e2e and open PR to team1/QA with run instructions and note about "playwright install".

Acceptance criteria:
- Project exists at ./TetrisBlazor.PlaywrightTests and is added to the .sln
- dotnet test executes the new tests and they pass locally

Notes:
- Add scripts/install-playwright.sh and a README snippet for running tests and preparing CI.
- Prefer Playwright wait-for selectors and deterministic waits; avoid global tool assumptions in CI.

Context:
- Repository: TetrisBlazor (Blazor app). Your branch: team1/QA. Work from a feature branch off team1/QA.

Assignment (deliverable only):
1) Create a new xUnit test project at repository root named TetrisBlazor.PlaywrightTests.
   - Commands:
     - dotnet new xunit -n TetrisBlazor.PlaywrightTests
     - dotnet sln add TetrisBlazor.PlaywrightTests/TetrisBlazor.PlaywrightTests.csproj
2) Add Playwright for .NET and install the Playwright CLI/browsers:
   - dotnet add TetrisBlazor.PlaywrightTests package Microsoft.Playwright
   - dotnet tool install --global Microsoft.Playwright.CLI
   - playwright install
3) Implement an E2E test that:
   - Starts the server: dotnet run --project ./TetrisBlazor.Server (run as background process from test setup).
   - Waits until the server responds on http://localhost:5000 (simple polling with timeout).
   - Uses Microsoft.Playwright to launch Chromium and navigate to http://localhost:5000.
   - Asserts the main page loaded by checking for a page title or a Start button (selector example: text=Start or h1 containing app name).
   - Stops the server process and cleans up after the test.
4) Make tests runnable via: dotnet test
5) Commit on branch named: team1/QA/playwright-e2e and open a PR to team1/QA. In PR description include how to run tests locally and mention `playwright install` requirement.

Acceptance criteria:
- dotnet test passes for TetrisBlazor.PlaywrightTests.
- PR exists in team1/QA with test implementation and run instructions.

Notes:
- Keep tests robust (retry/wait for server), avoid flaky timing by using Playwright wait-for selectors and reasonable timeouts.
- If Playwright global tool cannot be installed in the environment, document alternative steps (npm install -D playwright) in PR.

