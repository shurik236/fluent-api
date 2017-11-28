using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    interface IPrintingConfig
    {
        void SetAllowedStringLength(int length);

        void SetTypeSerialization(Type type, Delegate serializationMethod);

        void SetPropertySerialization(PropertyInfo prop, Delegate serializationMethod);

        void SetNumericCulture(Type type, CultureInfo culture);
    }
}
