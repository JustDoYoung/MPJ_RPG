### 네임스페이스와 라이브러리
using UnityEngine.Pool; 와 같이 using 키워드를 통해 가져오는 것은 **네임스페이스(namespace)**입니다. 네임스페이스는 클래스, 인터페이스, 구조체, 열거형 등을 논리적으로 그룹화하여 코드의 가독성과 관리성을 높이는 데 사용됩니다.

하지만, 네임스페이스가 포함된 파일이나 어셈블리 자체는 **라이브러리(library)**라고 부를 수 있습니다. 라이브러리는 여러 네임스페이스와 클래스를 포함하는 코드의 집합으로, 재사용 가능한 기능을 제공합니다. 예를 들어,
 UnityEngine.Pool
 
네임스페이스는 유니티 엔진의 객체 풀링 기능을 제공하는 라이브러리의 일부입니다.

정리하자면:네임스페이스(namespace): 클래스, 인터페이스, 구조체 등을 그룹화하는 논리적 단위입니다.
라이브러리(library): 여러 네임스페이스와 클래스를 포함하는 코드의 집합으로, 재사용 가능한 기능을 제공합니다.

### 래퍼 클래스
이미 있는 기능을 새로운 클래스로 감싸서 새로운 정책에 대응할 수 있음.
(ex. SceneManager의 씬 전환 기능을 비동기 처리로 할 지, 즉시 전환으로 할 지 바뀔 수 있는 상황을 SceneMangerEx 클래스로 감싸서 LoadScene 기능을 구현할 수 있다.)