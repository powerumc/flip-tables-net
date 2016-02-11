using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using FlipTablesNet;

namespace System
{
    public static class FlipTablesExtensions
    {
	    public static string FlipTablesFrom(this object obj, FlipTablesPad pad = FlipTablesPad.Left)
        {
            return FlipTable.Of(obj, pad);
        }

        internal static string FlipTablesFromObject<T>(this T obj, FlipTablesPad pad = FlipTablesPad.Left) where T : class
        {
            return FlipTable.Of(obj, pad);
        }

        public static string FlipTablesFrom(this DataTable dataTable, FlipTablesPad pad = FlipTablesPad.Left)
        {
            return FlipTable.Of(dataTable, pad);
        }

        public static string FlipTablesFrom(this DataSet dataSet, FlipTablesPad pad = FlipTablesPad.Left)
        {
            return FlipTable.Of(dataSet, pad);
        }
    }
}