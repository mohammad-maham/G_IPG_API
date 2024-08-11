using System.Data;
using System.Reflection;

namespace G_IPG_API.Common;

public static class DataTableExtention
{
    public static IEnumerable<T> AsEnumerable<T>(this DataTable dataTable) where T : new()
    {

        // Define a dictionary to cache the property info for the type T
        var propertyInfoCache = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p);

        // Iterate through each row in the DataTable
        foreach (DataRow row in dataTable.Rows)
        {
            // Create an instance of T
            var item = new T();

            // Iterate through each column in the row
            foreach (DataColumn column in dataTable.Columns)
            {
                if (propertyInfoCache.TryGetValue(column.ColumnName, out var propertyInfo))
                {
                    // Get the value from the row
                    var value = row[column];

                    // Set the property value if it's not null
                    if (value != DBNull.Value)
                    {
                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                        {
                            if (value is DateTime dateTimeValue)
                            {
                                propertyInfo.SetValue(item, dateTimeValue);
                            }
                            else
                            {
                                if (DateTime.TryParse(value.ToString(), out var parsedDateTime))
                                {
                                    propertyInfo.SetValue(item, parsedDateTime);
                                }
                            }
                        }
                        else
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(value, propertyInfo.PropertyType));
                        }
                    }
                }
            }

            // Yield return the item
            yield return item;
        }
    }

    public static T AsOne<T>(this DataTable dataTable) where T : new()
    {

        return dataTable.AsEnumerable<T>().FirstOrDefault()!;

    }

}

