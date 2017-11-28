﻿using System;

namespace ObjectPrinting
{
    public static class ObjectExtension
    {
        public static string PrintToString<T>(this T obj) => ObjectPrinter.For<T>().PrintToString(obj);

        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> configurePrinting)
            => configurePrinting(ObjectPrinter.For<T>()).PrintToString(obj);
    }
}
