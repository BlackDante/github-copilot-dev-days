# Copilot CLI Presentation Runbook

## 1. Clone the repo

```bash
git clone https://github.com/BlackDante/github-copilot-dev-days
cd github-copilot-dev-days
```

## 2. Install Copilot CLI

```bash
npm install -g @github/copilot
# or
brew install copilot-cli
# or
winget install GitHub.Copilot
```

## 3. Start and log in

```bash
copilot
```

```text
/login
```

## 4. Quick orientation commands

```text
/help
/version
/env
/model
```

## 5. Verify it works

```text
Say hello and tell me what you can help with
```

## 6. Run the sample app

```bash
cd samples/notes-app-project
dotnet run -- list
cd ../..
```

## 7. Use shell commands inside Copilot

```text
!pwd
!git status
!cd samples/notes-app-project && dotnet run -- list
```

## 8. Explain the permissions model

```text
/allow-all
/add-dir
/list-dirs
/cwd
/reset-allowed-tools
```

## 9. Ask about code with file context

```text
What does @samples/notes-app-project/Program.cs do?
@samples/notes-app-project/NoteStore.cs Explain this file
@samples/notes-app-project/ Give me an overview of this project
@samples/notes-app-project/Program.cs @samples/notes-app-project/NoteStore.cs How do they work together?
```

## 10. Run in non-interactive mode

```bash
copilot -p "How do I read a JSON file in C#?"
copilot --allow-all -p "List all functions in @samples/notes-app-project/Program.cs and describe what each does"
copilot -p "Review @samples/notes-app-project/NoteStore.cs and suggest 3 improvements"
```

## 11. Use `/research` before implementation

```text
/research
Research the best approach for input validation in a small C# CLI app before we implement search. Compare 2-3 practical options, explain tradeoffs, and recommend the simplest workshop-friendly approach.
```

## 12. Plan and implement a feature

```bash
copilot --plan
```

```text
/plan Add a "search" command to the notes app. Search title, category, and body. Keep archived notes hidden unless --all is used.
Implement it in @samples/notes-app-project/Program.cs and @samples/notes-app-project/NoteStore.cs
/diff
/review
```

After Copilot applies the change:

```bash
cd samples/notes-app-project
dotnet run -- search demo
dotnet run -- search archive --all
cd ../..
```

## 13. Showcase `/sessions` and `/fleet`

```text
/sessions
Show my recent sessions for this repository
Open the session where we added search to the notes app
```

```text
/fleet
Onboard me to this repository. Use multiple agents to inspect the main files in parallel and summarize the architecture, key commands, and where I should start.
/fleet Scan @samples/notes-app-project/ and split the work across agents: one agent reviews Program.cs, one reviews NoteStore.cs, and one reviews the tests. Then give me a concise repo walkthrough.
```

## 14. Rename the current session

```text
/rename
Workshop demo - notes app search feature
```

This is useful before sharing or revisiting a session later.

## 15. Useful session commands to demo

```text
/tasks
/session
/context
/usage
/share
```

## 16. Use the built-in review flow

```bash
copilot
```

```text
/review
Review the current changes in @samples/notes-app-project/Program.cs and @samples/notes-app-project/NoteStore.cs
```

## 17. Rewind the last turn

```text
Add a temporary `stats` command to @samples/notes-app-project/Program.cs that prints the total number of notes and archived notes.
Update @samples/notes-app-project/NoteStore.cs if needed to support it.
```

```text
/rewind
```

## 18. Create a project agent

```bash
mkdir -p .github/agents
cat <<'EOF' > .github/agents/notes-reviewer.agent.md
---
name: notes-reviewer
description: Reviews the Notes CLI sample for C# and .NET code quality, maintainability, and simple design improvements.
---

You are reviewing a small .NET console application used in a workshop.

Focus on:
- C# readability and naming
- CLI argument handling
- JSON persistence and file access
- Small, practical refactor suggestions

Avoid:
- Large architectural rewrites
- Suggestions that add unnecessary complexity for a workshop sample
EOF
```

```bash
copilot --agent notes-reviewer
```

```text
/agent
Review @samples/notes-app-project/Program.cs
Review @samples/notes-app-project/NoteStore.cs for C# code quality issues
```

## 19. Use skills to generate and improve tests

```text
/skills
/xunit-test-gen Create a new xUnit test project in @samples/notes-app-project.tests and add tests for the search behavior in @samples/notes-app-project/NoteStore.cs
Review whether the tests cover archived-note behavior and edge cases
```

```bash
mkdir -p .github/skills/xunit-test-gen
cat <<'EOF' > .github/skills/xunit-test-gen/SKILL.md
---
name: xunit-test-gen
description: Generates focused xUnit tests for the Notes CLI workshop project.
---

When asked to generate or improve tests for the Notes CLI sample:
- write xUnit tests in samples/notes-app-project.tests
- create the xUnit test project if it does not exist yet
- prefer deterministic tests around NoteStore and CLI-facing behavior
- use temporary files and directories for persistence tests
- keep the tests small, readable, and workshop-friendly
EOF
```

```text
/skills
/xunit-test-gen Create a new xUnit test project in @samples/notes-app-project.tests and add tests for the search behavior in @samples/notes-app-project/NoteStore.cs
```

After Copilot creates the test project and files:

```bash
cd samples/notes-app-project.tests
dotnet test
cd ../..
```

## 20. Explore plugins with `/plugin`

```text
/plugin
```

## 21. Connect MCP servers with `/mcp add`

```text
/mcp
/mcp add
```

Add:

1. `context7`
2. `playwright`

Then verify what is loaded:

```text
/env
```

## 22. Connect MCP servers with config

Create `.mcp.json` in the repo root:

```json
{
  "mcpServers": {
    "context7": {
      "type": "local",
      "command": "npx",
      "args": ["-y", "@upstash/context7-mcp"],
      "tools": ["*"]
    },
    "playwright": {
      "type": "local",
      "command": "npx",
      "args": ["@playwright/mcp@latest"],
      "tools": ["*"]
    }
  }
}
```

Then restart Copilot:

```text
/restart
/env
```

## 23. MCP prompts to demo

```text
Find the current docs for xUnit fixtures and summarize the main patterns
Open dotnet.microsoft.com in a browser and tell me what the main navigation items are
Go to the Playwright docs homepage and summarize what it can do
Use Context7 to get the latest ASP.NET Core docs for dependency injection
```

## 24. Extra commands worth showing

```text
/init
/instructions
/experimental
/new
/restart
/clear
/exit
```
