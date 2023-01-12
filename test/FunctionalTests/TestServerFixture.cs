using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomPlanner.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTests
{
    public class TestServerFixture : IDisposable
    {
        public const string Environment = "Testing";

        private readonly TestServer _server;

        public IWebHost Host { get; private set; }

        public TestServerFixture()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment(Environment)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{Environment}.json"))
                    .Build())
                .UseStartup<TStartup>();

            // inject chirpstack backend-mockup
            builder.ConfigureTestServices(services =>
            {
                var provider = services.BuildServiceProvider();

                // replace services with mocks if needed
            });

            _server = new TestServer(builder);

            var dataContext = _server.Host.Services.GetRequiredService<ApplicationDbContext>();
            DataSeeder.SeedDataAsync(dataContext).Wait();

            Host = _server.Host;
        }

        public HttpClient CreateHttpClient()
        {
            return _server.CreateClient();
        }

        public IServiceProvider Services
        {
            get { return _server.Services; }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
