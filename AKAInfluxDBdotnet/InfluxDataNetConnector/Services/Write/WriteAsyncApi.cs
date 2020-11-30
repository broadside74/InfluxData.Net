using System.Collections.Generic;
using System.Threading.Tasks;
using AKAInfluxDBdotnet.InfluxDataNetConnector.Services.InfluxDataPoint;
using InfluxData.Net.InfluxDb;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector.Services.Write
{
    public class WriteAsyncApi
    {
        private readonly InfluxDataPointService _influxDataPointService;
        private readonly InfluxDbClient _influxDbClient;

        public WriteAsyncApi(InfluxDbClient influxDbClient, InfluxDataPointService influxDataPointService)
        {
            _influxDbClient = influxDbClient;
            _influxDataPointService = influxDataPointService;
        }

        public async Task<bool> Write(models.InfluxDataPoint point, string database, string retentionPolicy)
        {
            var response =
                await _influxDbClient.Client.WriteAsync(_influxDataPointService.MakeIfInfluxDataPointFrom(point),
                    database, retentionPolicy);
            return response.Success;
        }

        public async Task<bool> Write(IEnumerable<models.InfluxDataPoint> points, string database,
            string retentionPolicy)
        {
            var response =
                await _influxDbClient.Client.WriteAsync(_influxDataPointService.MakeIfInfluxDataPointFrom(points),
                    database, retentionPolicy);
            return response.Success;
        }
    }
}