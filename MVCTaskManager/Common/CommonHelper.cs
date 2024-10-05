using System.Data;

namespace MVCTaskManager.Common
{
    public class CommonHelper
    {
        public static async Task<List<T>> ConvertDataTableToList<T>(DataTable table) where T : new()
        {
            try
            {
                return await Task.Run(() =>
                {
                    List<T> list = new List<T>();

                    foreach (DataRow row in table.Rows)
                    {
                        T obj = new T();

                        // Loop through the properties of the class
                        foreach (var prop in typeof(T).GetProperties())
                        {
                            if (table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                            {
                                // Set the property value from the DataTable row
                                prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType), null);
                            }
                        }

                        list.Add(obj);
                    }

                    return list;
                });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message, "ConvertDataTableToList Error Message");
                // _logger.LogError(ex.StackTrace, "ConvertDataTableToList Error StackTrace");
                return new List<T>();
            }
        }

    }
}
