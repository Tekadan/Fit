using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NutritionBoi.Web.Models
{
    public abstract class PCookieable<T> where T : new()
    {
        public static HttpCookie ToCookie(T tModel)
        {
            Type type = tModel.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());

            HttpCookie cookie = new HttpCookie(type.Name);
            cookie.Expires = DateTime.Now.AddDays(14d);

            foreach (PropertyInfo prop in props)
            {
                // Skip if the property is tagged with nonserialized
                if (prop.IsDefined(typeof(NonSerializedAttribute)))
                {
                    continue;
                }
                cookie[prop.Name] = prop.GetValue(tModel, null).ToString();
            }

            return cookie;
        }

        public static T FromCookie(HttpCookie tCookie)
        {
            T tModel = new T();

            Type type = tModel.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                // Don't try to read if we did not serialize this property
                if (!prop.IsDefined(typeof(NonSerializedAttribute)))
                {
                    // Ensure that that property is in the cookie and has a setter
                    if (tCookie[prop.Name] != null && prop.GetSetMethod() != null)
                    {
                        prop.SetValue(tModel, Convert.ChangeType(tCookie[prop.Name], prop.PropertyType), null);
                    }
                }
            }

            return tModel;
        }
    }
}