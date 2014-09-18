using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class ExpandoObjectExtensions
    {
        public static dynamic[] ToExpandoObject(this object[] self)
        {
            return self.Select(item => item.ToExpandoObject()).ToArray();
        }

        public static dynamic ToExpandoObject(this object self)
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;

            foreach (var propertyInfo in self.GetType().GetProperties())
            {
                dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(self, null));
            }

            return dictionary;
        }
    }
}