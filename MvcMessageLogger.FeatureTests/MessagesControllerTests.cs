using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MvcMessageLogger.FeatureTests
{
    [Collection("Controller Tests")]
    public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MessagesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private MvcMessageLoggerContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MvcMessageLoggerContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new MvcMessageLoggerContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public async Task Create_AddsMessages_RedirectsToUserShowPage()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            User user1 = new User { Name = "Skylar", Username = "ssandler" };
            context.Users.Add(user1);
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                { "Content", "Hello World!" }

            };

            var response = await client.PostAsync($"/users/{user1.Id}", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"/users/{user1.Id}", response.RequestMessage.RequestUri.ToString());
            Assert.Contains("Hello World!", html);
            Assert.DoesNotContain("Happy Birthday!", html);
        }
    }
}
