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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}