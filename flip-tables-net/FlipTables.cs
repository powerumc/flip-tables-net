using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace flip_tables_net
{

    public static class StringExtensions
    {
        public static char CharAt(this string str, int index)
        {
            return str[index];
        }
    }

/**
 * <pre>
 * ╔═════════════╤════════════════════════════╤══════════════╗
 * ║ Name        │ Function                   │ Author       ║
 * ╠═════════════╪════════════════════════════╪══════════════╣
 * ║ Flip Tables │ Pretty-print a text table. │ Jake Wharton ║
 * ╚═════════════╧════════════════════════════╧══════════════╝
 * </pre>
 */
    public sealed class FlipTable
    {
        private static String EMPTY = "(empty)";

        /** Create a new table with the specified headers and row data. */
        public static String Of(String[] headers, String[][] data)
        {
            if (headers == null) throw new NullReferenceException("headers == null");
            if (headers.Length == 0) throw new ArgumentException("Headers must not be empty.");
            if (data == null) throw new NullReferenceException("data == null");
            return new FlipTable(headers, data).ToString();
        }

        private String[] headers;
        private String[][] data;
        private int columns;
        private int[] columnWidths;
        private int emptyWidth;

        private FlipTable(String[] headers, String[][] data)
        {
            this.headers = headers;
            this.data = data;

            columns = headers.Length;
            columnWidths = new int[columns];
            for (int row = -1; row < data.Length; row++)
            {
                String[] rowData = (row == -1) ? headers : data[row]; // Hack to parse headers too.
                if (rowData.Length != columns)
                {
                    throw new ArgumentException(
                        String.Format("Row {0}'s {1} columns != {2} columns", row + 1, rowData.Length, columns));
                }
                for (int column = 0; column < columns; column++)
                {
                    foreach (String rowDataLine in rowData[column].Split('\n'))
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
            this.emptyWidth = emptyWidth;

            if (emptyWidth < EMPTY.Length)
            { // Make sure we're wide enough for the empty text.
                columnWidths[columns - 1] += EMPTY.Length - emptyWidth;
            }
        }

        public override String ToString()
        {
            var builder = new StringBuilder();
            //printDivider(builder, "╔═╤═╗");
              printDivider(builder, "+=+=+");
            printData(builder, headers);
            if (data.Length == 0)
            {
                //printDivider(builder, "╠═╧═╣");
                  printDivider(builder, "+=+=+");
                //builder.Append('║').Append(pad(emptyWidth, EMPTY)).Append("║\n");
                builder.Append("|").Append(EMPTY.PadLeft(emptyWidth)).Append("|\n");
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
                @out.Append(column == 0 ? format.CharAt(0) : format.CharAt(2)).Append(format.CharAt(1)).Append(format.CharAt(1));
                //@out.Append(pad(columnWidths[column], "").Replace(' ', format.CharAt(1)));
                @out.Append("".PadLeft(columnWidths[column]).Replace(' ', format.CharAt(1)));
            }
            @out.Append(format.CharAt(4)).Append('\n');
        }

        private void printData(StringBuilder @out, string[] data)
        {
            for (int line = 0, lines = 1; line < lines; line++)
            {
                for (int column = 0; column < columns; column++)
                {
                    @out.Append(column == 0 ? "|" : "|");
                    var cellLines = data[column].Split('\n');
                    lines = Math.Max(lines, cellLines.Length);
                    var cellLine = line < cellLines.Length ? cellLines[line] : "";
                    //@out.Append(pad(columnWidths[column], cellLine));
                    //@out.Append(cellLine.PadLeft(columnWidths[column]));
                    @out.Append(' ').Append(cellLine.PadLeft(columnWidths[column])).Append(' ');
                }
                //@out.Append("║\n");
                @out.Append("|\n");
            }
        }
    }
}
