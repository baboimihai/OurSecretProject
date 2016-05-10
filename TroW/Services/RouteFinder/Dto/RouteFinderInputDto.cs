﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RouteFinder.Dto
{
    public class RouteFinderInputDto
    {
        public int UserId { get; set; }
        public List<int> StationToGoThrow { get; set; }
        public List<int> StationToSkip { get; set; }
        public TimeSpan? OraPlecare { get; set; }
        public TimeSpan? OraSosire { get; set; }
        public DateTime? DataPlecare { get; set; }
    }
}
