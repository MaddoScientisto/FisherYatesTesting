using FisherYatesWebApp.Services;
using FisherYatesWebApp.Services.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherYatesTests
{
    [TestClass]
    public class Tests
    {

        private IFisherYatesService _service;

        [TestInitialize]
        public async Task InitializeTest()
        {
            _service = new DummyFisherYatesService();
        }


        private string[] GetShuffledStrings(string input, int iterations, int? seed = null)
        {
            var results = new string[iterations];
            for (int i = 0; i < iterations; i++)
            {
                var result = _service.Shuffle(input, seed);

                results[i] = result;
            }
            return results;
        }




        /// <summary>
        /// Checks if all the input values are found in the output
        /// </summary>
        [TestMethod]
        public void IsOutputCorrect()
        {
            string[] stringsToTest = new[]
            {
                "A-B-C-D",
                "1-2-3-4",
                "Z-F-D-H",
                "5-3-4-1",
                "A-B-C-D-E-F-G-H-I",
                "1-2-3-4-5-6-7-8-9"
            };

            TestVariousStrings(stringsToTest);

        }

        private void TestVariousStrings(string[] stringsToTest)
        {
            foreach (var stringToTest in stringsToTest)
            {
                var shuffledStrings = GetShuffledStrings(stringToTest, 1000);

                foreach (var shuffledString in shuffledStrings)
                {
                    Assert.IsTrue(shuffledString.Length == stringToTest.Length);
                    Assert.IsTrue(DoStringsHaveSameElements(stringToTest, shuffledString));
                }
            }
        }


        private bool DoStringsHaveSameElements(string inputA, string inputB)
        {
            var firstArray = inputA.Split('-');
            var secondArray = inputB.Split('-');

            return firstArray.OrderBy(x => x).SequenceEqual(secondArray.OrderBy(x => x));

        }



        /// <summary>
        /// This tests the randomness of the Fisher-Yates algorithm, it's calibrated for a properly implemented algorithm so a naively implemented algorithm should fail
        /// </summary>
        [TestMethod]
        public void StatisticalTest()
        {

            StatTest("2-3-4", 600000, 0.1f, 0.1f);
            StatTest("B-C-D", 600000, 0.1f, 0.1f);

            // I'm limiting the tests to only 4 elements because as more elements are added the combinations increase enormously and would take way too long to do the same test
            StatTest("1-2-3-4", 600000, 0.1f, 0.2f);
            StatTest("A-B-C-D", 600000, 0.1f, 0.2f);
        }


        /// <summary>
        /// This function does a statistical test to make sure that the results are inside expected range. A properly implemented algorithm will have roughly similar ranges while a naive algorithm will have wildly differing ranges.
        /// This should support an arbitrarily large number of elements but the number of iterations will grow enormously and may prove to be a very long process.
        /// </summary>
        /// <param name="input">The input string in the format '1-2-3-4'</param>
        /// <param name="iterationsTarget">The target number of iterations to test the algorithm with, higher combinations amounts would require more iterations to properly determine the statistical validity of the algorithm</param>
        /// <param name="lowErrorMargin">Percentage margin of the lowest aggregation</param>
        /// <param name="highErrorMargin">Percentage margin of the highest aggregation</param>
        private void StatTest(string input, int iterationsTarget, float lowErrorMargin, float highErrorMargin)
        {

            int combinations = Enumerable.Range(1, input.Split('-').Length).Aggregate(1, (p, item) => p * item);

            // I find the closest divisible number for the combinations closest to the iterations target
            int newIterations = closestNumber(iterationsTarget, combinations);

            float expectedDistribution = (float)newIterations / (float)combinations;
            // I calculate the min and max tresholds for randomness, a properly implemented algorithm should always return results within this range
            float highStatisticalTreshold = expectedDistribution + (expectedDistribution * highErrorMargin);
            float lowStatisticalTreshold = expectedDistribution - (expectedDistribution * lowErrorMargin);


            var results = GetShuffledStrings(input, newIterations);

            Assert.IsTrue(results.Any() && results.Length >= newIterations);

            // I aggregate the results so that I can see how many times a particular result appears
            var groupedResults = from r in results
                                 group r by r into g
                                 let count = g.Count()
                                 orderby count descending
                                 select new { Value = g.Key, Count = count };


            // I check if the representation is within this margin, since anything higher means that it's not as random and the algorithm was not implemented correctly
            Assert.IsTrue(groupedResults.All(x => x.Count <= highStatisticalTreshold && x.Count >= lowStatisticalTreshold), $"Greatest similar result is {groupedResults.First().Count} {groupedResults.First().Value}, low treshold: {lowStatisticalTreshold} high treshold: {highStatisticalTreshold}");
        }

        static int closestNumber(int n, int m)
        {
            // find the quotient
            int q = n / m;

            // 1st possible closest number
            int n1 = m * q;

            // 2nd possible closest number
            int n2 = (n * m) > 0 ? (m * (q + 1)) : (m * (q - 1));

            // if true, then n1 is the required closest number
            if (Math.Abs(n - n1) < Math.Abs(n - n2))
                return n1;

            // else n2 is the required closest number
            return n2;
        }





    }
}
