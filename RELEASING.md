# Releasing

NestLight follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html): `MAJOR.MINOR.PATCH`.

- **MAJOR**: a change that breaks how existing users mark or see their code (for example a removed language id or marker form).
- **MINOR**: new hosts, languages or features that keep existing behavior.
- **PATCH**: fixes only.
- While MAJOR is `0` the extension is in initial development: anything may change, and releases are flagged as pre-releases. `1.0.0` marks the first stable release.

The version lives in one place, the `Identity` element of [NestLight/source.extension.vsixmanifest](NestLight/source.extension.vsixmanifest).
The VSIX manifest accepts only numeric parts, so pre-release suffixes (`-beta.1`) are not used.

## Steps

1. Update `Version` in `source.extension.vsixmanifest`.
2. Merge to `main`.
3. Tag the merge commit and push the tag:
   ```
   git tag -a v0.1.0 -m "NestLight 0.1.0"
   git push origin v0.1.0
   ```

The [Build workflow](.github/workflows/build.yml) then runs the tests, builds `NestLight-<version>.vsix`, writes the test report and creates the GitHub release with both files.
It fails if the tag does not match the manifest version, or if any test fails.

Pushing a branch named `build/*` runs the same build without creating a release: the VSIX and the report are downloadable from the workflow run.
