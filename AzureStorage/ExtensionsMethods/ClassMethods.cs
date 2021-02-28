using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.ExtensionsMethods
{
    public static class ClassMethods
    {
        public static string PrintObject<T>(this T entry) where T : class
        {
            var propertiesObject = typeof(T).GetProperties();

            List<string> values = new List<string>();

            foreach (var property in propertiesObject)
            {

                object value = property.GetValue(entry) ?? "NULL";

                values.Add($"{property.Name}: {value}");
            }

            return string.Join(Environment.NewLine, values);
        }
    }
}
