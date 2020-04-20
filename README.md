# UpdateCompactControl

[!["Language"](https://img.shields.io/github/languages/top/saeeddiscovery/UpdateCompactControl.svg)](https://docs.microsoft.com/en-us/dotnet/csharp)
[!["Latest Release"](https://img.shields.io/github/v/release/saeeddiscovery/UpdateCompactControl.svg)](https://github.com/saeeddiscovery/UpdateCompactControl/releases/latest)
[!["Release Date"](https://img.shields.io/github/release-date/saeeddiscovery/UpdateCompactControl.svg)](https://github.com/saeeddiscovery/UpdateCompactControl/releases/latest)

[!["Company"](https://img.shields.io/badge/Company-AitinTech-blue.svg?style=social&logo=c)](http://AitinTech.ir/)
[!["Contributors"](https://img.shields.io/github/contributors/saeeddiscovery/CompactControl.svg?style=social&logo=coderwall)](https://github.com/saeeddiscovery/CompactControl/graphs/contributors)
[!["GUI-WPF"](https://img.shields.io/badge/Made%20with-WPF-brightgreen.svg?style=social&logo=Microsoft)]()


## Application to update the CompactControl application to the latest release

> This app would only work inside the directory of the CompactControl application

-------------------------
## Publish the Release Automatically:

1. Set the proper version in the Visual Studio.
    - Project (menu) -> UpdateCompactControl Properties... -> Application (tab) -> Assembly Information... (button)
2. Build the Release
3. Update ```CHANGELOG.md``` with the correct version and change logs.
    - version MUST be in this format: ```1.1.1-rc.1``` (Change the first 3 numbers according to the version set in the Visual Studio)
4. Commit changes.
5. push to the master branch (or your current branch)
    ```> git push```
6. Tag the last commit. (Tag MUST be identical to the ChangeLog's version):  
    ```> git tag -a v1.1.1-rc.1 -m "comment"```
7. Push with the same tag:   
    ```> git push origin --tag v1.1.1-rc.1```

-------------------------
### If you want to remove a tag or multiple tags, it is beter to remove it first from the remote and then from the local repositories

#### Method1: Using a list
``` bash
> git tag -l "v1*" > tags_to_remove.txt
> git tag -d $(cat ./tags_to_remove.txt)
> git push --delete origin $(cat ./tags_to_remove.txt)
```

#### Method2: Manual
- Delete a tag from remote repository:
    - ```> git push --delete origin v1.1.1-rc.1```
- Delete multiple tags from remote repository: 
    - ```> git push --delete origin v1.1.1-rc.1 v1.2.1-rc.1 v1.3.1-rc.1```

- Delete a tag from local repository: 
    - ```> git tag -d v1.1.1-rc.1```
- Delete multiple tags from local repository: 
    - ```> git tag -d $(git tag -l "v1*")```
