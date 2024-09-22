using FlyFramework.Common.Utilities.RabbitMqs;

namespace FlyFramework.Test
{
    public class RabbitMqTest
    {
        private readonly IRabbitMqManager _rabbitMqManager;

        public RabbitMqTest()
        {

        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var me = _rabbitMqManager.GetChannel();
            Assert.Pass();
        }
    }
}