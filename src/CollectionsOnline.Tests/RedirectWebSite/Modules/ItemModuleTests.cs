using CollectionsOnline.RedirectWebSite.Modules;
using Nancy;
using Nancy.Testing;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.RedirectWebSite.Modules
{
    public class RedirectModuleTests
    {
        [Fact]
        public void GetIndex_ReturnsMovedPermanently()
        {
            // Given When
            var result = new Browser(with => with.Module<RedirectModule>()).Get("/", with => with.HttpRequest());

            // Then
            result.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
        }
    }
}