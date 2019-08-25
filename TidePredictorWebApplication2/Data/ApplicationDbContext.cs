using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TidePredictorWebApplication2.Models;

namespace TidePredictorWebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tide> Tides { get; set; }
        public DbSet<HulhuleTide> HulhuleTides { get; set; }
        public DbSet<HanimaadhooTide> HanimaadhooTides { get; set; }
        public DbSet<GanTide> GanTides { get; set; }
        public DbSet<TideStation> TideStations { get; set; }
       

        

        ////protected override void OnModelCreating(ModelBuilder builder)
        ////{
        ////    base.OnModelCreating(builder);
        ////    // Customize the ASP.NET Identity model and override the defaults if needed.
        ////    // For example, you can rename the ASP.NET Identity table names and more.
        ////    // Add your customizations after calling base.OnModelCreating(builder);
        ////}
    }
}
