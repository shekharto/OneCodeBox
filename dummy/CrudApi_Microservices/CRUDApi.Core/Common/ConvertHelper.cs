using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Common
{
    public static class ConvertHelper
    {
        public static T Convert<T>(object value)
        {
            return (T)Convert(typeof(T), value);
        }
        public static object Convert(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }

    }
}
