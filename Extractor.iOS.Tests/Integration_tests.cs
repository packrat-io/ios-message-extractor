using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Extractor.iOS.Tests
{
    public class Integration_tests
    {
        [Fact]
        public async Task Extract_single_message()
        {
            using (var extractor = new IosExtractor(Helper.GetPathToLocalFile("single-message.sqlite"), myHandle: "IT"))
            {
                var messages = await extractor.ExtractAsync();

                messages.Count.ShouldBe(1);
            }
        }
    }
}
