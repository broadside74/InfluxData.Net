using System.Threading.Tasks;
using AKAInfluxDBdotnet.InfluxDataNetConnector.Diagnostics.models;
using InfluxData.Net.InfluxDb;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector.Diagnostics
{
    public class DiagnosticsInflux
    {
        private readonly InfluxDbClient _influxDbClient;

        public DiagnosticsInflux(InfluxDbClient influxDbClient)
        {
            _influxDbClient = influxDbClient;
        }

        public DiagnosticsRuntimeConnector Runtime { get; set; }

        public DiagnosticsSystemConnector System { get; set; }

        public DiagnosticsBuildConnector Build { get; set; }

        public DiagnosticsNetworkConnector Network { get; set; }

        public async Task<DiagnosticsInflux> GetDiagnostics()
        {
            var response = await _influxDbClient.Diagnostics.GetDiagnosticsAsync();

            var diagnosticsRuntimeConnector = new DiagnosticsRuntimeConnector
            {
                Version = response.Runtime.Version,
                GOOS = response.Runtime.GOOS,
                GOARCH = response.Runtime.GOARCH,
                GOMAXPROCS = response.Runtime.GOMAXPROCS
            };
            Runtime = diagnosticsRuntimeConnector;

            var diagnosticsBuildConnector = new DiagnosticsBuildConnector
            {
                Branch = response.Build.Branch, Commit = response.Build.Commit, Version = response.Build.Version
            };
            Build = diagnosticsBuildConnector;

            var diagnosticsNetworkConnector = new DiagnosticsNetworkConnector
            {
                Hostname = response.Network.Hostname
            };
            Network = diagnosticsNetworkConnector;

            var diagnosticsSystemConnector =
                new DiagnosticsSystemConnector
                {
                    Started = response.System.Started,
                    CurrentTime = response.System.CurrentTime,
                    Uptime = response.System.Uptime,
                    PID = response.System.PID
                };
            System = diagnosticsSystemConnector;

            return this;
        }
    }
}