using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Api.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Property> Property { get; set; }
        public DbSet<Visit> Visit { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=property-db.db");
        }
    }

    public class Property
    {
        public int PropertyId { get; set; }
        public string OwnerName { get; set; }
        public string RealtorName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public ICollection<Visit> Visits { get; set; }
    }
    public class Visit
    {
        public int VisitId { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public DateTime Date { get; set; }
    }
}
