﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flip_tables_net
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] headers = { "Test", "Header" };
            String[][] data = {
                new[] { "Foo", "Bar" },
                new[] { "Kit", "Kat" }
            };
            Console.WriteLine(FlipTable.of(headers, data));
        }
    }
}