using System;
using System.Collections.Generic;
using System.Data;
using FlipTablesNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flip_tables_net.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			{
				string[] headers = { "Test", "Header" };
				string[][] data =
				{
					new[] {"Foo", "Bar"},
					new[] {"Kit", "Kat"}
				};
				Console.WriteLine(FlipTable.Of(headers, data));
			}

			{
				string[] headers = { "One Two\nThree", "Four" };
				string[][] data = { new[] { "Five", "Six\nSeven Eight" } };
				Console.WriteLine(FlipTable.Of(headers, data));
			}

			{
				string[] innerHeaders = { "One", "Two" };
				string[][] innerData = { new[] { "1", "2" } };
				string inner = FlipTable.Of(innerHeaders, innerData);
				string[] headers = { "Left", "Right" };
				string[][] data = { new[] { inner, inner } };
				Console.WriteLine(FlipTable.Of(headers, data));
			}

			{
				var personList = new List<Person>
				{
					new Person("Junil", "Um", 37),
				};
				personList[0].Children.Add(new Person("A", "B", 12));
				Console.WriteLine(personList.FlipTablesFrom());
			}

			{
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
			}

			{
				var person2 = new Person2()
				{
					Name = new Name("Junil", "Um"),
					Age = 37
				};
				Console.WriteLine(person2.FlipTablesFrom());
			}
		}
	}

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
}
