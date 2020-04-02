# UpdateCompactControl

[!["Language"](https://img.shields.io/github/languages/top/saeeddiscovery/UpdateCompactControl.svg)](https://docs.microsoft.com/en-us/dotnet/csharp)
[![Publish](https://github.com/saeeddiscovery/UpdateCompactControl/workflows/Publish/badge.svg?branch=master)](https://github.com/saeeddiscovery/UpdateCompactControl/actions?query=workflow%3APublish)

[!["Latest Release"](https://img.shields.io/github/v/release/saeeddiscovery/UpdateCompactControl.svg)](https://github.com/saeeddiscovery/UpdateCompactControl/releases/latest)
[!["Release Date"](https://img.shields.io/github/release-date/saeeddiscovery/UpdateCompactControl.svg)](https://github.com/saeeddiscovery/UpdateCompactControl/releases/latest)

[!["Contributors"](https://img.shields.io/github/contributors/saeeddiscovery/UpdateCompactControl.svg)](https://github.com/saeeddiscovery/UpdateCompactControl/graphs/contributors)


## Application to update the CompactControl application to the latest release

> This app would only work inside the directory of the CompactControl application

-------------------------
## Publish the Release Automatically:

1. Set the proper version in the Visual Studio.
    - Project (menu) -> UpdateCompactControl Properties... -> Application (tab) -> Assembly Information... (button)
2. Update ```CHANGELOG.md``` with the correct version and change logs.
    - version MUST be in this format: ```1.1.1-rc.1``` (Change the first 3 numbers according to the version set in the Visual Studio)
3. Commit changes.
4. push to the master branch (or your current branch)
    ```> git push```
5. Tag the last commit. (Tag MUST be identical to the ChangeLog's version):  
    ```> git tag -a v1.1.1-rc.1 -m "comment"```
6. Push with the same tag:   
    ```> git push origin --tag v1.1.1-rc.1```

-------------------------
- Delete a tag from local repository: 
    - ```> git tag -d v1.1.1-rc.1```
- Delete a tag from remote repository: 
    - ```> git push --delete origin v1.1.1-rc.1```
