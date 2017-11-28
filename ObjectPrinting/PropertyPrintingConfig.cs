using System;
using System.Reflection;


namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropertyType> : IPropertyPrintingConfig<TOwner, TPropertyType>
    {
        private readonly PrintingConfig<TOwner> outerContext;
        private readonly PropertyInfo property;

        public PropertyPrintingConfig(PrintingConfig<TOwner> outerContext, PropertyInfo propInfo = null)
        {
            this.outerContext = outerContext;
            property = propInfo;
        }

        public PrintingConfig<TOwner> With(Func<TPropertyType, string> serializingMethod)
        {
            if (property == null)
                outerContext.SetTypeSerialization(typeof(TPropertyType), serializingMethod);
            else
                outerContext.SetPropertySerialization(property, serializingMethod);

            return outerContext;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropertyType>.ParentConfig => outerContext;
    }
}
