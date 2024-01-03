# What is this?

### - Large File Manager
This program is not necessary for everyone. It is a useful application for developers who want to archive and manage their own `unitypackage` files


>  Imagine you have 100,000 files with a .unitypackage or your .unityapckage in .zip or .7z .. etc , You can only infer from the filename whether it's 2D, animated, stylized, etc (This is because there is no metadata, so you can only infer from the filename whether it is a 2D, animated, or stylized file.)
In this case, we would have to look at the file names directly and make inferences. For example, if it's called `RPG Medieval 3D Animation`, we don't know if it's called `RPG Medieval 3D Assets` or `Animation` until we recognize the word `Animation`.
 
**This tool uses REGEX to infer the version of a file, tags, and categorize it**
-----------------
### - Sync Dir Manager (Optional)

 This program streams folders on your local disk to a network drive. A maximum of `n threads` can be specified, and the number of concurrents is allowed via `SemaphoreSlim` in C#.
I use it to upload only newly updated files, as it's a pain in the ass to manually backup my numerous assets/files to `dropbox`, `web-dav (nas)`, etc.

**This saves network traffic and eliminates the need to keep Windows File Explorer or FileZila open**

-------------------------------------
## This asset manager therefore performs the following roles

- Performs auto-tagging, name-based search, and `Regex`-based search for files with the `.unitypackage` extension.
> For example, the file `RPG Medieval 3D Animation` is tagged with 3D, Animation. If a tagging-based search were to find the `3D` asset, it would be excluded from the search because it contains `Animation`.
- Includes functionality to correctly unzip `.unitypackage` extensions inside `.zip` files.
- **All operations are thread-optimized so they are very fast.** 



## How to use - Large File Manager
Here's an example of a file tree
```
c:\
 ㄴ your_assets
   ㄴ A
    ㄴ included +1000 .unitypackages
   ㄴ B
    ㄴ include +100 unity apckages and other directories
     ㄴ C...
```
     
`root folder is c:\your_assets`

- `.exe -p [PATH]`
- `.exe -p [PATH] -r ".*\.unitypackage|.zip|.7z"`
- 
  **Warning : if you want fastest tree search, re-write simple regex ex)"[".*\.unitypackage]**
- `.exe -p [PATH, ex) c:\unityassets] -r ".*\.unitypackage|.zip|.7z"`
--------------------------
## How to use - Sync Dir Manager
 soon.
 
## When Executed, You can input command this

- `list` - Show All Files
- `tag` - Search by Tag
> ex ) `tag` 2D,RPG` | `tag` `3D`
- `name` - Search by Name
- `unzip` - Extract All Files (Detects only if there is a .unitypackage in the zip file.)
- `unzip-allow-delete` - Extract All Files And Delete .zip File (Detects only if there is a .unitypackage in the zip file.)
 
## This is the result of running a tagging and name search after browsing over 10,000 files.
![image](https://github.com/shlifedev/unity-large-assets-manager/assets/49047211/19708959-9d46-4596-85a8-e88591e7edf2)
![image](https://github.com/shlifedev/unity-large-assets-manager/assets/49047211/d081c904-0ab8-4c69-8f06-2b6e3148e084)


### I Will Develop 
- Find files in the Unity Asset Store based on their filename and generate metadata.
- Create a faster file tree traversal algorithm, or add the --cache option.
