using URLShortening.Application;
using URLShortening.Domain.Constants;

namespace URLShortening.Tests
{
    public class UtilitiesCodeTests
    {
        [Fact]
        public void GenerateCode_LengthShouldBeShortLinkSettingsLength()
        {
            var code = Utilities.GenerateCode();
            Assert.Equal(ShortLinkSettings.Length, code.Length);
        }

        [Theory]
        [InlineData("https://WtqWuOa", "WtqWuOa")]
        [InlineData("https://UmqWuOa", "UmqWuOa")]
        [InlineData("https://WtqWub4", "WtqWub4")]
        public void GetCode_ReturnsURLCode(string url, string expectedCode)
        {
            var code = Utilities.GetCode(url);
            Assert.Equal(expectedCode, code);
        }
    }
}

