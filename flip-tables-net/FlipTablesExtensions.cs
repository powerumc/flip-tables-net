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
		    var ret = "";
		    if (!obj.GetType().IsPrimitive && obj.GetType() != typeof(string) && obj is IEnumerable)
		    {
			    var enumerable = (IEnumerable) obj;
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
						    value = FlipTablesFrom(value, pad);
					    }

					    data.Add(value.ToString());
				    }

				    list.Add(data.ToArray());
			    } while (enumerator.MoveNext());

			    ret += FlipTable.Of(headerProperty.Select(o => o.Name).ToArray(), list.ToArray(), pad);

		    }
		    else if (!obj.GetType().IsPrimitive && obj.GetType() != typeof(string) && obj.GetType().IsClass)
		    {
			    ret += FlipTablesFromObject(obj, pad);
		    }

		    return ret;
	    }

	    internal static string FlipTablesFromObject<T>(this T obj, FlipTablesPad pad = FlipTablesPad.Left) where T : class
	    {
		    var headerProperty = obj.GetType().GetProperties();

			var data = new List<string>();
			foreach (var header in headerProperty)
		    {
			    var value = header.GetValue(obj, null);
			    var type = (value ?? "(null)").GetType();
			    if (type != typeof(string) && type.IsClass)
			    {
				    value = FlipTablesFromObject(value, pad);
			    }

				data.Add((value ?? "(null)").ToString());
		    }

		    return FlipTable.Of(headerProperty.Select(o => o.Name).ToArray(), new[] {data.ToArray()}, pad);
	    }

	    public static string FlipTablesFrom(this DataTable dataTable, FlipTablesPad pad = FlipTablesPad.Left)
	    {
		    var headers = dataTable.Columns.OfType<DataColumn>().AsEnumerable().Select(o => o.ColumnName).ToArray();
		    return FlipTable.Of(headers,
			    (from DataRow r in dataTable.Rows select headers.Select(h => r[h].ToString()).ToArray()).ToArray());
	    }

	    public static string FlipTablesFrom(this DataSet dataSet, FlipTablesPad pad = FlipTablesPad.Left)
	    {
		    var sb = new StringBuilder(1024);
		    foreach (DataTable dt in dataSet.Tables)
		    {
			    sb.AppendLine(FlipTablesFrom(dt, pad));
		    }
		    sb.AppendLine();

		    return sb.ToString();
	    }
    }
}