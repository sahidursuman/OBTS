﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBTS.API.Models.DTO
{
    public class CountryRegionDTO
    {
        public Guid RegionId { get; set; }
        public string RegionDesc { get; set; }
        public Guid CountryId { get; set; }
        public string CountryDesc { get; set; }
    }
}