# Mutation testing report

Run on 2026-10-10 with Stryker.NET 4.8.1 on branch `test/mutation-run` (base `main` at `a24738b`), 4 test processes, 2h53m. The 1,445 tests passed before the run.
Full interactive report: [`mutation/mutation-report.html`](mutation/mutation-report.html) (download and open in a browser).

## Result

**Mutation score: 85.64 %** (thresholds in the config: high 90, low 70).

| Status | Mutants |
|---|---:|
| Killed | 5416 |
| Timeout (counts as detected) | 1786 |
| Survived | 1078 |
| No coverage | 130 |
| Compile error (skipped) | 759 |
| Ignored (skipped) | 724 |
| **Total created** | **9893** |

Score = (Killed + Timeout) / (Killed + Timeout + Survived + No coverage).

## Files, by number of undetected mutants

| File | Detected | Survived | No coverage | Score |
|---|---:|---:|---:|---:|
| `Completion/Languages/SqlSchema.cs` | 302 | 175 | 17 | 61.1 % |
| `Completion/CompletionEngine.cs` | 472 | 110 | 16 | 78.9 % |
| `EmbeddedLanguages/Css/CssTokenizer.cs` | 665 | 93 | 14 | 86.1 % |
| `Completion/Languages/HtmlCompletion.cs` | 396 | 66 | 16 | 82.8 % |
| `EmbeddedLanguages/Yaml/YamlTokenizer.cs` | 353 | 67 | 11 | 81.9 % |
| `Completion/Languages/CssCompletion.cs` | 451 | 67 | 8 | 85.7 % |
| `EmbeddedLanguages/Markdown/MarkdownTokenizer.cs` | 435 | 61 | 4 | 87.0 % |
| `Detection/Detectors.cs` | 168 | 52 | 0 | 76.4 % |
| `Detection/ILanguageDetector.cs` | 56 | 29 | 15 | 56.0 % |
| `EmbeddedLanguages/GraphQl/GraphQlTokenizer.cs` | 185 | 36 | 0 | 83.7 % |
| `Hosts/CSharpHostScanner.cs` | 251 | 32 | 0 | 88.7 % |
| `EmbeddedLanguages/Xml/XmlTokenizer.cs` | 169 | 21 | 7 | 85.8 % |
| `Hosts/CppHostScanner.cs` | 148 | 27 | 0 | 84.6 % |
| `Hosts/JavaScriptHostScanner.cs` | 220 | 23 | 0 | 90.5 % |
| `EmbeddedLanguages/Regex/RegexTokenizer.cs` | 260 | 22 | 0 | 92.2 % |
| `Completion/ApproximateMatcher.cs` | 129 | 21 | 0 | 86.0 % |
| `Completion/Languages/WgslCompletion.cs` | 78 | 21 | 0 | 78.8 % |
| `Highlighting/HighlightEngine.cs` | 126 | 18 | 1 | 86.9 % |
| `EmbeddedLanguages/Html/HtmlTokenizer.cs` | 204 | 13 | 5 | 91.9 % |
| `Hosts/PythonHostScanner.cs` | 131 | 16 | 0 | 89.1 % |
| `EmbeddedLanguages/Sql/SqlTokenizer.cs` | 356 | 12 | 2 | 96.2 % |
| `Completion/Languages/YamlCompletion.cs` | 38 | 10 | 2 | 76.0 % |
| `EmbeddedLanguages/Shaders/ShaderTokenizer.cs` | 153 | 12 | 0 | 92.7 % |
| `Common/TextUtil.cs` | 71 | 8 | 1 | 88.8 % |
| `Detection/DetectionOptions.cs` | 52 | 5 | 4 | 85.2 % |
| `Completion/Languages/CompletionLanguage.cs` | 54 | 8 | 0 | 87.1 % |
| `Completion/Languages/JsonCompletion.cs` | 42 | 4 | 4 | 84.0 % |
| `Common/EmbeddedLanguageMarkers.cs` | 125 | 7 | 0 | 94.7 % |
| `Common/ResumableScan.cs` | 51 | 7 | 0 | 87.9 % |
| `Completion/Languages/GlslCompletion.cs` | 38 | 6 | 0 | 86.4 % |
| `Completion/WordRanking.cs` | 84 | 6 | 0 | 93.3 % |
| `Highlighting/CachingHostScanner.cs` | 15 | 4 | 2 | 71.4 % |
| `Highlighting/IncrementalHostScan.cs` | 87 | 5 | 0 | 94.6 % |
| `Completion/Languages/SqlCompletion.cs` | 306 | 3 | 0 | 99.0 % |
| `EmbeddedLanguages/Json/JsonTokenizer.cs` | 120 | 2 | 1 | 97.6 % |
| `Completion/Languages/CompletionLanguages.cs` | 6 | 2 | 0 | 75.0 % |
| `Highlighting/EmbeddedLanguageRegistry.cs` | 7 | 2 | 0 | 77.8 % |
| `Highlighting/NestLightComposition.cs` | 14 | 2 | 0 | 87.5 % |
| `Highlighting/SnapshotTokenCache.cs` | 33 | 2 | 0 | 94.3 % |
| `Completion/Position.cs` | 7 | 1 | 0 | 87.5 % |
| `Common/Model.cs` | 7 | 0 | 0 | 100.0 % |
| `Completion/CompletionFeatures.cs` | 11 | 0 | 0 | 100.0 % |
| `Completion/KeywordUse.cs` | 258 | 0 | 0 | 100.0 % |
| `Completion/Languages/GraphQlCompletion.cs` | 7 | 0 | 0 | 100.0 % |
| `Completion/Languages/MarkdownCompletion.cs` | 2 | 0 | 0 | 100.0 % |
| `Completion/Languages/RegexCompletion.cs` | 2 | 0 | 0 | 100.0 % |
| `Completion/Languages/XmlCompletion.cs` | 1 | 0 | 0 | 100.0 % |
| `Completion/SuggestionIcon.cs` | 5 | 0 | 0 | 100.0 % |
| `EmbeddedLanguages/Shaders/ShaderVocabulary.cs` | 51 | 0 | 0 | 100.0 % |

## Reading it

A survived mutant is a change to the code that no test failed on, so it points to a missing or weak assertion. The files at the top of the table are where new tests pay off most.

