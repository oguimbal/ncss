using NCss.Parsers;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class PropertyParserTests
    {
        [Test]
        public void Simple()
        {
            var parser = new PropertyParser();
            parser.SetContext(" background-color: red ");
            var prop  = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(prop.IsValid);
            Assert.AreEqual("background-color", prop.Name);
            Assert.AreEqual("background-color:red;", prop.ToString());
        }

        [Test]
        public void DataUrl()
        {
            // contains ';', that's the heck !
            var parser = new PropertyParser();
            parser.SetContext("background-image:url(data:image/png;base64,XXXX)");
            var prop = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(prop.IsValid);
            Assert.AreEqual("background-image:url(data:image/png;base64,XXXX);", prop.ToString());
        }

        [Test]
        public void NestedFunctions()
        {
            var parser = new PropertyParser();
            parser.SetContext("x:f1(f2(1),f3(2px))");
            var prop = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(prop.IsValid);
            Assert.AreEqual("x:f1(f2(1),f3(2px));", prop.ToString());
        }

        [Test]
        public void FontZero()
        {
            var parser = new PropertyParser();
            parser.SetContext("font:0/0 a");
            var prop = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(prop.IsValid);
            Assert.AreEqual("font:0/0 a;", prop.ToString());
        }
    }
}
