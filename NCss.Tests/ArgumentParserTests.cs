using NCss.Parsers;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class ArgumentParserTests
    {

        [Test]
        public void PropertyValueWithFunction()
        {
            var parser = new CssSimpleValueParser();
            parser.SetContext("fn(1,2)");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.AreEqual("fn(1,2)", arg.ToString());
        }

        [Test]
        public void Simple()
        {
            var parser = new CssValueParser();
            parser.SetContext("#fff");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.IsInstanceOf<CssSimpleValue>(arg);
            Assert.AreEqual("#fff", arg.ToString());
        }

        [Test]
        public void SubArg()
        {
            var parser = new CssValueParser();
            parser.SetContext("fn(#fff)");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.IsInstanceOf<CssSimpleValue>(arg);
            Assert.AreEqual("fn(#fff)", arg.ToString());
        }

        [Test]
        public void Addition()
        {
            var parser = new CssValueParser();
            parser.SetContext("30%+10%");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.IsInstanceOf<CssArithmeticOperation>(arg);
            Assert.AreEqual("30%+10%", arg.ToString());

            parser.SetContext("(30%+10%)");
            arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.IsInstanceOf<CssArithmeticOperation>(arg);
            Assert.AreEqual("(30%+10%)", arg.ToString());
        }

        [Test]
        public void WithParenthesis()
        {
            var parser = new CssValueParser();
            parser.SetContext("(30%+10%)*3");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.IsInstanceOf<CssArithmeticOperation>(arg);
            Assert.AreEqual("(30%+10%)*3", arg.ToString());
        }

        [Test]
        public void Priority()
        {
            var parser = new CssValueParser();
            parser.SetContext("30%/fn(2,3)+10%*1.4+2*3");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.IsInstanceOf<CssArithmeticOperation>(arg);
            Assert.AreEqual("30%/fn(2,3)+10%*1.4+2*3", arg.ToString());

            Assert.AreEqual("30%/fn(2,3)", ((CssArithmeticOperation) arg).Left.ToString());
            Assert.AreEqual("10%*1.4+2*3", ((CssArithmeticOperation)arg).Right.ToString());

            var o = ((CssArithmeticOperation) arg).Right;
            Assert.IsInstanceOf<CssArithmeticOperation>(o);
            Assert.AreEqual("10%*1.4", ((CssArithmeticOperation)o).Left.ToString());
            Assert.AreEqual("2*3", ((CssArithmeticOperation)o).Right.ToString());
        }

        [Test]
        public void WithSpaces()
        {
            var parser = new CssSimpleValueParser();
            parser.SetContext("x(abc, de fg )");
            var arg = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(arg.IsValid);
            Assert.AreEqual(3, arg.Arguments.Count);
            Assert.AreEqual("x(abc,de fg)", arg.ToString());
        }
    }
}
