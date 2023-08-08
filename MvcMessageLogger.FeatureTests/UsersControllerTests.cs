using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;
using MvcMessageLogger.FeatureTests;
using System.Net;

namespace MvcMessageLogger.FeatureTests
{
    [Collection("Controller Tests")]
    public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UsersControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task Index_ReturnsViewWithUsers()
        {
            var context = GetDbContext();
            context.Users.Add(new User { Name = "Skylar", Username = "ssandler" });
            context.Users.Add(new User { Name = "Scott", Username = "sganz" });
            context.SaveChanges();

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Users");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Skylar", html);
            Assert.Contains("sganz", html);
            Assert.DoesNotContain("Lisa", html);
        }

        [Fact]
        public async Task New_ReturnsViewWithForm()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/Users/New");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"<form method=\"post\" action=\"/users\">", html);
        }

        [Fact]
        public async void Create_AddsUserToDatabase()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            context.Users.Add(new User { Name = "Skylar", Username = "ssandler" });
            context.SaveChanges();

            var formData = new Dictionary<string, string>
            {
                { "Name", "Skylar" },
                { "Username", "ssandler" }
            };

            var response = await client.PostAsync("/users", new FormUrlEncodedContent(formData));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Skylar", html);

            var savedUser = context.Users.FirstOrDefault(
               u => u.Name == "Skylar"
           );
            Assert.NotNull(savedUser);
            Assert.Equal("Skylar", savedUser.Name);
        }

        [Fact]
        public async Task Show_ReturnsItemDetails()
        {
            var context = GetDbContext();
            context.Users.Add(new User { Name = "Skylar", Username = "ssandler" });
            context.Users.Add(new User { Name = "Scott", Username = "sganz" });
            context.SaveChanges();

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Users/1");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains("Skylar", html);
            Assert.DoesNotContain("Scott", html);
            Assert.Contains("ssandler", html);
            Assert.DoesNotContain("sganz", html);
        }
    }
}