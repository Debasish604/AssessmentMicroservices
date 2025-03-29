using Microsoft.EntityFrameworkCore;
using AssessmentServices.Models.Entity;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AssessmentServices.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Assessment> JobAssessments { get; set; }
        public DbSet<JDBasedAIMCQ> JDBasedAIMCQ { get; set; }

        public DbSet<GenericThreshold> GenericThresholds { get; set; }

        public DbSet<JobApplications> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map table to schema and name
            modelBuilder.Entity<Assessment>().ToTable("JobAssessments", schema: "ADANI_TALENT");

        }
    }
}
