# 0001 - Project Scafolding

Create a .NET ASP.NET Core project using the latest available framework version.

All production projects must be located under:
./src/Project/Project.csproj

All unit test projects must be located under:
./__tests__/Project.Tests/Project.Tests.csproj

All projects must include the following libraries:
- SonarAnalyzer.CSharp
- StyleCop.Analyzers
- Roslynator.Analyzers
- Roslynator.CodeFixes
- Roslynator.Formatting.Analyzers

Prefer adding these libraries via a Directory.Build.Props file so they are applied consistently across all projects.