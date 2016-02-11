using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FlipTablesNet
{

/**
 * <pre>
 * ╔═════════════╤════════════════════════════╤══════════════╗
 * ║ Name        │ Function                   │ Author     ║
 * ╠═════════════╪════════════════════════════╪══════════════╣
 * ║ Flip Tables │ Pretty-print a text table. │ Jake Wharton ║
 * ╚═════════════╧════════════════════════════╧══════════════╝
 * </pre>
 */
    public sealed partial class FlipTable
    {
	    private readonly FlipTablesPad pad;
	    private static readonly string EMPTY = "(empty)";

        /** Create a new table with the specified headers and row data. */
        public static string Of(string[] headers, string[][] data, FlipTablesPad pad = FlipTablesPad.Left)
        {
            if (headers == null) throw new NullReferenceException("headers == null");
            if (headers.Length == 0) throw new ArgumentException("Headers must not be empty.");
            if (data == null) throw new NullReferenceException("data == null");
            return new FlipTable(headers, data, pad).ToString();
        }



        public static string Of(object obj, FlipTablesPad pad)
        {
            var ret = "";
            if (!obj.GetType().IsPrimitive && obj.GetType() != typeof(string) && obj is IEnumerable)
            {
                var enumerable = (IEnumerable)obj;
                var enumerator = enumerable.GetEnumerator();
                var list = new List<string[]>();

                var can = enumerator.MoveNext();
                if (!can) return "";

                var current = enumerator.Current;
                var headerProperty = current.GetType().GetProperties();

                do
                {
                    var data = new List<string>();
                    foreach (var header in headerProperty)
                    {
                        var value = header.GetValue(current, null) ?? "(null)";
                        if (!value.GetType().IsPrimitive && value.GetType() != typeof(string) && value is IEnumerable)
                        {
                            value = Of(value, pad);
                        }

                        data.Add(value.ToString());
                    }

                    list.Add(data.ToArray());
                } while (enumerator.MoveNext());

                ret += Of(headerProperty.Select(o => o.Name).ToArray(), list.ToArray(), pad);

            }
            else if (!obj.GetType().IsPrimitive && obj.GetType() != typeof(string) && obj.GetType().IsClass)
            {
                ret += Of<object>(obj, pad);
            }

            return ret;
        }

        private static string Of<T>(T obj, FlipTablesPad pad) where T : class
        {
            var headerProperty = obj.GetType().GetProperties();

            var data = new List<string>();
            foreach (var header in headerProperty)
            {
                var value = header.GetValue(obj, null);
                var type = (value ?? "(null)").GetType();
                if (type != typeof(string) && type.IsClass)
                {
                    value = Of(value, pad);
                }

                data.Add((value ?? "(null)").ToString());
            }

            return Of(headerProperty.Select(o => o.Name).ToArray(), new[] { data.ToArray() }, pad);
        }


        public static string Of(DataTable dataTable, FlipTablesPad pad)
        {
            var headers = dataTable.Columns.OfType<DataColumn>().AsEnumerable().Select(o => o.ColumnName).ToArray();
            return FlipTable.Of(headers,
                (from DataRow r in dataTable.Rows select headers.Select(h => r[h].ToString()).ToArray()).ToArray(), pad);
        }

        public static string Of(DataSet dataSet, FlipTablesPad pad)
        {
            var sb = new StringBuilder(1024);
            foreach (DataTable dt in dataSet.Tables)
            {
                sb.AppendLine(Of(dt, pad));
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private readonly string[] headers;
        private readonly string[][] data;
        private readonly int columns;
        private readonly int[] columnWidths;
        private readonly int emptyWidth;

        private FlipTable(string[] headers, string[][] data, FlipTablesPad pad)
        {
	        this.pad = pad;
	        this.headers = headers;
            this.data = data;

            columns = headers.Length;
            columnWidths = new int[columns];
            for (int row = -1; row < data.Length; row++)
            {
                string[] rowData = (row == -1) ? headers : data[row]; // Hack to parse headers too.
                if (rowData.Length != columns)
                {
                    throw new ArgumentException(
                        $"Row {row + 1}'s {rowData.Length} columns != {columns} columns");
                }
                for (int column = 0; column < columns; column++)
                {
                    foreach (string rowDataLine in rowData[column].Split('\n'))
                    {
                        columnWidths[column] = Math.Max(columnWidths[column], rowDataLine.Length);
                    }
                }
            }

            var emptyWidth = 3 * (columns - 1); // Account for column dividers and their spacing.
            foreach (int columnWidth in columnWidths)
            {
                emptyWidth += columnWidth;
            }
            this.emptyWidth = emptyWidth + 2;

            if (emptyWidth < EMPTY.Length)
            { // Make sure we're wide enough for the empty text.
                columnWidths[columns - 1] += EMPTY.Length - emptyWidth;
            }
        }

	    private string padding(string str, int totalWidth)
	    {
		    switch (this.pad)
		    {
			    case FlipTablesPad.Right:
				    return str.PadLeft(totalWidth);

			    case FlipTablesPad.Left:
				    return str.PadRight(totalWidth);
		    }

		    return str.PadLeft(totalWidth);
	    }

        public override string ToString()
        {
            var builder = new StringBuilder();
            //printDivider(builder, "╔═╤═╗");
	        //if (data.Length != 0)
	        //{
		        printDivider(builder, "+=+=+");
		        printData(builder, headers);
	        //}
	        if (data.Length == 0)
            {
				//printDivider(builder, "╠═╧═╣");
				printDivider(builder, "+=+=+");
				//builder.Append('║').Append(pad(emptyWidth, EMPTY)).Append("║\n");
				builder.Append("|").Append(padding(EMPTY, emptyWidth)).Append("|\n");
				//printDivider(builder, "╚═══╝");
				printDivider(builder, "+===+");
			}
            else {
                for (int row = 0; row < data.Length; row++)
                {
                    //printDivider(builder, row == 0 ? "╠═╪═╣" : "╟─┼─╢");
                      printDivider(builder, row == 0 ? "+=+=+" : "+-+-+");
                    printData(builder, data[row]);
                }
                //printDivider(builder, "╚═╧═╝");
                  printDivider(builder, "+=+=+");
            }
            return builder.ToString();
        }

        private void printDivider(StringBuilder @out, string format)
        {
            for (int column = 0; column < columns; column++)
            {
                @out.Append(column == 0 ? format[0] : format[2]).Append(format[1]).Append(format[1]);
                //@out.Append(pad(columnWidths[column], "").Replace(' ', format.CharAt(1)));
                @out.Append(padding("", columnWidths[column]).Replace(' ', format[1]));
            }
            @out.Append(format[4]).Append('\n');
        }

        private void printData(StringBuilder @out, string[] data)
        {
            for (int line = 0, lines = 1; line < lines; line++)
            {
                for (int column = 0; column < columns; column++)
                {
                    @out.Append(column == 0 ? "|" : "|");
                    var cellLines = (data[column] ?? "").Split('\n');
                    lines = Math.Max(lines, cellLines.Length);
                    var cellLine = line < cellLines.Length ? cellLines[line] ?? "(null)" : "";
                    //@out.Append(pad(columnWidths[column], cellLine));
                    //@out.Append(cellLine.PadLeft(columnWidths[column]));
                    @out.Append(' ').Append(padding(cellLine, columnWidths[column])).Append(' ');
                }
                //@out.Append("║\n");
                @out.Append("|\n");
            }
        }
    }
}
