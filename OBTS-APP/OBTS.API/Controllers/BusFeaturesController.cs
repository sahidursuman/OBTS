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

namespace OBTSAPI.Controllers
{
    public class BusFeaturesController : ApiController
    {
        private OBTSAPIContext db = new OBTSAPIContext();

        private static readonly Expression<Func<CodeTable, CodeValueDTO>> AsCodeValueDTO =
           a => new CodeValueDTO
           {
               KeyCode = a.KeyCode,
               Value = a.Value,
           };

        // GET: api/BusesFeatures
        [Route("api/busesfeatures", Name = "GetBusFeatures")]
        public IQueryable<BusFeatureDTO> GetBusFeatures()
        {
            var features = from b in db.BusFeatures
                        join ct in db.CodeTables on b.BusFeatureCode equals ct.KeyCode
                        where ct.Title.Equals("BusFeatures")

                        select new BusFeatureDTO()
                        {
                            BusFeatureId = b.BusFeatureId,
                            BusId =b.BusId,
                            BusFeatureCode=ct.KeyCode,
                            BusFeatureDesc =ct.Value
                        };
            return features;
        }

        // GET: api/bus/{id}/features
        [Route("api/bus/{Id}/features", Name = "GetFeaturesByBus")]
        public IQueryable<BusFeatureDTO> GetFeaturesByBus(Guid Id)
        {

            var features = from b in db.BusFeatures
                           join ct in db.CodeTables on b.BusFeatureCode equals ct.KeyCode
                           where ct.Title.Equals("BusFeatures") && b.BusId.Equals(Id)

                           select new BusFeatureDTO()
                           {
                               BusFeatureId = b.BusFeatureId,
                               BusId = b.BusId,
                               BusFeatureCode = ct.KeyCode,
                               BusFeatureDesc = ct.Value
                           };

            return features;
        }

        // GET: api/Bus/Feature/5
        [ResponseType(typeof(BusFeature))]
        [Route("api/bus/feature/{Id}", Name = "GetBusFeature")]
        public async Task<IHttpActionResult> GetBusFeature(Guid id)
        {
            BusFeature busFeature = await db.BusFeatures.FindAsync(id);
            if (busFeature == null)
            {
                return NotFound();
            }

            return Ok(busFeature);
        }

        // PUT: api/BusFeatures/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBusFeature(Guid id, BusFeature busFeature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != busFeature.BusFeatureId)
            {
                return BadRequest();
            }

            db.Entry(busFeature).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusFeatureExists(id))
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

        // POST: api/BusFeatures
        [ResponseType(typeof(BusFeature))]
        public async Task<IHttpActionResult> PostBusFeature(BusFeature busFeature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BusFeatures.Add(busFeature);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BusFeatureExists(busFeature.BusFeatureId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = busFeature.BusFeatureId }, busFeature);
        }

        // DELETE: api/BusFeatures/5
        [ResponseType(typeof(BusFeature))]
        public async Task<IHttpActionResult> DeleteBusFeature(Guid id)
        {
            BusFeature busFeature = await db.BusFeatures.FindAsync(id);
            if (busFeature == null)
            {
                return NotFound();
            }

            db.BusFeatures.Remove(busFeature);
            await db.SaveChangesAsync();

            return Ok(busFeature);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BusFeatureExists(Guid id)
        {
            return db.BusFeatures.Count(e => e.BusFeatureId == id) > 0;
        }
    }
}