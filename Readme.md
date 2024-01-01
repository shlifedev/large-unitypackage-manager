
# What is this?
This program is not necessary for everyone. It is a useful application for developers who want to archive and manage their own `unitypackage` files


>  Imagine you have 100,000 files with a .unitypackage hidden inside a random .unityapckage or .zip, and the information in those files is not categorized as to whether they are 3D, Animation, 2D, etc.
In this case, we would have to look at the file names directly and make inferences. For example, if it's called `RPG Medieval 3D Animation`, we don't know if it's called `RPG Medieval 3D Assets` or `Animation` until we recognize the word `Animation`.

Translated with www.DeepL.com/Translator (free version)


## This asset manager therefore performs the following roles

- Performs auto-tagging, name-based search, and `Regex`-based search for files with the `.unitypackage` extension.
> For example, the file `RPG Medieval 3D Animation` is tagged with 3D, Animation. If a tagging-based search were to find the `3D` asset, it would be excluded from the search because it contains `Animation`.
- Includes functionality to correctly unzip `.unitypackage` extensions inside `.zip` files.
- **All operations are thread-optimized so they are very fast.**

Translated with www.DeepL.com/Translator (free version)



## How to use
- `.exe -p [PATH] -r "[".*\\.unitypackage|zip|7z]"`

  **Warning : if you want fastest tree search, re-write simple regex ex)"[".*\\.unitypackage]**
- `.exe -p [PATH, ex) c:\unityassets] -r "[".*\\.unitypackage]"`
## When Executed, You can input command this
- `list` - Show All Files
- `tag` - Search by Tag
> ex ) `tag` 2D,RPG` | `tag` `3D`
- `name` - Search by Name
- `unzip` - Extract All Files
- `unzip-allow-delete` - Extract All Files And Delete .zip File

- 
## This is the result of running a tagging and name search after browsing over 10,000 files.
![image](https://github.com/shlifedev/unity-large-assets-manager/assets/49047211/19708959-9d46-4596-85a8-e88591e7edf2)
![image](https://github.com/shlifedev/unity-large-assets-manager/assets/49047211/d081c904-0ab8-4c69-8f06-2b6e3148e084)


### I Will Develop 
- 파일 이름을 기반으로 유니티 에셋 스토어에서 파일을 찾고 메타데이터를 생성합니다.
- 좀 더 빠른 파일 트리 탐색 알고리즘을 만듭니다. 혹은 --cache 옵션을 추가합니다.

