# Contributing

Thanks for contributing.

## Development setup
1. Install .NET 10 SDK.
2. Restore and build:

```powershell
dotnet restore .\Atualizador_Versao_RDS.sln
dotnet build .\Atualizador_Versao_RDS.sln -c Release
```

## Pull request guidelines
1. Create a branch from `main`.
2. Keep changes focused (one feature/fix per PR).
3. Update `README` and/or `CHANGELOG.md` when behavior changes.
4. Ensure build passes locally.
5. Open a PR with:
   - clear summary
   - test steps
   - screenshots (if UI changes)

## Commit message suggestion
- `feat: ...`
- `fix: ...`
- `docs: ...`
- `chore: ...`
