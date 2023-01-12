using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomPlanner.App;
using RoomPlanner.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FunctionalTests
{
    public class TStartup : Startup
    {
        private readonly Guid dbGuid;

        public TStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            dbGuid = Guid.NewGuid();
        }

        public override void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("RoomPlanner.Functionaltesting" + dbGuid);
            });
        }
    }
}
