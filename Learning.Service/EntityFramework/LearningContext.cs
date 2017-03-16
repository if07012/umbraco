using System.Data.Entity;
using Learning.Entities;

namespace Learning.Service.EntityFramework
{
    public class LearningContext : DbContext
    {
        public LearningContext() : base("umbracoDbDSN")
        {

        }

        public LearningContext(string conn) : base(conn)
        {

        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
    }
}