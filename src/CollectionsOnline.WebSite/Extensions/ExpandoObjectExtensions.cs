using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class ExpandoObjectExtensions
    {
        public static dynamic[] ToExpandoObject(this object[] items)
        {
            return items.Select(item => item.ToExpandoObject()).ToArray();
        }

        public static dynamic ToExpandoObject(this object item)
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;

            foreach (var propertyInfo in item.GetType().GetProperties())
            {
                dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(item, null));
            }

            return dictionary;
        }
    }
}