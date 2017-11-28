using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtension
    {
        public static PrintingConfig<TOwner> With<TOwner>(this IPropertyPrintingConfig<TOwner, double> propertyPrintingConfig, CultureInfo culture)
        {
            var outerContext = propertyPrintingConfig.ParentConfig;
            outerContext.SetNumericCulture(typeof(double), culture);
            return outerContext;
        }

        public static PrintingConfig<TOwner> With<TOwner>(this IPropertyPrintingConfig<TOwner, float> propertyPrintingConfig, CultureInfo culture)
        {
            var outerContext = propertyPrintingConfig.ParentConfig;
            outerContext.SetNumericCulture(typeof(float), culture);
            return outerContext;
        }

        public static PrintingConfig<TOwner> With<TOwner>(this IPropertyPrintingConfig<TOwner, int> propertyPrintingConfig, CultureInfo culture)
        {
            var outerContext = propertyPrintingConfig.ParentConfig;
            outerContext.SetNumericCulture(typeof(int), culture);
            return outerContext;
        }

        public static PrintingConfig<TOwner> RestrictingLengthTo<TOwner>(this IPropertyPrintingConfig<TOwner, string> propertyPrintingConfig, int length)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be positive");

            var outerContext = propertyPrintingConfig.ParentConfig;
            outerContext.SetAllowedStringLength(length);
            return outerContext;
        }
    }
}
