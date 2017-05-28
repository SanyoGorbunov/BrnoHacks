using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebApp.Models;

namespace WebApp.DAL
{
    public class GoVisiblyContext: DbContext
    {
        public GoVisiblyContext() : base("GoVisiblyContext")
        {
        }

        public DbSet<ZsjLocation> ZsjLocations { get; set; }

        public DbSet<OTwoData> O2Datas { get; set; }

        public DbSet<VehicleData> VehicleDatas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer<GoVisiblyContext>(null);
        }
    }
}