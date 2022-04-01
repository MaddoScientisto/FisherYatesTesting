using CsCheck;
using FisherYatesWebApp.Services;
using FisherYatesWebApp.Services.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace FisherYatesTests
{
    [TestClass]
    public class Tests
    {

        private IFisherYatesService _service;
        private IFisherYatesService _naiveService;
        private const int TEST_ITERATIONS = 100;

        [TestInitialize]
        public async Task InitializeTest()
        {
            _service = new FisherYatesService();
            _naiveService = new NaiveFisherYatesService();
        }

        private void RunRepeatedSimpleTest(string input, int iterations, int? seed = null, string[] expectedSeedTests = null)
        {
            string previousResult = input;

            for (int i = 0; i < iterations; i++)
            {
                var arrayToTest = input.Split('-');
                _service.Shuffle(arrayToTest, seed);
                var finishedResult = string.Join("-", arrayToTest);
                Debug.WriteLine($"{i} {finishedResult}");
                Assert.AreNotEqual(input, finishedResult);

                if (expectedSeedTests != null)
                {
                    Assert.AreEqual(finishedResult, expectedSeedTests[i]);
                }

                previousResult = finishedResult;
            }
        }

        private string[] GetShuffledStrings(string input, int iterations, int? seed = null)
        {
            var results = new string[iterations];
            for (int i = 0; i < iterations; i++)
            {
                var arrayToTest = input.Split('-');
                _service.Shuffle(arrayToTest, seed);
                var finishedResult = string.Join("-", arrayToTest);
                
                results[i] = finishedResult;
            }
            return results;
        }

        private string[] GetNaiveShuffledStrings(string input, int iterations, int? seed = null)
        {
           

            var results = new string[iterations];
            for (int i = 0; i < iterations; i++)
            {
                var arrayToTest = input.Split('-');
                _naiveService.Shuffle(arrayToTest, seed);
                var finishedResult = string.Join("-", arrayToTest);

                results[i] = finishedResult;
            }
            return results;
        }

        [TestMethod]
        public void IsDifferentFromStartWithNumbers()
        {
            RunRepeatedSimpleTest("1-2-3-4-5-6-7-8-9", TEST_ITERATIONS);
        }

        [TestMethod]
        public void IsDifferentFromStartWithLetters()
        {
            RunRepeatedSimpleTest("A-B-C-D-E-F-G-H-I", TEST_ITERATIONS);
        }

        [TestMethod]
        public void FixedSeed()
        {
            var firstTenTests = new[]
            {
                "3-2-6-8-4-7-5-9-1",
                "1-7-3-5-6-9-4-2-8",
                "3-6-7-8-9-2-1-4-5",
                "8-3-1-5-9-4-2-6-7",
                "7-2-9-8-3-6-4-5-1",
                "3-9-7-6-1-8-2-4-5",
                "3-4-6-5-1-8-2-7-9",
                "9-2-1-6-8-4-7-5-3",
                "1-4-6-5-9-2-3-8-7",
                "1-4-6-5-9-2-3-8-7"
            };

            RunRepeatedSimpleTest("1-2-3-4-5-6-7-8-9", 1);
        }
        
        [TestMethod]
        public void IsOutputCorrect()
        {
            // Need to check if all the input values are found in the output
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

        


        [TestMethod]
        public void StatisticalTest()
        {
            
            StatTest("1-2-3-4", 600000, 0.2f);
            StatTest("A-B-C-D", 600000, 0.2f);


            StatTest("1-2-3-4-5-6-7-8-9", 600000, 0.04f);
        }
        // Devo trovare un numero vicino a 600000 che è divisibile per il numero di combinazioni, con un limite
        private void StatTest(string input, int iterations, float errorMargin)
        {
            int combinations = Enumerable.Range(1, input.Split('-').Length).Aggregate(1, (p, item) => p * item);

            //iterations = combinations * 25000;
            int newIterations = closestNumber(iterations, combinations);

            float expectedDistribution = (float) newIterations / (float) combinations;
            float statisticalTreshold = expectedDistribution + (expectedDistribution * errorMargin);


            // I don't really care if I lose anything in the conversion since this
            var results = GetShuffledStrings(input, newIterations);

            results.Should().NotBeEmpty().And.HaveCount(newIterations);

            var groupedResults = from r in results
                                 group r by r into g
                                 let count = g.Count()
                                 orderby count descending
                                 select new { Value = g.Key, Count = count };


            // I check if the representation is within this margin, since anything higher means that it's not as random and the algorithm was not implemented correctly
            Assert.IsTrue(groupedResults.All(x => x.Count <= statisticalTreshold), $"Greatest similar result is {groupedResults.First().Count} {groupedResults.First().Value}");
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

        private int FindClosestDivisibleNumber(float n, float m)
        {
            var q = n / m;
            var n1 = m * q;
            float n2;

            if (n*m > 0)
            {
                n2 = m * (q + 1);
            }
            else
            {
                n2 = m * (q -1);
            }

            if (Math.Abs(n-n1) < Math.Abs(n - n2))
            {
                return Convert.ToInt32(MathF.Round(n1));
            }

            return Convert.ToInt32(MathF.Round(n2));
        }

        [TestMethod]
        public void StatisticalTestWithNaiveAlgorithm()
        {
            var input = "1-2-3-4";
            const int iterations = 600000;
            const int whatIsThis = 24;

            var results = GetNaiveShuffledStrings(input, iterations);

            results.Should().NotBeEmpty().And.HaveCount(iterations);

            var groupedResults = from r in results
                                 group r by r into g
                                 let count = g.Count()
                                 orderby count descending
                                 select new { Value = g.Key, Count = count };

            var statisticalTreshold = (iterations / whatIsThis) + ((iterations / whatIsThis) * 1 / 100); // I add 1% of margin

            // Fails because it has some wildly overrepresented results
            Assert.IsFalse(groupedResults.All(x => x.Count <= statisticalTreshold), $"Greatest similar result is {groupedResults.First().Count} {groupedResults.First().Value}");

        }

        //        io valuterei:
        //- la chiarezza dei test
        //- quanto coprono
        //- (questo solo per una questione di successiva fase di interview per vedere come ragiona il candidato) quante implementazioni non random posso scrivere che non fanno fallire i test
    }
}
