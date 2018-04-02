using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using BizComponent;

namespace UnitTest
{
    [TestClass]
    public class SEOComponentTest
    {
        [TestMethod]
        public void AnalyzeStringTest()
        {
            SimpleSEOComponent sc = new SimpleSEOComponent();

            string tc = "this test is unit test";

            List<WordOccurence> expectedResult = new List<WordOccurence>();
            expectedResult.Add(new WordOccurence() { Word = "this", NoOfOccurences = 1 });
            expectedResult.Add(new WordOccurence() { Word = "test", NoOfOccurences = 2 });
            expectedResult.Add(new WordOccurence() { Word = "is", NoOfOccurences = 1 });
            expectedResult.Add(new WordOccurence() { Word = "unit", NoOfOccurences = 1 });

            expectedResult = expectedResult.OrderBy(o => o.Word).ToList();

            var result = sc.AnalyzeString(tc, false);

            /*
             * List of Test
             */
            //Assert.AreEqual(result.Count, expectedResult.Count);
            //Assert.AreEqual(result[1].Word, expectedResult[1].Word);
            Assert.AreEqual(result[1].NoOfOccurences, expectedResult[1].NoOfOccurences);
        }

        [TestMethod]
        public void ExcludeStringTest()
        {
            SimpleSEOComponent sc = new SimpleSEOComponent();

            string tc = "this test" + Environment.NewLine + "is, unit. test";

            string expectedResult = "this test  is unit test";
            var result = sc.ExcludeString(tc);
            
            Assert.AreEqual(result, expectedResult);
        }
    }
}
