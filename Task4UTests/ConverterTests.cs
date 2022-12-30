using System.Globalization;
using Tasks4U.Converters;
using Tasks4U.Models;
using TaskStatus = Tasks4U.Models.TaskStatus;

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

        [TestMethod]
        public void TestDateAndFrequencyConverter()
        {
            var dateAndFrequencyConverter = new DateAndFrequencyConverter();

            var convert = (Frequency frequency, DateOnly date) =>
                dateAndFrequencyConverter.Convert(new object[] { date, frequency }, typeof(string), null, CultureInfo.InvariantCulture);

            var today = DateOnly.FromDateTime(DateTime.Today);

            Assert.AreEqual("01/02/0003", convert(Frequency.Once, new DateOnly(3, 2, 1)));

            var nextMondayDate = today.AddDays((DayOfWeek.Monday - today.DayOfWeek + 7) % 7);

            Assert.AreEqual("Monday", convert(Frequency.EveryWeek, nextMondayDate));

            Assert.AreEqual("05", convert(Frequency.EveryMonth, new DateOnly(2024, 7, 5)));
            Assert.AreEqual("February 03", convert(Frequency.EveryYear, new DateOnly(2023, 2, 3)));

            Assert.AreEqual(string.Empty, convert(Frequency.Once, DateOnly.MinValue));
            Assert.AreEqual(string.Empty, convert(Frequency.EveryWeek, DateOnly.MinValue));
            Assert.AreEqual(string.Empty, convert(Frequency.EveryMonth, DateOnly.MinValue));
            Assert.AreEqual(string.Empty, convert(Frequency.EveryYear, DateOnly.MinValue));
        }

        [TestMethod]
        public void TestSplitByCapitalLettersConveter()
        {
            var splitByCapitalLettersConverter = new SplitByCapitalLettersConverter();

            var convert = (object value) => 
                splitByCapitalLettersConverter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.AreEqual(convert(Frequency.Once), "Once");
            Assert.AreEqual(convert(Frequency.EveryWeek), "Every Week");
            Assert.AreEqual(convert(Frequency.EveryMonth), "Every Month");
            Assert.AreEqual(convert(Frequency.EveryYear), "Every Year");

            Assert.AreEqual(convert(TaskStatus.NotStarted), "Not Started");
            Assert.AreEqual(convert(TaskStatus.InProgress), "In Progress");
            Assert.AreEqual(convert(TaskStatus.Finished), "Finished");
            Assert.AreEqual(convert(TaskStatus.Pending), "Pending");

            Assert.AreEqual(convert(Desk.USA), "USA");
            Assert.AreEqual(convert(Desk.General), "General");
            Assert.AreEqual(convert(Desk.Canada), "Canada");
            Assert.AreEqual(convert(Desk.UK), "UK");
        }
    }
}
