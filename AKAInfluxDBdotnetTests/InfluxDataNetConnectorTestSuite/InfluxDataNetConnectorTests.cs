using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using AKAInfluxDBdotnet.InfluxDataNetConnector;
using AKAInfluxDBdotnet.InfluxDataNetConnector.models;
using AKAInfluxDBdotnet.utils;
using Xunit;
using Xunit.Abstractions;

namespace AKAInfluxDBdotnetTests.InfluxDataNetConnectorTestSuite
{
    public class InfluxDataNetConnectorTests
    {
        private const string Database = "influxDataNetConnector_tests";
        private const string RetentionPolicy = "autogen";
        private readonly InfluxDataNetConnector _influxDataNetConnector;
        private readonly MeasureTime _measureTime;
        private readonly ITestOutputHelper _testOutputHelper;

        public InfluxDataNetConnectorTests(ITestOutputHelper testOutputHelper)
        {
            _measureTime = new MeasureTime();
            _testOutputHelper = testOutputHelper;
            // BE CAREFUL TO PUT THE IP (127.0.0.1) INSTEAD OF LOCALHOST IF THE INFLUXDB SERVER IS ON THE SAME MACHINE (50ms vs 7ms send batch 100 points)
            // HERE THERE IS LOCALHOST TO BE GENERIC TEST
            _influxDataNetConnector =
                new InfluxDataNetConnector("http://127.0.0.1:8086/", "root", "password", Database, true, "AKATEST");
            _influxDataNetConnector.RetentionPolicy = RetentionPolicy;
        }

        private InfluxDataPoint createPoint(DateTime? time)
        {
            return new InfluxDataPoint
            {
                Measurement = "reading", // serie/measurement/table to write into
                Tags = new Dictionary<string, object>
                {
                    {"SensorId", 8},
                    {"SerialNumber", "00AF123B"}
                },
                Fields = new Dictionary<string, object>
                {
                    {"SensorState", "act"},
                    {"Humidity", 431},
                    {"Temperature", 22.1},
                    {"Resistance", 34957}
                },
                UtcTimestamp = time // optional (can be set to any DateTime moment)
            };
        }

        [Fact]
        private void InfluxDataNetConnectorPingTest()
        {
            var result = _influxDataNetConnector.Ping().Result;
            Assert.True(result);
        }

        [Fact]
        private void InfluxDataNetConnectorDiagnosticsTest()
        {
            var result = _influxDataNetConnector.GetDiagnostics();
            Assert.True(result.System.Uptime.TotalSeconds > 0);
        }

        [Fact]
        private void InfluxDataNetConnectorWriteTest()
        {
            var point = createPoint(null);
            var t = _measureTime.Measure(() => _influxDataNetConnector.WriteInfluxDataPoint(point),
                str => _testOutputHelper.WriteLine(str));
            Assert.True(t);
        }

        /**
         * Please note that ForLoop with write single point has very poor performances due to http overhead. Please send an array of point (Batch instead)
         */
        [Fact]
        private void InfluxDataNetConnectorWriteForLoopTest()
        {
            var utcTimestamp = DateTime.UtcNow;
            double meanTime = 0;
            double totalTime = 0;
            var point = createPoint(null);
            var stopWatch = new Stopwatch();
            for (var i = 1; i <= 1000; i++)
            {
                stopWatch.Start();
                var result = _influxDataNetConnector.WriteInfluxDataPoint(point);
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                var ts = stopWatch.Elapsed;
                if (i > 10)
                {
                    totalTime += ts.TotalMilliseconds;
                    meanTime = totalTime / i;
                }

                Assert.True(result);
                stopWatch.Reset();
            }

            _testOutputHelper.WriteLine("Meantime (ms) " + meanTime);
            _testOutputHelper.WriteLine("Total Time (ms) " + totalTime);
        }

        [Fact]
        private void InfluxDataNetConnectorWriteBatchTest()
        {
            var points = new List<InfluxDataPoint>();
            var now = DateTime.UtcNow;
            for (var i = 0; i < 1000; i++)
            {
                //Last 1000 seconds, one point par second
                var point = createPoint(now.AddSeconds(-i));
                points.Add(point);
            }

            var result = _measureTime.Measure(() => _influxDataNetConnector.WriteInfluxDataPoint(points),
                str => _testOutputHelper.WriteLine(str));
            Assert.True(result.Result);
        }

        [Fact]
        private void PingAsync()
        {
            for (int i = 0; i < 1000; i++)
            {
                var result = _measureTime.Measure(() => _influxDataNetConnector.DbClient.Diagnostics.PingAsync().Result,
                    str => _testOutputHelper.WriteLine(str));
                _testOutputHelper.WriteLine(result.ResponseTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
                Assert.True(result.Success);
            }
            
        }
    }
}