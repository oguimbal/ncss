using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NCss.Tests
{
    [TestFixture]
    public class CloneTests
    {
        // NB: Most of the tests are performed via RealTests (cloned on each library tested)


        [Test]
        public void ClonePercentSubRules()
        {
            var input = "@-webkit-keyframes AdsAssetSelector_highlight{0%{background-color:#fff;}}";
            var sheet = new CssParser().ParseSheet(input);
            var s2 = sheet.Clone();
            Assert.AreEqual(input, sheet.ToString());
            Assert.AreEqual(input, s2.ToString());
        }
    }
}
