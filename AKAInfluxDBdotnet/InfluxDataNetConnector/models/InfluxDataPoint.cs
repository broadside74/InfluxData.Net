using System;
using System.Collections.Generic;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector.models
{
    public class InfluxDataPoint
    {
        public InfluxDataPoint(string measurement, IDictionary<string, object> fields,
            IDictionary<string, object> tags = null, DateTime? utcTimestamp = null)
        {
            Measurement = measurement;
            Fields = fields;
            Tags = tags;
            UtcTimestamp = utcTimestamp;
        }

        public InfluxDataPoint()
        {
        }

        public string Measurement { get; set; }
        public IDictionary<string, object> Fields { get; set; }
        public IDictionary<string, object> Tags { get; set; }
        public DateTime? UtcTimestamp { get; set; }
    }
}