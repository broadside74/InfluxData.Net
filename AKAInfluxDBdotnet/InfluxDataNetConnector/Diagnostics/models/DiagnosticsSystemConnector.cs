using System;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector.Diagnostics.models
{
    public class DiagnosticsSystemConnector
    {
        public long PID { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime Started { get; set; }

        public TimeSpan Uptime { get; set; }
    }
}