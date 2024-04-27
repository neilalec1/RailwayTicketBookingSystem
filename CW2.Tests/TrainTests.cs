using CW2.BusinessLogicLayer;
using NUnit.Framework;

namespace CW2.BusinessLogicLayer
{
    [TestFixture]
    public class TrainTests
    {
        [Test]
        public void ToString_ReturnsCorrectStringRepresentation()
        {
            
            Train train = new Train
            {
                ID = "T123",
                DepartureStation = "London",
                TerminalStation = "Edinburgh",
                DateOfService = new DateTime(2023, 07, 31),
                DepartureTime = new TimeSpan(10, 30, 0)
            };

            
            string result = train.ToString();

            //using the constraint model
            string expected = "Train T123: London to Edinburgh on 31/07/2023 at 10:30";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}