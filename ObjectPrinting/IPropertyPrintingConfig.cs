namespace ObjectPrinting
{
    public interface IPropertyPrintingConfig<T, T1>
    {
        PrintingConfig<T> ParentConfig { get; }
    }
}