using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace FlipTablesNet
{
    public static class FlipTablesExtensions
    {
	    public static string FlipTablesFrom(this object obj)
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
						    value = FlipTablesFrom(value);
					    }

					    data.Add(value.ToString());
				    }

				    list.Add(data.ToArray());
			    } while (enumerator.MoveNext());

			    ret += FlipTable.Of(headerProperty.Select(o => o.Name).ToArray(), list.ToArray());

		    }
		    else if (!obj.GetType().IsPrimitive && obj.GetType() != typeof(string) && obj.GetType().IsClass)
		    {
			    ret += FlipTablesFromObject(obj);
		    }

		    return ret;
	    }

	    internal static string FlipTablesFromObject<T>(this T obj) where T : class
	    {
		    var headerProperty = obj.GetType().GetProperties();

			var data = new List<string>();
			foreach (var header in headerProperty)
		    {
			    var value = header.GetValue(obj, null);
			    var type = value.GetType();
			    if (type != typeof(string) && type.IsClass)
			    {
				    value = FlipTablesFromObject(value);
			    }

				data.Add((value ?? "(null)").ToString());
		    }

		    return FlipTable.Of(headerProperty.Select(o => o.Name).ToArray(), new[] {data.ToArray()});
	    }

	    public static string FlipTablesFrom(this DataTable dataTable)
	    {
		    var headers = dataTable.Columns.OfType<DataColumn>().AsEnumerable().Select(o => o.ColumnName).ToArray();
		    return FlipTable.Of(headers,
			    (from DataRow r in dataTable.Rows select headers.Select(h => r[h].ToString()).ToArray()).ToArray());
	    }

	    public static string FlipTablesFrom(this DataSet dataSet)
	    {
		    var sb = new StringBuilder(1024);
		    foreach (DataTable dt in dataSet.Tables)
		    {
			    sb.AppendLine(FlipTablesFrom(dt));
		    }
		    sb.AppendLine();

		    return sb.ToString();
	    }
    }
}