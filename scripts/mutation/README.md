# Mutation testing

Stryker.NET changes the code of the extension a little at a time (a `<` becomes `<=`, a condition is negated, a statement is removed) and runs the tests.
A mutant the tests do not fail on is a hole in the tests. The test project targets `net48` and compiles the source in, which Stryker cannot mutate, so
this folder holds a copy of the project in `net8.0`: `Core` compiles the same source files, `Tests` compiles the same test files.

```
dotnet tool install -g dotnet-stryker
cd scripts/mutation/Tests
dotnet test                       # first, the tests must pass here
dotnet stryker                    # report: StrykerOutput/<time>/reports/mutation-report.html
```

- `concurrency` in `stryker-config.json` is the number of test processes at once: set it to the cores of the machine (a few hundred MB each).
- A full run is about 8,000 mutants: roughly 1 to 1.5 hours on 4 cores.
- `dotnet stryker --since:main` mutates only what changed against `main`.
- Narrow a run to a folder: `dotnet stryker --mutate "**/Completion/**/*.cs"`.
