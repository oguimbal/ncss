using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NCss.Parsers;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class RealTests
    {
        string GetTestFile(string file)
        {

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "NCss.Tests.RealTestsCss." + file + ".css";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        [Test]
        public void TwoBracesAtOnce()
        {
            var input = "@media print{.table-bordered th{border:1px solid #ddd !important;}}@font-face{font-family:'Glyphicons Halflings';}";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
            Assert.AreEqual(input, p.ToString());
        }

        [Test]
        public void AllSelectorBlock()
        {
            var input = "*{color:blue;}";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
            Assert.AreEqual(input, p.ToString());
        }
        [Test]
        public void OrphanBlock()
        {
            var input = "{color:blue;}.class{color:red;}";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.False(p.IsValid, "must be invalid css");
            Assert.AreEqual(input, p.ToString());
            Assert.AreEqual(2, p.Rules.Count);
            Assert.IsInstanceOf<OrphanBlockRule>(p.Rules[0]);
            Assert.IsInstanceOf<ClassRule>(p.Rules[1]);
            Assert.True(p.Rules[1].IsValid);
            Assert.AreEqual(".class{color:red;}",p.Rules[1].ToString());
        }

        [Test]
        public void StringsInProperty()
        {
            // from materialize.css
            var input = "a[href]:after {content: \" (\" attr(href) \")\"}";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
        }

        [Test]
        public void LinearGradient()
        {
            // from facebook
            var input = ".x{background-image:-webkit-linear-gradient(135deg, red 0%, orange 15%, yellow 30%, green 45%, blue 60%,indigo 75%, violet 80%, red 100%);}";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
            Assert.AreEqual(input.Replace(", ",","), p.ToString());
        }

        [Test]
        public void Rem()
        {
            
            // from materialize.css
            var input = "textarea{width:calc(100% - 3rem);}";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
            Assert.AreEqual(input.Replace(" - ","-"), p.ToString());
        }

        [Test]
        public void GumbyComment()
        {
            var input = "@charset \"UTF-8\";\n/*  */\n@import url(url);";
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.True(parser.End);
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
        }
        
        [Test]
        public void LokadBootstrap()
        {
            TestFile("Lokad_Bootstrap");
        }

        [Test]
        public void Materialize()
        {
            TestFile("materialize");
        }

        [Test]
        public void Facebook()
        {
            TestFile("facebook");
        }

        [Test]
        public void Stackoverflow()
        {
            TestFile("stackoverflow");
        }

        [Test]
        public void Gumby()
        {
           TestFile("gumby_min");
           TestFile("gumby",false);
        }

        [Test]
        public void Normalize()
        {
            TestFile("normalize", false);
        }

        [Test]
        public void Foundation()
        {
           TestFile("foundation.min");
           TestFile("foundation",false);
        }

        Stylesheet TestFile(string file, bool testSimilarities=true)
        {
            var input = GetTestFile(file);
            var parser = new StylesheetParser();
            parser.SetContext(input);
            var p = parser.DoParse();
            Assert.IsTrue(parser.End);
            var ir = p.Rules.Where(r => !r.IsValid).ToArray();
            Assert.AreEqual(0, parser.Errors.Count);
            Assert.True(p.IsValid, "invalid css");
            var output = p.ToString();

            if (testSimilarities)
            {
                Assert.AreEqual(input.Count(c => c == '{'), output.Count(c => c == '{'));
                Assert.AreEqual(input.Count(c => c == '}'), output.Count(c => c == '}'));
                foreach (var c in new[] { "{", ";\r" })
                {
                    string toReparse;
                    if (c == "{")
                    {
                        var reg = new Regex(@"(?<=\}[^\{\}\'\" + "\"" + @"+]+)\{", RegexOptions.Multiline | RegexOptions.Compiled);
                        toReparse = reg.Replace(output, "/* some comment */{");
                    }
                    else
                    {
                        var reg = new Regex(@"(?<!url\(data:[a-zA-Z\+/]+)\;", RegexOptions.Multiline | RegexOptions.Compiled);
                        toReparse = reg.Replace(output, "/* some comment */;");
                    }
                    parser.SetContext(toReparse);
                    var p2 = parser.DoParse();
                    Assert.IsTrue(parser.End);
                    Assert.AreEqual(0, parser.Errors.Count);
                    Assert.True(p2.IsValid, "invalid css on reparse");
                    var output2 = p2.ToString();
                    Assert.AreEqual(output, output2);
                }
            }

            return p;
        }
    }
}
