namespace awes0mecoderz.Retarder.Tests
{
    using System;
    using Xunit;

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

        // Nothing has been done yet

        #endregion
    }
}
