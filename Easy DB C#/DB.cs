using Microsoft.EntityFrameworkCore;

namespace Easy_DB_C_
{
    public class DB : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test_GH;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>().ToTable("Tasks");

            modelBuilder.Entity<Task>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(t => t.EmployeeID);
        }
    }
}