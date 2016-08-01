using System;
using Xunit;
namespace awes0mecoderz.Retarder.Tests
{
    public class RetarderTests
    {
        #region Ctor

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        public void Ctor_PassInvalidTimePeriod_ThrowException(int timePeriod) 
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Retarder(timePeriod, 1));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        public void Ctor_PassInvalidMaxExecutions_ThrowException(int maxExecutions) 
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Retarder(1, maxExecutions));
        }

        #endregion

        #region Methods

        // [Fact]
        // public void HangOn_PassZeroTimePeriod_ThrowException() 
        // {
        //     Assert.Throws<ArgumentOutOfRangeException>(() => new Retarder(0, 1));
        // }

        #endregion
    }
}
