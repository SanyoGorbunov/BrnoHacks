using System.Data.Entity;

namespace WebApp.DAL
{
    public class GoVisiblyInitializer: DropCreateDatabaseIfModelChanges<GoVisiblyContext>
    {
        protected override void Seed(GoVisiblyContext context)
        {
            context.SaveChanges();
        }
    }
}