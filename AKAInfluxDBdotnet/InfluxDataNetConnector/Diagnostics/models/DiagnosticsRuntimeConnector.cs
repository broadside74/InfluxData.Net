namespace AKAInfluxDBdotnet.InfluxDataNetConnector.Diagnostics.models
{
    public class DiagnosticsRuntimeConnector
    {
        public string GOARCH { get; set; }

        public long GOMAXPROCS { get; set; }

        public string GOOS { get; set; }

        public string Version { get; set; }
    }
}