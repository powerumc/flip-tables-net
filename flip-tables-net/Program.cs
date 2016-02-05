using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace flip_tables_net
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                String[] headers = {"Test", "Header"};
                String[][] data =
                {
                    new[] {"Foo", "Bar"},
                    new[] {"Kit", "Kat"}
                };
                Console.WriteLine(FlipTable.Of(headers, data));
            }

            {
                String[] headers = {"One Two\nThree", "Four"};
                String[][] data = {new[]{"Five", "Six\nSeven Eight"}};
                Console.WriteLine(FlipTable.Of(headers, data));
            }
        }
    }
}
