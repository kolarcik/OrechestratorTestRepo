Task: Sync feature branch and prepare merge to main (QA)

Deliverable (exact):
1) Create branch team1/QA/merge-to-main from team1/QA and prepare it for merging into main with the fixes below applied and verified.

Actions to perform in your QA worktree:
- git checkout team1/QA && git pull origin team1/QA
- git checkout -b team1/QA/merge-to-main

Fixes to apply:
1) Set correct TargetFramework in TetrisBlazor.PlaywrightTests/TetrisBlazor.PlaywrightTests.csproj to match repository (e.g., <TargetFramework>net6.0</TargetFramework>)
2) Remove duplicate task.md at repo root if present (git rm task.md)
3) Make install script executable: chmod +x scripts/install-playwright.sh
4) Run install and tests locally:
   - ./scripts/install-playwright.sh
   - dotnet restore && dotnet build
   - dotnet test TetrisBlazor.PlaywrightTests --logger "console;verbosity=minimal"
   Fix any issues and commit

Finalization:
- git add -A && git commit -m "chore(playwright): prepare merge to main - tfm, scripts, cleanup"
- git push -u origin team1/QA/merge-to-main
- Open PR: base=main, head=team1/QA/merge-to-main with checklist: build passes, tests pass locally, playwright install documented, TargetFramework change noted

Acceptance criteria:
- dotnet test passes locally (including Playwright tests)
- PR to main is created and CI passes

Notes:
- If CI cannot run Playwright browsers, document npx playwright install in the PR and coordinate with infra for OS deps.
- If TargetFramework change breaks other projects, stop and request guidance before proceeding.
