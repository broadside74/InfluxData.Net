using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.InfluxDb.Models;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector.Services.InfluxDataPoint
{
    public class InfluxDataPointService
    {
        public Point MakeIfInfluxDataPointFrom(models.InfluxDataPoint point)
        {
            return new Point
            {
                Fields = point.Fields,
                Name = point.Measurement,
                Tags = point.Tags,
                Timestamp = point.UtcTimestamp
            };
        }

        public IEnumerable<Point> MakeIfInfluxDataPointFrom(IEnumerable<models.InfluxDataPoint> points)
        {
            return points.Select(MakeIfInfluxDataPointFrom);
        }
    }
}