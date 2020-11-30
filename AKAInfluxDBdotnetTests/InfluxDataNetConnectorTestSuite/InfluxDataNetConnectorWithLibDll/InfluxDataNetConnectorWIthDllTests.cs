using System.Linq;
using AKAInfluxDBdotnet.InfluxDataNetConnector;
using AKAInfluxDBdotnet.utils;
using Xunit;
using Xunit.Abstractions;

namespace AKAInfluxDBdotnetTests.InfluxDataNetConnectorTestSuite.InfluxDataNetConnectorWithLibDll
{
    public class InfluxDataNetConnectorWIthDllTests
    {
        private const string Database = "WithDll_influxDataNetConnector_tests";
        private const string RetentionPolicy = "rp_test";

        private readonly InfluxDataNetConnector _influxDataNetConnector;
        private readonly MeasureTime _measureTime;
        private readonly ITestOutputHelper _testOutputHelper;

        public InfluxDataNetConnectorWIthDllTests(ITestOutputHelper testOutputHelper)
        {
            _measureTime = new MeasureTime();
            _testOutputHelper = testOutputHelper;
            _influxDataNetConnector =
                new InfluxDataNetConnector("http://127.0.0.1:8086/", "root", "password", Database, true, "AKATEST");
        }

        private void Cleanup()
        {
        }

        [Fact]
        private void CreateRetentionPolicyTest()
        {
            Assert.True(_influxDataNetConnector.CreateRetentionPolicy(Database, RetentionPolicy, "1d"));
            _influxDataNetConnector.RetentionPolicy = RetentionPolicy;
            var retentions = _influxDataNetConnector.DbClient.Retention.GetRetentionPoliciesAsync(Database).Result;
            Assert.True(retentions.Any(r => r.Name.Equals(RetentionPolicy)));
            Assert.True(_influxDataNetConnector.DbClient.Retention.DropRetentionPolicyAsync(Database, RetentionPolicy)
                .Result.Success);
            Assert.False(_influxDataNetConnector.DbClient.Retention.GetRetentionPoliciesAsync(Database).Result
                .Any(db => db.Name.Equals(RetentionPolicy)));
        }
    }
}