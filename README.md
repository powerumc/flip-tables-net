flip-tables-net
====================

Flip-Tables-Net converted from flip-table in Java code, should be make pretty-tables on console.


### Installation

in the command line
```cmd
nuget install flip-tables-net
```

or in the visual studio package manager console.

```cmd
Install-Package Flip-Tables-Net
```


Usage likes Java
-----------------
A `FlipTable` requires headers and data in string form:

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

New lines are supported
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

which means tables can be nested.

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

For the .NET
-----------------------
flip-tables-net version supported .NET specification objects.

From .NET `DataTable` or `DataSet` object.
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

From .NET `nested entity object`
First, Entity model classes is:
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

Complex objects contains `entity model` and `List<>` objects.
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


`FlipTablePad` options

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
