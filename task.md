Task: Add test project to solution and implement Playwright E2E test (QA)

Assignment (deliverable only):
1) From branch team1/QA create feature branch: team1/QA/playwright-e2e.
2) Create a new xUnit test project at repository root named TetrisBlazor.PlaywrightTests and add it to the solution:
   - dotnet new xunit -n TetrisBlazor.PlaywrightTests
   - dotnet sln add TetrisBlazor.PlaywrightTests/TetrisBlazor.PlaywrightTests.csproj
3) Add Playwright and browsers:
   - dotnet add TetrisBlazor.PlaywrightTests package Microsoft.Playwright
   - Install browsers (local dev): playwright install
4) Implement an E2E test that:
   - Starts the server: dotnet run --project ./TetrisBlazor.Server --urls http://localhost:5000 (background from test setup).
   - Waits for readiness (poll http://localhost:5000/ or a health endpoint) with timeout.
   - Uses Microsoft.Playwright to launch Chromium, navigate to http://localhost:5000, and assert a main UI element (title or Start button).
   - Stops the server and cleans up.
5) Ensure the project's TargetFramework matches the solution (e.g., net6.0). Make tests runnable via: dotnet test
6) Commit changes on team1/QA/playwright-e2e and open a PR to team1/QA with instructions including `playwright install` and how to run the tests.

Acceptance criteria:
- The test project is present in the repo and listed in the .sln.
- dotnet test runs and the Playwright test passes locally.

Notes:
- Prefer adding a small script or README note: scripts/install-playwright.sh or npm alternative for CI.
- Keep lifecycle deterministic (fixed port, explicit shutdown).

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

