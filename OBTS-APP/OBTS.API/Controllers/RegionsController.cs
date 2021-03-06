﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
//using OBTSAPI.DbContexts;
using OBTS.API.Models;
using System.Configuration;
using System.Linq.Expressions;
using OBTS.API.Models.DTO;

namespace OBTS.API.Controllers
{
    public class RegionsController : ApiController
    {
        private ApplicationDbContext  db = new ApplicationDbContext ();

        // Typed lambda expression for Select() method. 
        private static readonly Expression<Func<City, CitiesRegionDTO>> AsCitiesRegionDTO =
            x => new CitiesRegionDTO
            {
                CityId = x.CityId,
                CityDesc = x.CityDesc,
            };

        // GET: api/Regions
        [ResponseType(typeof(CountryRegionDTO))]
        public async Task<IHttpActionResult> GetRegions()
        {
            var regions =await( from b in db.Regions
                          select new CountryRegionDTO()
                        {
                            RegionId = b.RegionId,
                            RegionDesc = b.RegionDesc,
                            CountryId= b.CountryId,
                            CountryDesc = b.Country.CountryDesc
                        }).ToListAsync();
            return Ok(regions.AsQueryable());
        }

        // GET: api/region/1/cities
        [ResponseType(typeof(CitiesRegionDTO))]
        [Route("api/region/{Id}/cities", Name = "GetCitiesByRegion")]
        public async Task<IHttpActionResult> GetCitiesByRegion(Guid Id)
        {
            // City city = await db.Cities.FindAsync(id);
            var cities=await( db.Cities.Include(b => b.CountryRegion)
            .Where(b => b.CountryRegion.RegionId == Id)
            .Select(AsCitiesRegionDTO)).ToListAsync();
            return Ok(cities.AsQueryable());
        }

        // GET: api/region/{Id}/operators
        [ResponseType(typeof(OperatorDTO))]
        [Route("api/region/{Id}/operators", Name = "GetOperatorsByRegion")]
        public async Task<IHttpActionResult> GetOperatorsByRegion(Guid Id)
        {
            var _operators =await( from o in db.Operators
                .Include(o => o._city)
                .Include(o => o._city.CountryRegion)
                .Include(o => o._city.CountryRegion.Country)
                .Where(o => o._city.CountryRegion.RegionId == Id)
                             select new OperatorDTO()
                             {
                                 OperatorId = o.OperatorId,
                                 FirstName = o.FirstName,
                                 LastName = o.LastName,
                                 Mobile = o.Mobile,
                                 EmailAddress = o.EmailAddress,
                                 PhoneNumber = o.PhoneNumber,
                                 Company = o.Company,
                                 CompanyPhone = o.CompanyPhone,
                                 Address = o.Address,
                                 CountryId = o._city.CountryRegion.Country.CountryId,
                                 CountryName = o._city.CountryRegion.Country.CountryDesc,
                                 RegionId = o._city.CountryRegion.RegionId,
                                 RegionName = o._city.CountryRegion.RegionDesc,
                                 CityId = o.CityId,
                                 CityName = o._city.CityDesc,
                                 NumberOfBuses = o.NumberOfBuses,
                                 NumberOfRoutes = o.NumberOfRoutes,
                                 Status = o.Status,
                                 UserName = o.UserName,
                                 Password = o.Password
                             }).ToListAsync();


            return Ok(_operators.AsQueryable());
        }

        // GET: api/region/1
        [ResponseType(typeof(CountryRegionDTO))]
        //[ActionName("region")]
        [Route("api/region/{Id}", Name = "GetCountryRegion")]
        public async Task<IHttpActionResult> GetCountryRegion(Guid id)
        {
            var countryRegion = await db.Regions.Include(b => b.Country).Select(b =>
               new CountryRegionDTO()
               {
                   RegionId = b.RegionId,
                   RegionDesc = b.RegionDesc,
                   CountryId = b.CountryId,
                   CountryDesc = b.Country.CountryDesc
               }).SingleOrDefaultAsync(b => b.RegionId == id);
            if (countryRegion == null)
            {
                return NotFound();
            }

            return Ok(countryRegion);
        }

        // PUT: api/Regions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCountryRegion(Guid id, Region countryRegion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != countryRegion.RegionId)
            {
                return BadRequest();
            }

            db.Entry(countryRegion).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryRegionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Regions
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> PostCountryRegion(Region countryRegion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            countryRegion.RegionId = Guid.NewGuid();
            db.Regions.Add(countryRegion);
            await db.SaveChangesAsync();

            // Load country 
            db.Entry(countryRegion).Reference(x => x.Country).Load();

            var dto = new CountryRegionDTO()
            {
                RegionId = countryRegion.RegionId,
                RegionDesc = countryRegion.RegionDesc,
                CountryId = countryRegion.CountryId,
                CountryDesc = countryRegion.Country.CountryDesc
            };
            return CreatedAtRoute("DefaultApi", new { id = countryRegion.RegionId }, countryRegion);
        }

        // DELETE: api/Regions/5
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> DeleteCountryRegion(Guid id)
        {
            Region countryRegion = await db.Regions.FindAsync(id);
            if (countryRegion == null)
            {
                return NotFound();
            }

            db.Regions.Remove(countryRegion);
            await db.SaveChangesAsync();

            return Ok(countryRegion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CountryRegionExists(Guid id)
        {
            return db.Regions.Count(e => e.RegionId == id) > 0;
        }
    }
}