using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCss.Parsers;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class BlockParserTests
    {
        static Stylesheet Test(string input, string expected = null, int count = 1)
        {
            expected = expected ?? input;
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(p.IsValid, "invlid css");
            Assert.AreEqual(count, p.Rules.Count);
            Assert.AreEqual(expected, p.ToString());
            return p;
        }


        [Test]
        public void _1_OneClassNoContent()
        {
            var sh = ".class{}";
            Test(sh, null);
            Test(".class {}", sh);
            Test(" .class { \n  } ", sh);
            Test(" .class[cond] test { \n  } ", ".class[cond] test{}");
        }

        [Test]
        public void _2_MultipleClassesNoContent()
        {
            var sh = ".c1{}#c2{}";
            Test(sh, count: 2);
            Test(".c1{ \n }\n  #c2{ }  ", sh, count: 2);
        }

        [Test]
        public void _3_SingleWithContent()
        {
            var sh = ".cl{background-color:red;margin:5px;}";
            Test(sh);
        }

        [Test]
        public void Hacks_WithStar()
        {
            var sh = ".cl{*prop:white;}";
            var p = Test(sh);
            Assert.True(p.Rules.Single().Properties.Single().HasStar);
        }

        [Test]
        public void Hacks_With9()
        {

            var sh = "div{margin-top:1px\\9;}";
            var p = Test(sh);
            Assert.True(p.Rules.Single().Properties.Single().HasSlash9);
        }

        [Test]
        public void IE_WithDxTransform()
        {
            var sh = "div{filter:progid:DXImageTransform.Microsoft.gradient(enabled=false);}";
            Test(sh);
        }

    }
}
