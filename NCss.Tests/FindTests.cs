﻿using System;
using System.Linq;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class FindTests
    {
        [Test]
        public void ReplaceHover()
        {
            var sheet = new CssParser().ParseSheet(".cl:hover,.cl2:hover{}.cl3:hover{}");
            var found = sheet.Find<Selector>(x => x ==":hover").ToArray();
            Assert.True(sheet.IsValid);
            Assert.AreEqual(3, found.Length);
            foreach (var f in found)
                f.ReplaceBy(new SimpleSelector(".hov"));
            Assert.AreEqual(".cl.hov,.cl2.hov{}.cl3.hov{}", sheet.ToString());
        }

        [Test]
        public void RemoveHover()
        {
            var sheet = new CssParser().ParseSheet(".cl:hover,.cl2:hover{}.cl3:hover{}");
            foreach (var f in sheet.Find<SimpleSelector>(x => x.FullName == ":hover").ToArray())
                f.Remove();
            Assert.True(sheet.IsValid);
            Assert.AreEqual(".cl,.cl2{}.cl3{}", sheet.ToString());
        }

        [Test]
        public void RemoveOneClass()
        {
            var sheet = new CssParser().ParseSheet(".cl1,.cl2{}");
            foreach (var f in sheet.Find<SimpleSelector>(x => x.FullName == ".cl2").ToArray())
                f.Remove();
            Assert.True(sheet.IsValid);
            Assert.AreEqual(".cl1{}", sheet.ToString());
        }

        [Test]
        public void FindUrl()
        {
            var sheet = new CssParser().ParseSheet(".cl{background-image:url(test.png)}");
            foreach (var f in sheet.Find<CssSimpleValue>(x => x.IsFunction && x.Name == "url").ToArray())
                f.ReplaceBy(new CssSimpleValue("url", "other.png"));
            Assert.True(sheet.IsValid);
            Assert.AreEqual(".cl{background-image:url(other.png);}", sheet.ToString());
        }

        [Test]
        public void FindUrlAmongOthers()
        {
            var sheet = new CssParser().ParseSheet(".cl{background: transparent url(test.png)}");
            foreach (var f in sheet.Find<CssSimpleValue>(x => x.IsFunction && x.Name == "url").ToArray())
                f.ReplaceBy(new CssSimpleValue("url", "other.png"));
            Assert.True(sheet.IsValid);
            Assert.AreEqual(".cl{background:transparent url(other.png);}", sheet.ToString());
        }


        [Test]
        public void RemoveClass()
        {
            var sheet = new CssParser().ParseSheet(".cl1{}.cl2#id{}");
            foreach (var f in sheet.Find<ClassRule>(x => x.Selector == ".cl2#id").ToArray())
                f.Remove();
            Assert.True(sheet.IsValid);
            Assert.AreEqual(".cl1{}", sheet.ToString());
        }

        [Test]
        public void RemoveProperty()
        {
            var sheet = new CssParser().ParseSheet(".cl1{prop:red;other:null;}.cl2#id{prop:test;}");
            foreach (var f in sheet.Find<Property>(x => x.Name == "prop").ToArray())
                f.Remove();
            Assert.True(sheet.IsValid);
            Assert.AreEqual(".cl1{other:null;}.cl2#id{}", sheet.ToString());
        }

        [Test]
        public void RemoveInMedia()
        {
            var sheet = new CssParser().ParseSheet("@media test{.cl:hover{bg:red;}}");
            foreach (var f in sheet.Find<Selector>(x => x == ":hover").ToArray())
                f.Remove();
            Assert.True(sheet.IsValid);
            Assert.AreEqual("@media test{.cl{bg:red;}}", sheet.ToString());
        }
    }
}
