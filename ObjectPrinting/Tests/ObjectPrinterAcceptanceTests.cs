using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		[Test]
		public void Demo()
		{
			var person = new Person
            {
                Name = "Alex", Age = 19
			};

		    var printer = ObjectPrinter.For<Person>()
		        //1. Исключить из сериализации свойства определенного типа
		        .Excluding<int>()
		        //2. Указать альтернативный способ сериализации для определенного типа
		        .Printing<float>().With(x => x.ToString())
		        //3. Для числовых типов указать культуру
		        .Printing<double>().With(CultureInfo.CurrentUICulture)
		        //4. Настроить сериализацию конкретного свойства
		        .Printing(p => p.Name).With(x => x.ToUpper())
		        //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
		        .Printing<string>().RestrictingLengthTo(10)
		        //6. Исключить из сериализации конкретного свойства
		        .Excluding(p => p.Id);

			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
		    string str1 = person.PrintToString();
		    //8. ...с конфигурированием
		    string str2 = person.PrintToString(pr => pr.Excluding<int>());
		}
	}

    [TestFixture]
    public class PrinterShould
    {
        private static Person _person;
        private static ObjectPrinter _printer;
        [SetUp]
        public void SetUp()
        {
            _person = new Person
            {
                Name = "Valera",
                Age = 19,
                Height = 1.88,
                Id = new Guid(),
                Parent = new Person
                {
                    Name = "Valera's dad",
                    Age = 42,
                    Height = 1.85,
                    Id = new Guid(),
                    Parent = null,
                    Married = true
                },
                Married = false
                
            };
        }

        [Test]
        public static void PrintAccordingToDefaultFormat_WhenNoConfig()
        {
            var expected =
                "Person\r\n" +
                "-Id = 00000000-0000-0000-0000-000000000000\r\n" +
                "-Name = Valera\r\n" +
                "-Height = 1,88\r\n" +
                "-Age = 19\r\n" +
                "-Parent : Person\r\n" +
                "---Id = 00000000-0000-0000-0000-000000000000\r\n" +
                "---Name = Valera's dad\r\n" +
                "---Height = 1,85\r\n" +
                "---Age = 42\r\n" +
                "---Parent : null\r\n" +
                "---Married = True\r\n" +
                "-Married = False\r\n";
            _person.PrintToString().Should().Be(expected);
        }

        [Test]
        public static void TrimStringFields()
        {
            var expected = "Person\r\n" +
                           "-Id = 00000000-0000-0000-0000-000000000000\r\n" +
                           "-Name = Val\r\n" +
                           "-Height = 1,88\r\n" +
                           "-Age = 19\r\n" +
                           "-Parent : Person\r\n" +
                           "---Id = 00000000-0000-0000-0000-000000000000\r\n" +
                           "---Name = Val\r\n" +
                           "---Height = 1,85\r\n" +
                           "---Age = 42\r\n" +
                           "---Parent : null\r\n" +
                           "---Married = True\r\n" +
                           "-Married = False\r\n";
            _person.PrintToString(pr => pr.Printing<string>().RestrictingLengthTo(3)).Should().Be(expected);
        }

        [Test]
        public static void ApplyUniqueStringSerialization()
        {
            var expected =
                "Person\r\n" +
                "-Id = 00000000-0000-0000-0000-000000000000\r\n" +
                "-Name = VALERA\r\n" +
                "-Height = 1,88\r\n" +
                "-Age = 19\r\n" +
                "-Parent : Person\r\n" +
                "---Id = 00000000-0000-0000-0000-000000000000\r\n" +
                "---Name = VALERA'S DAD\r\n" +
                "---Height = 1,85\r\n" +
                "---Age = 42\r\n" +
                "---Parent : null\r\n" +
                "---Married = True\r\n" +
                "-Married = False\r\n";
            _person.PrintToString(pr => pr.Printing<string>().With(p => p.ToUpper()));
        }

        [Test]
        public static void ExcludeGuid_WhenGivenTypeGuid()
        {
            var expected =
                "Person\r\n" +
                "-Name = Valera\r\n" +
                "-Height = 1,88\r\n" +
                "-Age = 19\r\n" +
                "-Parent : Person\r\n" +
                "---Name = Valera's dad\r\n" +
                "---Height = 1,85\r\n" +
                "---Age = 42\r\n" +
                "---Parent : null\r\n" +
                "---Married = True\r\n" +
                "-Married = False\r\n";
            var printer = ObjectPrinter.For<Person>().Excluding<Guid>();
            printer.PrintToString(_person).Should().Be(expected);
        }

        [Test]
        public static void ApplyNumericCulture_WhenGivenEnglishCulture()
        {
            var expected = "Person\r\n" +
                           "-Id = 00000000-0000-0000-0000-000000000000\r\n" +
                           "-Name = Valera\r\n" +
                           "-Height = 1.88\r\n" +
                           "-Age = 19\r\n" +
                           "-Parent : Person\r\n" +
                           "---Id = 00000000-0000-0000-0000-000000000000\r\n" +
                           "---Name = Valera's dad\r\n" +
                           "---Height = 1.85\r\n" +
                           "---Age = 42\r\n" +
                           "---Parent : null\r\n" +
                           "---Married = True\r\n" +
                           "-Married = False\r\n";
            _person.PrintToString(pr => pr.Printing<double>().With(new CultureInfo("en-EN"))).Should().Be(expected);
        }
    }
}
