### 1. ResourceManager.cs
> 게임 중간에 리소스를 로드하면 버벅거림이 생기기 때문에 처음 게임 로딩 창에서 필요한 리소스를 모두 로드한 후에 게임실행하도록 구현.

### 2. Addressable
#### (유니티에서 리소스 관리하는 방식)
- Resources 폴더
> 빌드할 때 이 폴더에 있는 리소스도 같이 포함됨.   
베포하는 관점에서 생각해볼 때   
앱스토어 심사 후 -> 유저 다운로드 의 과정을 통하는데   
추가적인 패치가 생겼고 이를 다시 빌드해 앱스토어 올린다면   
빌드시간 + 심사기간 + 유저 재다운로드 라는 비효율적인 과정을 거치게 됨.   

- 에셋번들(Addressable)
> 리소스들을 쪼개서 번들이라는 개념으로 관리.   
리소스의 수정/추가 사항이 업데이트되면 새로운 빌드파일을 앱스토어에 등록하고 다운받는 것이 아니라 패치파일만 다운받으면 된다.   

#### (Addressable을 스크립트에서 다룰 때 주의할 점)
> Addressable 리소스들을 Group에 등록할 때 key가 중복되지 않도록 네이밍에 신경쓴다. 예를 들어 sprite와 texture2D 파일이 있는데 이름이 같다면 "이름_sprite", "이름_texture2D" 같은 형식으로 짓는다.
Addressable 함수를 통해 로드한 리소스들을 메모리에서 해제하는 과정을 반드시 해줘야 메모리 누수를 방지할 수 있다.

### 3. PoolManager.cs
> 객체 풀링을 통해 자주 생성되고 파괴되는 객체를 미리 생성해두고 재사용하는 기법으로 성능을 최적화하는 데 유용하다.

