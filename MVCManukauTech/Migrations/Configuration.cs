using Microsoft.AspNet.Identity.EntityFramework;
using MVCManukauTech.Models;

namespace MVCManukauTech.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVCManukauTech.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MVCManukauTech.Models.ApplicationDbContext context)
        {
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                Email = "admin@example.com",
                PasswordHash = "ANbPlzjaUTJ37O2NGcdw1wFcnBgaw10+RpfBry8qSm53gF0aCYTeRD+sVjA1QjnJXw==",
                SecurityStamp = "d6679934-48cb-4d05-8538-6d39c2c8589f",
                PhoneNumberConfirmed = true,
                PhoneNumber = "9999999999",
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                LockoutEndDateUtc = null,
                AccessFailedCount = 0,
                UserName = "admin@example.com",
            };

            var adminRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin"
            };

            var memberShipLiteRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "MemberAssociate"
            };

            var membershipFullRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "MemberFull"
            };

            if (!context.Roles.Any(item => item.Name == "MemberAssociate"))
            {
                context.Roles.Add(memberShipLiteRole);
            }

            if (!context.Roles.Any(item => item.Name == "MemberFull"))
            {
                context.Roles.Add(membershipFullRole);
            }

            if (!context.Roles.Any(item => item.Name == "Admin"))
            {
                context.Roles.Add(adminRole);
            }

            if (!context.Users.Any(item => item.Email == "admin@example.com"))
            {
                context.Users.Add(adminUser);
            }

            context.SaveChanges();
        }
    }
}