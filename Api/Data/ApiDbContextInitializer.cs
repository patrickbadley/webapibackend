using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data
{
    public static class ApiDbContextInitializer
    {
        public class Realtor
        {
            public string Name { get; set; }
        }

        public static void Initialize(ApiDbContext context)//SchoolContext is EF context
        {

            context.Database.Migrate();//if db is not exist ,it will create database .but ,do nothing .

            // Look for any students.
            if (context.Property.Any())
            {
                return;   // DB has been seeded
            }


            //Set the randomzier seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(8675309);

            var testRealtors = new Faker<Realtor>()
                //Basic rules using built-in generators
                .RuleFor(u => u.Name, (f, u) => f.Name.FullName()).Generate(25);

            var visitIds = 1;
            var testVisits = new Faker<Visit>()
                .RuleFor(o => o.VisitId, f => visitIds++)
                //Basic rules using built-in generators
                .RuleFor(u => u.Date, (f, u) => f.Date.Between(DateTime.Now.AddDays(-200), DateTime.Now));

            var propertyIds = 1;
            var testProperties = new Faker<Property>()
                .RuleFor(o => o.PropertyId, f => propertyIds++)
                //Basic rules using built-in generators
                .RuleFor(u => u.OwnerName, (f, u) => f.Name.FullName())
                .RuleFor(u => u.RealtorName, f => f.PickRandom(testRealtors).Name)
                .RuleFor(u => u.Address, f => f.Address.StreetAddress())
                .RuleFor(u => u.City, (f, u) => f.Address.City())
                .RuleFor(u => u.State, (f, u) => f.Address.State())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode())
                .RuleFor(u => u.Visits, (f, u) =>
                {
                    var visits = testVisits.Generate(f.PickRandom(Enumerable.Range(0, 20)));

                    //visits.ForEach(visit =>
                    //{
                    //    visit.PropertyId = u.PropertyId;
                    //});

                    return visits;
                });

            var properties = testProperties.Generate(250);

//            context.Visit.AddRange(user.SelectMany(u => u.Visits));

            context.Property.AddRange(properties);

            context.SaveChanges();
        }
    }
}