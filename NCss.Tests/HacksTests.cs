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
    public class HacksTests
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
        public void WithStar()
        {
            var sh = ".cl{*prop:white;}";
            var p = Test(sh);
            Assert.True(p.Rules.Single().Properties.Single().HasStar);
        }

        [Test]
        public void With9()
        {

            var sh = "div{margin-top:1px\\9;}";
            var p = Test(sh);
            Assert.True(p.Rules.Single().Properties.Single().HasSlash9);
        }

        [Test]
        public void With0()
        {
            var sh = "#div{height:300px\\0/;}";
            var p = Test(sh);
            Assert.True(p.Rules.Single().Properties.Single().HasSlash0);
        }

        [Test]
        public void IE_WithDxTransform()
        {
            var sh = "div{filter:progid:DXImageTransform.Microsoft.gradient(enabled=false);}";
            Test(sh);
        }

        [Test]
        public void IE6Only()
        {
            var sh = "* html #div{height:300px;}";
            Test(sh);
        }

        [Test]
        public void IE7Only()
        {
            var sh = "*+html #div{height:300px;}";
            Test(sh);
        }

        [Test]
        public void IE6Prop()
        {
            var sh = ".class{_prop:val;}";
            Test(sh);
        }

        [Test]
        public void BangHack()
        {
            var sh = ".class{_prop:val!ie7;}";
            Test(sh);
        }

    }
}
