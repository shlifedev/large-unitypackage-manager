
# 이 프로그램은 무엇인가요?
모든 사람에게 필요한 프로그램은 아닙니다. `unitypackage` 파일을 직접 보관하고 관리하는 개발자에게 유용한 어플리케이션 입니다.

> 무작위의 .unityapckage 혹은 .zip안에 .unitypackage가 숨겨진 10만개의 파일이 있습니다. 그 파일의 정보가 3D인지, Animation인지, 2D인지 등 구분되어 있지 않다고 생각해보십시오.
이 경우, 우리는 직접 파일의 이름을 보고 유추해야합니다. 예를 들어 `RPG Medieval 3D Animation` 인 경우 `Animation` 이라는 단어를 인지하기 전까지는 이 파일이 `RPG Medieval 3D Assets` 인지 `Animation` 인지 알 수 없습니다.

이 에셋 매니저는 따라서 다음의 역할을 수행합니다.

- `.unitypackage` 확장자를 가진 파일에 대해서 오토 Tagging, 이름 기반 검색, `Regex` 기반 검색을 수행합니다.
> 예를 들어 `RPG Medieval 3D Animation` 파일에 대해서는 3D, Animation이 태깅됩니다. 태깅 기반 검색시 `3D` 에셋을 찾는다면 `Animation`이 포함되어있기에 검색에서 제외됩니다.
- `.zip`파일안에 있는 `.unitypackage` 확장자를 올바르게 압축 해제하는 기능이 포함되어 있습니다.
- **모든 작업은 스레드 최적화 되어있어 매우 빠릅니다.**



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
## 1만개가 넘는 파일에 대해서 탐색을 한 이후, 태깅, 이름 검색 실행 결과입니다.
![image](https://github.com/shlifedev/unity-large-assets-manager/assets/49047211/19708959-9d46-4596-85a8-e88591e7edf2)
![image](https://github.com/shlifedev/unity-large-assets-manager/assets/49047211/d081c904-0ab8-4c69-8f06-2b6e3148e084)


### 곧 지원할 예정
- 파일 이름을 기반으로 유니티 에셋 스토어에서 파일을 찾고 메타데이터를 생성합니다.
- 좀 더 빠른 파일 트리 탐색 알고리즘을 만듭니다. 혹은 --cache 옵션을 추가합니다.

