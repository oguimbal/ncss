﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCss.Parsers;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class RestitutionTests
    {
        [Test]
        public void UnparsableProperties()
        {
            var p = new CssParser().ParseSheet(".class{ldsk}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".class{ldsk}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.RemoveInvalid));
            p = new CssParser().ParseSheet(".class{ldsk%xk}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".class{ldsk%xk}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
        }

        [Test]
        public void InvalidPropertyValue()
        {
            var p = new CssParser().ParseSheet(".class{prop:#ffff;}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".class{prop:#ffff;}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".class{prop:#ffff;}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".class{prop:#ffff;}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.RemoveInvalid));

            p = new CssParser().ParseSheet(".class{prop:#ffff;msldkqj;}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".class{prop:#ffff;msldkqj;}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".class{prop:#ffff;}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".class{prop:#ffff;}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.RemoveInvalid));

            p = new CssParser().ParseSheet(".class{prop:#ffff;msldkqj;prop:;}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".class{prop:#ffff;msldkqj;prop:;}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".class{prop:#ffff;prop:;}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".class{prop:#ffff;prop:;}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".class{}", p.ToString(CssRestitution.RemoveInvalid));
        }

        [Test]
        public void FuckedUpBlocks()
        {
            var p = new CssParser().ParseSheet(".c1{prop:#ffff}}.c2{prop:red}}.c3{color:red}{}");
            Assert.That(p.Rules.All(x => x != null));

            Assert.AreEqual(".c1{prop:#ffff}}.c2{prop:red;}}.c3{color:red;}{}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".c1{prop:#ffff;}.c2{prop:red;}.c3{color:red;}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".c1{prop:#ffff;}.c2{prop:red;}.c3{color:red;}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".c1{}.c2{prop:red;}.c3{color:red;}", p.ToString(CssRestitution.RemoveInvalid));
        }

        [Test]
        public void FuckedUpSelectors()
        {
            var p = new CssParser().ParseSheet(".c1/c2, c3#c4, div[=c5]{}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".c1/c2,c3#c4,div[=c5]{}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".c1,c3#c4,div{}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".c1,c3#c4,div{}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".c1,c3#c4,div{}", p.ToString(CssRestitution.RemoveInvalid));

        }



        [Test]
        public void UsedToThrow()
        {

            var p = new CssParser().ParseSheet(".test{0%{test:red}}");
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual(".test{0%{test:red}}", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".test{}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".test{}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".test{}", p.ToString(CssRestitution.RemoveInvalid));
        }


        [Test]
        public void HtmlCommentOutOfNowhere()
        {
            var parser = new CssParser();
            var p = parser.ParseSheet("<!-- .class{test:x;} -->");
            Assert.AreEqual(4, parser.Errors.Count);
            Assert.That(p.Rules.All(x => x != null));
            Assert.AreEqual("<!-- .class{test:x;}-->", p.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
            Assert.AreEqual(".class{test:x;}", p.ToString(CssRestitution.OnlyWhatYouUnderstood));
            Assert.AreEqual(".class{test:x;}", p.ToString(CssRestitution.RemoveErrors));
            Assert.AreEqual(".class{test:x;}", p.ToString(CssRestitution.RemoveInvalid));
        }
    }
}
