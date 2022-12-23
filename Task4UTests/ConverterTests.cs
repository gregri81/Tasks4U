using System.Globalization;
using Tasks4U.Converters;

namespace Task4UTests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void TestFirstLineConverter()
        {
            var firstLineConverter = new FirstLineConverter();
            
            // Assert that we take only the first line
            Assert.AreEqual("line1", 
                            firstLineConverter.Convert("line1\nline2", typeof(string), null, CultureInfo.InvariantCulture));

            // Assert that an empty string remains empty
            Assert.AreEqual("",
                            firstLineConverter.Convert("", typeof(string), 10, CultureInfo.InvariantCulture));


            // Assert that if there is only one line, we take the whole string
            Assert.AreEqual("line1",
                            firstLineConverter.Convert("line1", typeof(string), 10, CultureInfo.InvariantCulture));
        }
    }
}
