using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AKAInfluxDBdotnet.InfluxDataNetConnector.Diagnostics;
using AKAInfluxDBdotnet.InfluxDataNetConnector.models;
using AKAInfluxDBdotnet.InfluxDataNetConnector.Services.Database;
using AKAInfluxDBdotnet.InfluxDataNetConnector.Services.InfluxDataPoint;
using AKAInfluxDBdotnet.InfluxDataNetConnector.Services.Write;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector
{
    public class InfluxDataNetConnector
    {

        private readonly DatabaseService _databaseService;
        private readonly DiagnosticsInflux _diagnosticsInflux;

        private string Username { get; }
        private string Password { get; }
        
        /**
         * Retention Policy named autogen and with an indefinite duration should always be created
         */
        private string _retentionPolicy = "autogen";
        public string RetentionPolicy
        {
            get => _retentionPolicy;
            set
            {
                if (_databaseService.CheckIfRetentionPolicyExists(Database, value))
                    _retentionPolicy = value;
                else
                    throw new Exception(
                        $"There is no retention policy named : {value} in the actual influx db instance ({Url} => {Database}). " +
                        "You probably made a mistake in the name or forgot to create the retention policy. Please create the retention policy in Chronograf (:8888)" +
                        "or use DatabaseService.CreateRetentionPolicy");
            }
        }
        
        private bool AllowDbCreation { get; set; }
        public bool Enabled { get; set; }
        public string Url { get; }        
        public string Database { get; set;  }
        public string Name { get; }
        
        private readonly WriteAsyncApi _writeAsyncApi;
        public InfluxDbClient DbClient { get; }

        public InfluxDataNetConnector(string url, string username, string password, string database,
            bool allowDbCreation, string name)
        {
            /*
             * Services
             */
            DbClient = new InfluxDbClient(url, username, password, InfluxDbVersion.Latest);
            _diagnosticsInflux = new DiagnosticsInflux(DbClient);
            _writeAsyncApi = new WriteAsyncApi(DbClient, new InfluxDataPointService());
            _databaseService = new DatabaseService(DbClient, allowDbCreation);
            Url = url;
            Username = username;
            Password = password;
            Database = database;
            Name = name;
            _databaseService.UseCreateDb(database);
        }
        public InfluxDataNetConnector(string url, string username, string password, bool allowDbCreation, string name)
        {
            /*
             * Services
             */
            DbClient = new InfluxDbClient(url, username, password, InfluxDbVersion.Latest);
            _diagnosticsInflux = new DiagnosticsInflux(DbClient);
            _writeAsyncApi = new WriteAsyncApi(DbClient, new InfluxDataPointService());
            _databaseService = new DatabaseService(DbClient, allowDbCreation);
            Url = url;
            Username = username;
            Password = password;
            Name = name;
        }

        
        public async Task<bool> Ping()
        {
            Task task = DbClient.Diagnostics.PingAsync();
            return await Task.WhenAny(task, Task.Delay(1000)) == task;
        }

        public DiagnosticsInflux GetDiagnostics()
        {
            return _diagnosticsInflux.GetDiagnostics().Result;
        }

        /**
         * Write InfluxDataPoint
         */
        public bool WriteInfluxDataPoint(InfluxDataPoint point)
        {
            return _writeAsyncApi.Write(point, Database, RetentionPolicy).Result;
        }

        /**
         * Write Raw measurement
         */
        public bool WriteInfluxDataPoint(string measurement, IDictionary<string, object> fields,
            IDictionary<string, object> tags = null, DateTime? utcTimestamp = null)
        {
            return _writeAsyncApi.Write(new InfluxDataPoint(measurement, fields, tags, utcTimestamp), Database,
                RetentionPolicy).Result;
        }

        /**
         * Write multiple InfluxDataPoints
         */
        public bool WriteInfluxDataPoint(IEnumerable<InfluxDataPoint> points)
        {
            return _writeAsyncApi.Write(points, Database, RetentionPolicy).Result;
        }

        public bool CreateRetentionPolicy(string database, string retentionPolicyName, string duration,
            int replicationFactor = 1)
        {
            return _databaseService.CreateRetentionPolicy(database, retentionPolicyName, duration, replicationFactor);
        }

        /**
         * Continuous Query
         */
        public bool CreateContinuousQuery(CqParams cqParams)
        {
            return DbClient.ContinuousQuery.CreateContinuousQueryAsync(cqParams).Result.Success;
        }
    }
}