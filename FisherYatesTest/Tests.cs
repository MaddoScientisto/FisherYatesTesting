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
        private const int TEST_ITERATIONS = 100;

        [TestInitialize]
        public async Task InitializeTest()
        {
            _service = new FisherYatesService();
        }

        private void RunRepeatedSimpleTest(string input, int? seed = null, string[] expectedSeedTests = null)
        {
            string previousResult = input;

            for (int i = 0; i < TEST_ITERATIONS; i++)
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

                //if (seed.HasValue && i > 0)
                //{
                //    Assert.AreEqual(previousResult, finishedResult);
                //}
                //else
                //{
                //    Assert.AreNotEqual(previousResult, finishedResult);
                //}

                previousResult = finishedResult;
            }
        }

        [TestMethod]
        public void IsDifferentFromStartWithNumbers()
        {
            RunRepeatedSimpleTest("1-2-3-4-5-6-7-8-9");
        }

        [TestMethod]
        public void IsDifferentFromStartWithLetters()
        {
            RunRepeatedSimpleTest("A-B-C-D-E-F-G-H-I");
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
    }
}
