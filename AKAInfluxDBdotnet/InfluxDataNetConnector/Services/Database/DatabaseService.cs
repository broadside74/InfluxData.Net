using System;
using System.Linq;
using InfluxData.Net.InfluxDb;

namespace AKAInfluxDBdotnet.InfluxDataNetConnector.Services.Database
{
    public class DatabaseService
    {
        private readonly bool _allowDbCreation;
        private readonly InfluxDbClient _influxDbClient;

        public DatabaseService(InfluxDbClient influxDbClient, bool allowDbCreation)
        {
            _influxDbClient = influxDbClient;
            _allowDbCreation = allowDbCreation;
        }

        public bool CheckIfDatabaseExist(string db)
        {
            var databasesAsync = _influxDbClient.Database.GetDatabasesAsync().Result;
            var enumerable = databasesAsync.ToList();
            foreach (var database in enumerable)
            {
                if (database.Name.Equals(db))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CreateDatabase(string databaseName)
        {
            if (CheckIfDatabaseExist(databaseName)) return false;
            return _influxDbClient.Database.CreateDatabaseAsync(databaseName).Result.Success;
        }

        public TOut UseCreateDb<TOut>(Func<TOut> handler, string db)
        {
            if (CheckIfDatabaseExist(db)) return handler();
            if (!_allowDbCreation)
                throw new Exception($"Database : {db} not found, property 'AllowDbCreation' is set to false");
            CreateDatabase(db);
            return handler();
        }

        public void UseCreateDb(string db)
        {
            if (CheckIfDatabaseExist(db)) return;
            if (!_allowDbCreation)
                throw new Exception($"Database : {db} not found, property 'AllowDbCreation' is set to false");
            CreateDatabase(db);
        }

        public bool CheckIfRetentionPolicyExists(string database, string retentionPolicy)
        {
            var retentionPoliciesAsync = _influxDbClient.Retention.GetRetentionPoliciesAsync(database).Result;
            var enumerable = retentionPoliciesAsync.ToList();
            foreach (var c in enumerable)
            {
                if (c.Name.Equals(retentionPolicy)) return true;
            }

            return false;
        }

        public bool CreateRetentionPolicy(string database, string retentionPolicyName, string duration,
            int replicationFactor = 1)
        {
            if (CheckIfRetentionPolicyExists(database, retentionPolicyName))
                throw new Exception(
                    $"Retention Policy named : {retentionPolicyName} already exists in database : {database}");
            return _influxDbClient.Retention
                .CreateRetentionPolicyAsync(database, retentionPolicyName, duration, replicationFactor).Result.Success;
        }
    }
}