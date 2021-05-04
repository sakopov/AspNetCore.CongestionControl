# Release Process

The following steps are recommended in order to make a new release.

1. Submit a pull request containing proposed changes. Note, commit messages have to follow semantic-release conventions as defined by [Angular Commit Message Conventions](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#-git-commit-guidelines).

2. After approval, the pull request is merged and the master branch is automatically tagged with a new version based on new commit history. The binaries will not be published to Nuget until a release is created.

3. Draft a new release and tag it with the latest version. Fill out description and publish the release. This will start a new build, which will package and publish binaries to Nuget.
