using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentAssertions.Common;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig
    {
        private readonly HashSet<Type> finalTypes = new HashSet<Type>
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(bool)
        };
        private readonly HashSet<Type> allowedNumericTypes = new HashSet<Type>
        {
            typeof(double), typeof(int), typeof(long)
        };

        private readonly Dictionary<Type, Delegate> customTypeSerialization = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, CultureInfo> customNumericCulture = new Dictionary<Type, CultureInfo>();
        private readonly Dictionary<PropertyInfo, Delegate> customPropertySerialization = new Dictionary<PropertyInfo, Delegate>();

        private readonly HashSet<Type> excludedTypes = new HashSet<Type>();
        private readonly HashSet<PropertyInfo> excludedProperties = new HashSet<PropertyInfo>();
        private int stringPropertyLength = -1;


        public string PrintToString(TOwner obj) => PrintToString(obj, 0);

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropertyType>(
            Expression<Func<TOwner, TPropertyType>> memberSelector)
        {
            excludedProperties.Add(memberSelector.GetPropertyInfo());
            return this;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>() => new PropertyPrintingConfig<TOwner, TPropType>(this);

        public PropertyPrintingConfig<TOwner, TPropertyType> Printing<TPropertyType>(
            Expression<Func<TOwner, TPropertyType>> memberSelector)
            => new PropertyPrintingConfig<TOwner, TPropertyType>(this, memberSelector.GetPropertyInfo());

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('-', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                var propertyIsExcluded = excludedProperties.Contains(propertyInfo) ||
                                         excludedTypes.Contains(propertyInfo.PropertyType);
                if (propertyIsExcluded) continue;

                sb.Append(identation + propertyInfo.Name +
                    (finalTypes.Contains(propertyInfo.PropertyType) ? " = " : " : ") +
                              SerializeProperty(obj, propertyInfo,
                                  nestingLevel + 1));
            }
            return sb.ToString();
        }

        private string SerializeProperty(object obj, PropertyInfo property, int nestingLevel)
        {
            var propValue = property.GetValue(obj);
            var propType = property.PropertyType;
            string result = null;

            if (customPropertySerialization.ContainsKey(property))
                result = (string)customPropertySerialization[property].DynamicInvoke(propValue);
            if (customTypeSerialization.ContainsKey(propType))
                result = (string)customTypeSerialization[property.PropertyType].DynamicInvoke(propValue);
            if (customNumericCulture.ContainsKey(propType))
                result = ((IFormattable) propValue).ToString("", customNumericCulture[propType]);
            if (propType == typeof(string) && stringPropertyLength > 0)
                result = string.Concat(((string)propValue).Take(stringPropertyLength));

            return result == null ? PrintToString(propValue, nestingLevel + 1) : result + Environment.NewLine;
        }

        public void SetAllowedStringLength(int length)
        {
            stringPropertyLength = length;
        }

        public void SetTypeSerialization(Type type, Delegate serializationMethod)
        {
            if (!customTypeSerialization.ContainsKey(type))
                customTypeSerialization[type] = serializationMethod;
            else
                customTypeSerialization.Add(type, serializationMethod);
        }

        public void SetPropertySerialization(PropertyInfo prop, Delegate serializationMethod)
        {
            if (!customPropertySerialization.ContainsKey(prop))
                customPropertySerialization[prop] = serializationMethod;
            else
                customPropertySerialization.Add(prop, serializationMethod);
        }

        public void SetNumericCulture(Type type, CultureInfo culture)
        {
            if (!allowedNumericTypes.Contains(type))
                return;
            if (!customNumericCulture.ContainsKey(type))
                customNumericCulture[type] = culture;
            else
                customNumericCulture.Add(type, culture);
        }
    }
}
