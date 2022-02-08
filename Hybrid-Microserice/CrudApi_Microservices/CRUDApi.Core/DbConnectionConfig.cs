using CRUD.Transaction.CRUDApi.Core.Extensions;
using CRUD.Transaction.CRUDApi.Core.Interface;
using CRUD.Transaction.Types;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core
{
    public enum ConnectionCatalogType { Metadata, AcuteCare, Stage } 

    public class DbConnectionConfig : IConnectionConfig
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        private const int _DefaultCommandTimeout = 30;
        private const string _ConnectionName = "SCAMD";
        private   IConfiguration _configuration;
    
        public  DbConnectionConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private DbConnectionConfig(string connection, int timeout)
        {
            ConnectionString = connection;
            CommandTimeout = timeout;
        }
         
        private int GetCommandTimeout()
        {
            int timeout = _DefaultCommandTimeout;
            if (!int.TryParse(_configuration.GetAppSettingValue(ConfigurationKeyType.App.CommandTimeout), out timeout))
            { timeout = _DefaultCommandTimeout; }
            return timeout;
        }


        private IConnectionConfig GetConnectionConfig(string name)
        {
            int timeout = GetCommandTimeout();
            try
            {
                var value = _configuration.GetConnectionString(name);
                if (!string.IsNullOrEmpty(value))
                { return CreateFromConnection(value, timeout); }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error accessing connection name:  {name}", ex);
            }
            return null;
        }

        public IConnectionConfig DbConnection(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                connectionName = _ConnectionName;
            }
            IConnectionConfig conn = GetConnectionConfig(connectionName);

            if (string.IsNullOrEmpty(conn.ConnectionString)) throw new ArgumentNullException("connection", "DbConnectionConfig.CreateFromConnection error - connection is null");

            return conn;
        }


        public static IConnectionConfig CreateFromConnection(string connection, int timeout)
        {
            if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException("connection", "DbConnectionConfig.CreateFromConnection error - connection is null");

            return new DbConnectionConfig(connection, timeout);
        }

        /// <summary>
        /// Create ConnectionConfig from an existing config swapping out the catalog type from current config for specified catalog type
        /// </summary>
        /// <param name="config">ConnectionConfig in context</param>
        /// <param name="catalogType">Type of catalog for resulting ConnectionConfig, defaulted to Metadata</param>
        /// <returns>ConnectionConfig with same server and specified database, based on catalogType</returns>
        public static IConnectionConfig CreateAsConnectionType(IConnectionConfig config, ConnectionCatalogType catalogType = ConnectionCatalogType.Metadata)
        {
            if (string.IsNullOrEmpty(config.ConnectionString)) throw new ArgumentNullException("connection", "DbConnectionConfig.CreateAsAcuteCareConnection error - connection is null");
            if (config.ConnectionString.ToLower().IndexOf("_metadata") < 0) throw new ArgumentException("connection", "DbConnectionConfig.CreateAsAcuteCareConnection error - connection string must include Metadata database as initial catalog.");

            return new DbConnectionConfig(
                config.ConnectionString.Replace("_Metadata", $"_{catalogType.ToString()}", StringComparison.CurrentCultureIgnoreCase),
                config.CommandTimeout);
        }

    }
}
