﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using OBTS.API.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.SqlTypes;
using System.Data.Entity.Validation;

namespace OBTS.API.Models
{
    public class ApplicationDbContext  : IdentityDbContext<ApplicationUser>
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ApplicationDbContext () : base("name=ApplicationDbContext ")
        {
            //Database.Initialize(true);
        }

        public static ApplicationDbContext  Create()
        {
           
            return new ApplicationDbContext ();
        }


        public override int SaveChanges()
        {
            UpdatedChangedAtDateTimestamps();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            try { 
                UpdatedChangedAtDateTimestamps();

                return base.SaveChangesAsync();
            }

            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        private void UpdatedChangedAtDateTimestamps()
        {
            var changedAtEntities = ChangeTracker.Entries()
                .Where(i => i.State != EntityState.Unchanged && i.Entity is EntityBase);
              //  .Where(i => );

           
            Guid userId = Guid.Empty;
            if (HttpContext.Current != null && HttpContext.Current.User!=null)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                
                var user = HttpContext.Current.User;

                ApplicationUser appUser = userManager.FindByName(user.Identity.Name);
                if(appUser!=null)
                    userId = Guid.Parse(appUser.Id);
                else userId = Guid.Parse("6eda40fc-0b54-4958-b52a-7f48601395da");
            }
            else
                userId = Guid.Parse("6eda40fc-0b54-4958-b52a-7f48601395da");

         //   Guid userId = Guid.Parse("6eda40fc-0b54-4958-b52a-7f48601395da");

            foreach (var entity in changedAtEntities)
            {
                if (entity.State == EntityState.Added) {

                    var values = entity.CurrentValues;
                   
                    ((EntityBase)entity.Entity).CreatedUtc = DateTime.UtcNow;
                    //need to replace with actual user
                    ((EntityBase)entity.Entity).CreatedBy = userId;

                    ((EntityBase)entity.Entity).ModifiedUtc = DateTime.UtcNow;
                    //need to replace with actual user
                    ((EntityBase)entity.Entity).ModifiedBy = userId;
                }
                else if (entity.State == EntityState.Modified)
                {
                    var values = entity.OriginalValues;

                    ((EntityBase)entity.Entity).CreatedUtc =(DateTime) values["CreatedUtc"];
                    //need to replace with actual user
                    ((EntityBase)entity.Entity).CreatedBy = Guid.Parse(values["CreatedBy"].ToString());

                    ((EntityBase)entity.Entity).ModifiedUtc = DateTime.UtcNow;
                    //need to replace with actual user
                    ((EntityBase)entity.Entity).ModifiedBy = userId;
                }
            }
        }
        /*
        protected virtual void InitializeDatabase()
        {
            if (!Database.Exists())
            {
                //Database.Initialize(true);
                new DatabaseInitializer().seed(this);
            }
        }*/


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Agent>().HasMany(i => i.City).WithRequired().WillCascadeOnDelete(false);
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
        }

        public System.Data.Entity.DbSet<OBTS.API.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.RefreshToken> RefreshTokens { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Agent> Agents { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.City> Cities { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Region> Regions { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Country> Countries { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Operator> Operators { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.CodeTable> CodeTables { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Bus> Buses { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.BusFeature> BusFeatures { get; set; }
        
        public System.Data.Entity.DbSet<OBTS.API.Models.OperatorAgent> OperatorAgents { get; set; }
        
        public System.Data.Entity.DbSet<OBTS.API.Models.Route> Routes { get; set; }
        
        
        public System.Data.Entity.DbSet<OBTS.API.Models.RoutePoint> RoutePoints { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Seat> Seats { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Bank> Banks { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Booking> Bookings { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.BookingDetail> BookingDetails { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.BookingPassenger> BookingPassengers { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.RouteSeat> RouteSeats { get; set; }

        public System.Data.Entity.DbSet<OBTS.API.Models.Dummy> Dummies { get; set; }

      
    }
}
