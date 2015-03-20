using NCss.Parsers;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class SelectorParserTests
    {
        [Test]
        public void Simple()
        {
            var parser = new SimpleSelectorParser();
            parser.SetContext(".my-class");
            var cl = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(cl != null && cl.IsValid, "invalid selector");
            Assert.AreEqual("my-class", cl.Name);
            Assert.AreEqual(SimpleSelector.Type.Class, cl.SelectorType);
        }

        [Test]
        public void Not()
        {
            var parser = new SimpleSelectorParser();
            parser.SetContext(":not(.notclass)");
            var not = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(not != null && not.IsValid, "invalid selector");
            Assert.AreEqual(":not(.notclass)", not.ToString());
            Assert.IsTrue(not.HasArgument);
            Assert.IsInstanceOf<SimpleSelector>(not.SelectorArgument);
            Assert.IsNull(not.Argument);
        }

        [Test]
        public void NotFollowedBySpace()
        {
            var parser = new SelectorParser();
            parser.SetContext(":not(.notclass) .after");
            var not = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(not.IsValid, "invalid selector");
            Assert.AreEqual(":not(.notclass) .after", not.ToString());
        }


        [Test]
        public void Lang()
        {
            var parser = new SimpleSelectorParser();
            parser.SetContext(":lang(en)");
            var not = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(not != null && not.IsValid, "invalid selector");
            Assert.AreEqual(":lang(en)", not.ToString());
            Assert.IsTrue(not.HasArgument);
            Assert.AreEqual("en", not.Argument.ToString());
            Assert.IsNull(not.SelectorArgument);
        }

        [Test]
        public void NthChild()
        {
            var parser = new SimpleSelectorParser();
            parser.SetContext(":nth-of-type(1n)");
            var not = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(not != null && not.IsValid, "invalid selector");

            parser.SetContext(":nth-of-type(3n+0)");
            not = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(not != null && not.IsValid, "invalid selector");
        }

        [Test]
        public void ElementType()
        {
            var parser = new SimpleSelectorParser();
            parser.SetContext("div");
            var not = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(not != null && not.IsValid, "invalid selector");
            Assert.AreEqual("div", not.ToString());
            Assert.AreEqual(SimpleSelector.Type.ElementType, not.SelectorType);
        }

        

        [Test]
        public void Complex()
        {
            var parser = new SelectorParser();
            parser.SetContext(".parent>.child#withcondition[and='attr'] ~ .subchild:not(.notclass#notdiv xx )::test .subsub:lang(en):nth-of-type(3)");
            var sel = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(sel != null && sel.IsValid, "invalid selector");
            Assert.NotNull(sel);
            Assert.AreEqual(".parent>.child#withcondition[and='attr']~.subchild:not(.notclass#notdiv xx)::test .subsub:lang(en):nth-of-type(3)", sel.ToString());
        }

        //[Test]
        //public void InvalidSelector()
        //{
        //    var parser = new SelectorParser();
        //    parser.SetContext("0x");
        //    var sel = parser.DoParse();
        //    Assert.IsTrue(parser.End);
        //    Assert.AreEqual(1, parser.Errors.Count);
        //    Assert.IsFalse(sel.IsValid);
        //    Assert.IsInstanceOf<InvalidSelector>(sel);
        //    Assert.AreEqual("", sel.ToString());
        //    Assert.AreEqual("0x", sel.ToString(CssRestitution.OriginalWhenErrorOrInvalid));

        //    parser.SetContext("%x;");
        //    sel = parser.DoParse();
        //    Assert.AreEqual(1, parser.Errors.Count);
        //    Assert.IsFalse(sel.IsValid);
        //    Assert.IsInstanceOf<InvalidSelector>(sel);
        //    Assert.AreEqual("", sel.ToString());
        //    Assert.AreEqual("%x", sel.ToString(CssRestitution.OriginalWhenErrorOrInvalid));
        //    Assert.AreEqual(';', parser.CurrentChar);
            
        //    parser.SetContext("1x.c");
        //    sel = parser.DoParse();
        //    Assert.IsTrue(parser.End);
        //    Assert.AreEqual(1, parser.Errors.Count);
        //    Assert.IsFalse(sel.IsValid);
        //    Assert.IsInstanceOf<MultiConditionSelector>(sel);
        //    var lst = ((MultiConditionSelector) sel).Conditions;
        //    Assert.AreEqual(2, lst.Count);
        //    Assert.IsInstanceOf<InvalidSelector>(lst[0]);
        //    Assert.IsInstanceOf<SimpleSelector>(lst[1]);
        //    Assert.AreEqual("1x", lst[0].ToString(CssRestitution.OriginalWhenErrorOrInvalid));
        //    Assert.AreEqual(".c", lst[1].ToString());


        //    parser.SetContext("1x(.c)");
        //    sel = parser.DoParse();
        //    Assert.IsTrue(parser.End);
        //    Assert.AreEqual(1, parser.Errors.Count);
        //    Assert.IsFalse(sel.IsValid);
        //    Assert.IsInstanceOf<InvalidSelector>(sel);
        //    Assert.AreEqual("1x(.c)", sel.ToString(CssRestitution.OriginalWhenErrorOrInvalid));

        //    parser.SetContext("1x(.c");
        //    sel = parser.DoParse();
        //    Assert.IsTrue(parser.End);
        //    Assert.AreEqual(1, parser.Errors.Count);
        //    Assert.IsFalse(sel.IsValid);
        //    Assert.IsInstanceOf<InvalidSelector>(sel);
        //    Assert.AreEqual("1x(.c", sel.ToString(CssRestitution.OriginalWhenErrorOrInvalid));

            
        //    parser.SetContext("1x(.c).x");
        //    sel = parser.DoParse();
        //    Assert.IsTrue(parser.End);
        //    Assert.AreEqual(1, parser.Errors.Count);
        //    Assert.IsFalse(sel.IsValid);
        //    Assert.IsInstanceOf<MultiConditionSelector>(sel);
        //    lst = ((MultiConditionSelector)sel).Conditions;
        //    Assert.AreEqual(2, lst.Count);
        //    Assert.IsInstanceOf<InvalidSelector>(lst[0]);
        //    Assert.IsInstanceOf<SimpleSelector>(lst[1]);
        //    Assert.AreEqual("1x(.c)", lst[0].ToString(CssRestitution.OriginalWhenErrorOrInvalid));
        //    Assert.AreEqual(".x", lst[1].ToString());

        //}

        [Test]
        public void AllSelector()
        {
            var parser = new SelectorParser();
            parser.SetContext("*");
            var ch = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(ch != null && ch.IsValid, "invalid selector");
            Assert.IsInstanceOf<SimpleSelector>(ch);
            Assert.AreEqual(SimpleSelector.Type.All, ((SimpleSelector) ch).SelectorType);
        }

        [Test]
        public void ChildSelectorTest()
        {
            var parser = new ChildSelectorParser();
            parser.SetContext(">.my-class");
            var ch = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(ch != null && ch.IsValid, "invalid selector");
            Assert.AreEqual(ChildSelector.ChildType.DirectChild, ch.Type);
            Assert.IsInstanceOf<SimpleSelector>(ch.Child);
            var cl = (SimpleSelector) ch.Child;
            Assert.AreEqual("my-class", cl.Name);
            Assert.AreEqual(SimpleSelector.Type.Class, cl.SelectorType);

            parser.SetContext("~.myclass");
            ch = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(ch != null && ch.IsValid, "invalid selector");
            Assert.AreEqual(ChildSelector.ChildType.PredecesorOf, ch.Type);

            parser.SetContext("+.myclass");
            ch = parser.DoParse();
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(ch != null && ch.IsValid, "invalid selector");
            Assert.AreEqual(ChildSelector.ChildType.FirstPredecesorOf, ch.Type);
        }

        [Test]
        public void SelectorList()
        {
            var parser = new SelectorParser();
            parser.SetContext(".cl1,.cl2");
            var p = parser.DoParse();
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(parser.End);
            Assert.IsInstanceOf<SelectorList>(p);
            var pl = (SelectorList) p;
            Assert.AreEqual(2, pl.Selectors.Count);
            Assert.AreEqual(".cl1,.cl2", pl.ToString());
            var c1 = pl.Selectors[0];
            var c2 = pl.Selectors[1];
            Assert.IsInstanceOf<SimpleSelector>(c1);
            Assert.IsInstanceOf<SimpleSelector>(c2);
        }

        [Test]
        public void Condition()
        {
            var parser = new AttributeConditionParser();

            parser.SetContext("[attr]");
            var p = parser.DoParse();
            Assert.IsTrue(p != null && p.IsValid, "invalid selector");
            Assert.AreEqual(AttributeCondition.Type.HavingAttribute, p.ConditionType);
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);

            parser.SetContext("[attr='test']");
            p = parser.DoParse();
            Assert.IsTrue(p != null && p.IsValid, "invalid selector");
            Assert.AreEqual(AttributeCondition.Type.Equals, p.ConditionType);
            Assert.AreEqual("'test'", p.Value);
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);

            parser.SetContext("[attr=test]");
            p = parser.DoParse();
            Assert.IsTrue(p != null && p.IsValid, "invalid selector");
            Assert.AreEqual(AttributeCondition.Type.Equals, p.ConditionType);
            Assert.AreEqual("test", p.Value);
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);

            parser.SetContext("[attr|=test]");
            p = parser.DoParse();
            Assert.IsTrue(p != null && p.IsValid, "invalid selector");
            Assert.AreEqual(AttributeCondition.Type.StartsWithDashSeparated, p.ConditionType);
            Assert.AreEqual("test", p.Value);
            Assert.IsTrue(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
        }



        [Test]
        public void DirectiveSelector()
        {
            var parser = new DirectiveSelectorParser();
            parser.SetContext("@xxx lksqdf/* ; { */ ss{");
            var cl = parser.DoParse();
            Assert.AreEqual('{',parser.CurrentChar);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.IsTrue(cl != null && cl.IsValid, "invalid selector");
            Assert.AreEqual("xxx", cl.Name);
            Assert.AreEqual("lksqdf ss", cl.Arguments);
        }
    }
}
