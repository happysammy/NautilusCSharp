
namespace QuickFix.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class DefaultMessageFactoryTests
    {
        [Test]
        public void GroupCreateTest()
        {
            DefaultMessageFactory dmf = new DefaultMessageFactory();

            Group g44 = dmf.Create("FIX.4.4", "B", 33);
            Assert.IsInstanceOf<QuickFix.FIX44.News.LinesOfTextGroup>(g44);
        }
    }
}
