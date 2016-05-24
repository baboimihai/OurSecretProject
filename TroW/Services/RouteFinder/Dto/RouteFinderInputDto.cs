using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RouteFinder.Dto
{
    public class RouteFinderInputDto
    {
        public Guid UserId { get; set; }
        public List<ViaPlaningDto> StationToGoThrow { get; set; }// [0]- statie plecare.. [length-1]-statie finala,,, 1---length-2---puncte intermediare
        public List<int> StationToSkip { get; set; }
        public TimeSpan? OraPlecare { get; set; }
        public TimeSpan? OraSosire { get; set; }
        public DateTime? DataPlecare { get; set; }
        public string YearIdentifier { get; set; }
        public bool Return { get; set; }
    }
    public class ViaPlaningDto
    {
        public int SencondsToWait { get; set; }
        public int StationCode { get; set; }
    }
}
