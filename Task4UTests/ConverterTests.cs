using System.Globalization;
using Tasks4U.Converters;

namespace Task4UTests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void TestSummaryConverter()
        {
            var summaryConverter = new SummaryConverter();
            
            // Assert that we take only the first line
            Assert.AreEqual("123", 
                            summaryConverter.Convert("123\n45678901", typeof(string), 10, CultureInfo.InvariantCulture));

            Assert.AreEqual("123",
                            summaryConverter.Convert("123\r45678901", typeof(string), 10, CultureInfo.InvariantCulture));

            // Assert that an empty string remains empty
            Assert.AreEqual("",
                            summaryConverter.Convert("", typeof(string), 10, CultureInfo.InvariantCulture));

            // Assert that if the first line is too long, we take up to n characters (according to the parameter)
            Assert.AreEqual("1234567890",
                            summaryConverter.Convert("12345678901\n2", typeof(string), 10, CultureInfo.InvariantCulture));

            // Assert that if the first line is too long, we take up to n characters (according to the parameter)
            Assert.AreEqual("1234567890",
                            summaryConverter.Convert("12345678901\n2", typeof(string), 10, CultureInfo.InvariantCulture));

            Assert.AreEqual("1234567890",
                            summaryConverter.Convert("12345678901", typeof(string), 10, CultureInfo.InvariantCulture));
        }
    }
}
