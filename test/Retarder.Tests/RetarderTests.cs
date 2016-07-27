using System;
using Xunit;
using awes0mecoderz.Retarder;

namespace awes0mecoderz.Retarder.Tests
{
    public class RetarderTests
    {
        [Fact]
        public void Test1() 
        {
            var retarder = new Retarder(100, 10);
            Assert.True(retarder.IsTrue());

        }
    }
}
