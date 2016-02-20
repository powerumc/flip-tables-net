Flip-Table-Net 은 자바 코드로 작성된 `flip-table`  을.NET 코드로 포팅한 프로젝트로, 콘솔에 데이터를 표로 표현해 줍니다.


### 설치

Command Line 에서 다음처럼 입력하거나,
```cmd
nuget install flip-tables-net
```

Visual Studio Package Manager Console 에서 다음처럼 입력합니다..

```cmd
Install-Package Flip-Tables-Net
```

또는 Nuget 패키지 관리자에서 flip-table-net 으로 검색합니다.


기존 자바에서 지원하던 기능
-----------------
`FlipTable`은 헤더 정보와 데이터 정보가 필요합니다.

```C#
string[] headers = { "Test", "Header" };
string[][] data =
{
    new[] {"Foo", "Bar"},
    new[] {"Kit", "Kat"}
};
Console.WriteLine(FlipTable.Of(headers, data));
```

```
+======+========+
| Test | Header |
+======+========+
| Foo  | Bar    |
+------+--------+
| Kit  | Kat    |
+======+========+
```

데이터에 개행 문자열도 지원합니다.
```C#
string[] headers = { "One Two\nThree", "Four" };
string[][] data = { new[] { "Five", "Six\nSeven Eight" } };
Console.WriteLine(FlipTable.Of(headers, data));
```
```
+=========+=============+
| One Two | Four        |
| Three   |             |
+=========+=============+
| Five    | Six         |
|         | Seven Eight |
+=========+=============+
```

그리고 테이블 안의 테이블도 지원합니다.

```C#
string[] innerHeaders = { "One", "Two" };
string[][] innerData = { new[] { "1", "2" } };
string inner = FlipTable.Of(innerHeaders, innerData);
string[] headers = { "Left", "Right" };
string[][] data = { new[] { inner, inner } };
Console.WriteLine(FlipTable.Of(headers, data));
```
```
+===============+===============+
| Left          | Right         |
+===============+===============+
| +=====+=====+ | +=====+=====+ |
| | One | Two | | | One | Two | |
| +=====+=====+ | +=====+=====+ |
| | 1   | 2   | | | 1   |   2 | |
| +=====+=====+ | +=====+=====+ |
|               |               |
+===============+===============+
```

.NET 으로 포팅하면서 추가된 기능
-----------------------
`flip-tables-net` 버전은 .NET 이 지원하는 객체를 사용할 수 있습니다.


`DataTable`, `DataSet` 을 사용하는 방법입니다.
```C#
var dt = new DataTable();
dt.Columns.Add("FirstName");
dt.Columns.Add("LastName");
dt.Columns.Add("Age");
var row1 = dt.NewRow();
row1["FirstName"] = "Junil";
row1["LastName"] = "Um";
row1["Age"] = 37;
dt.Rows.Add(row1);

Console.WriteLine(dt.FlipTablesFrom());
```
```
+===========+==========+=====+
| FirstName | LastName | Age |
+===========+==========+=====+
| Junil     | Um       | 37  |
+===========+==========+=====+
```

.NET `nested entity object` 객체는 다음과 같이 정의되어 있다면,

```C#
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public List<Person> Children { get; set; }
    public List<Name> Names { get; set; }

    public Person() { }
    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Children = new List<Person>();
        Names = new List<Name>() { new Name("A", "B") };
    }
}

public class Person2
{
    public Name Name { get; set; }
    public int Age { get; set; }
}

public class Name
{
    public string First { get; set; }
    public string Last { get; set; }

    public Name() { }

    public Name(string first, string last)
    {
        First = first;
        Last = last;
    }
}
```


```C#
var person2 = new Person2()
{
    Name = new Name("Junil", "Um"),
    Age = 37
};
Console.WriteLine(person2.FlipTablesFrom());
```

```
+==================+=====+
| Name             | Age |
+==================+=====+
| +=======+======+ | 37  |
| | First | Last | |     |
| +=======+======+ |     |
| | Junil |   Um | |     |
| +=======+======+ |     |
|                  |     |
+==================+=====+
```

복합적인 데이터가 담긴 `entity model` 와 `List<>` 객체입니다.
```C#
var personList = new List<Person>
{
    new Person("Junil", "Um", 37),
};
personList[0].Children.Add(new Person("A", "B", 12));
Console.WriteLine(personList.FlipTablesFrom());
```


```
+===========+==========+=====+==============================================================+==================+
| FirstName | LastName | Age | Children                                                     | Names            |
+===========+==========+=====+==============================================================+==================+
|     Junil |       Um |  37 | +===========+==========+=====+==========+==================+ | +=======+======+ |
|           |          |     | | FirstName | LastName | Age | Children |            Names | | | First | Last | |
|           |          |     | +===========+==========+=====+==========+==================+ | +=======+======+ |
|           |          |     | |         A |        B |  12 |          | +=======+======+ | | |     A |    B | |
|           |          |     | |           |          |     |          | | First | Last | | | +=======+======+ |
|           |          |     | |           |          |     |          | +=======+======+ | |                  |
|           |          |     | |           |          |     |          | |     A |    B | | |                  |
|           |          |     | |           |          |     |          | +=======+======+ | |                  |
|           |          |     | |           |          |     |          |                  | |                  |
|           |          |     | +===========+==========+=====+==========+==================+ |                  |
|           |          |     |                                                              |                  |
+===========+==========+=====+==============================================================+==================+
```


`FlipTablePad` 옵션

```C#
var person2 = new Person2()
{
	Name = new Name("Junil", null),
	Age = 37
};
Console.WriteLine(person2.FlipTablesFrom(FlipTablesPad.Right));
```

```
+====================+=====+
|               Name | Age |
+====================+=====+
| +=======+========+ |  37 |
| | First |   Last | |     |
| +=======+========+ |     |
| | Junil | (null) | |     |
| +=======+========+ |     |
|                    |     |
+====================+=====+
```
