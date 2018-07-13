The following tasks should be completed before publishing a release. Track the progress of the release by copying and pasting the tasks below into an issue for the release.

#### Github and Project Planning

- [ ] Review and merge any outstanding pull requests.
- [ ] Review any oustanding issues assigned to this release milestone.
- [ ] Branch from `develop` to `rc-[version]`, ex: `rc-2.0.0`.
- [ ] Draft release with version in the format of `v2.0.0` targeting the 'master' branch. Standard release should be named using the format `IBM Watson SDK for Unity [version]`, ex: `IBM Watson SDK for Unity v2.0.0`.
 
#### Source Changes (in `rc` branch)
- [ ] Update `String.Version` in `Scripts/Utilities/Constants.cs` to the current version, ex: `watson-apis-unity-sdk/2.0.0`
- [ ] Update changelog.
- [ ] Update `PROJECT_NUMBER` in `Doxyfile` to current version.

#### Publish Release

- [ ] Create a pull request to merge `rc` branch to `master`. After all checks have passed, merge the PR.
- [ ] Publish release.
- [ ] Create a pull request to merge `rc` branch into the `development` branch.