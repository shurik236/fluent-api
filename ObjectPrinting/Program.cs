using System;
using ObjectPrinting.Tests;

namespace ObjectPrinting
{
    class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person
            {
                Name = "Alex",
                Age = 19,
                Height = 1.88,
                Id = new Guid(),
                Parent = new Person
                {
                    Name = "Pavel",
                    Age = 42,
                    Height = 1.85,
                    Id = new Guid(),
                    Married = true
                },
               Married = false
                
            };
            Console.WriteLine(person.PrintToString(pr => pr.Excluding<int>().Printing<string>().With(s => s.ToUpper()).Printing(x => x.Name).RestrictingLengthTo(3)));
            Console.ReadKey();
        }
    }
}
