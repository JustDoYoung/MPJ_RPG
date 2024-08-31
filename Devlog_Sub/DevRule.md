1. 스크립트, 에셋, 씬 등을 관리하는 폴더 이름 앞에 "@"를 삽입한다.
- 예) 에셋을 관리하는 폴더의 경우 "@Resources"로 한다. Addressable 기반으로 관리할 것이기 때문에  빌드할 때 포함되지 않기 위해서다.

2. [리소스 관리](https://github.com/JustDoYoung/MPJ_RPG/blob/main/Devlog_Sub/ResourceManage.md "리드미")
- Addressable 기반으로 관리한다.
- 에셋번들로 리소스를 관리해 추후 효율적인 패치 시스템을 구축할 수 있다.

3. 클래스 상속구조에서 초기화 함수관리
최상층의 부모 클래스에서 Init()를 가상함수로 만들어 관리한다.
초기화 함수에는 처음 씬, 객체가 로드될 때 필수로 있어야 할 요소들을 체크하고 생성하는 역할을 삽입한다.(ex. UI 관련 클래스라면 EventSystem의 현재 씬에 있는 지 체크하고 생성한다.)

초기화 함수 역시 많은 클래스에 삽입될 수 있으므로 InitBase라는 별도의 클래스로 관리한다.

4. 확장함수(Utils/Extension.cs)
자주 반복적으로 사용되는 코드를 래핑해서 정적함수나 확장함수로 만들어두면 무지 편하다...