using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace PortfolioApi {
    public class DatabaseContext :DbContext {
        public DbSet<CounterDoc> Counters { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var containerId = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_ID");

            modelBuilder.Entity<CounterDoc>()
                .ToContainer(containerId) 
                .HasPartitionKey(x => x.PageId);  
            modelBuilder.Entity<CounterDoc>()
                .HasKey(x => x.Id);
        }
        
    }
}
