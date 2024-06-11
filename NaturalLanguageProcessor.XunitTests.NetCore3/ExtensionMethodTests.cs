using NaturalLanguageProcessor.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NaturalLanguageProcessor.XunitTests.NetCore3
{
    public class ExtensionMethodTests
    {
        [Fact]
        public void TestNumberInterpretation_WithTextNumbers_WillSucceed()
        {
            var resultsMap = new Dictionary<string, int>()
            {
                { "twenty-five", 25 },
                { "thirty-three", 33 },
                { "fifty-six", 56 },
                { "thirty", 30 },
            };

            foreach (var entry in resultsMap)
            {
                var calculatedResult = ExtensionMethods.NaturalNumberStringToInt(entry.Key);
                Assert.Equal(entry.Value, calculatedResult);
            }
        }
    }
}
