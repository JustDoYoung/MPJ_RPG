<img src="Images/데이터_프레임워크.png" alt="데이터프레임워크"></img><br/>

### (추가)
#### DataManager(데이터 시트)
데이터베이스는 유저별로 관리하는 데이터 관리
데이터 시트는 게임을 구성하는 수치들 관리

#### 리플랙션
클래스와 구조체 등과 같은 데이터의 정보를 조회할 때 사용.

#### (예제) 원하는 객체의 필드 정보를 조회
```c++
Type type = typeof(Data.TestData);
FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
    foreach (FieldInfo field in fields)
    {
        object value = field.GetValue(Managers.Data.TestDic[1]);

        print($"{field.Name} : {value}");
    }
```