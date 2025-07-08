using System;
using System.Net;
using System.Text;

namespace ChannelService.FunctionalTests
{
    public class ServerApiTests
    {
        //private HttpClient CreateHttpClient(ApiVersion apiVersion)
        //{
        //    var handler = new ApiVersionHandler(new QueryStringApiVersionWriter(), apiVersion);
        //    return _webApplicationFactory.CreateDefaultClient(handler);
        //}

        //[Fact]
        //public void Post_CreatesOrder_ReturnsCreated()
        //{
        //    var _httpClient = CreateHttpClient(new ApiVersion(version));

        //    // Arrange
        //    var orderRequest = new
        //    {
        //        CustomerId = Guid.NewGuid(),
        //        Items = new[]
        //        {
        //            new { ProductId = Guid.NewGuid(), Quantity = 2, Price = 100m }
        //        }
        //    };

        //    var content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");

        //    // Act
        //    var response = await _client.PostAsync("/api/orders", content);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}
    }
}
