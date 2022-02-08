using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Common
{

    #region AppSettingKey enum
    public enum AppConfigSettingKey : int
    {
        SQLServerServerName,
        SQLServerDatabaseName,
        TempFolder,
        SQLServerName,
        SQLServerMetadata,
        ErrorLogName,
        SessionTimeout,
        CommandTimeout,
        CacheExpiration,
        SharepointSiteURL,
        ReportingServicesURL,
        ReportingServicesFolderRoot,
        VisitListRelativePath,

        ReportBuilderPath,

        AnalysisServicesServerName,
        AnalysisServicesCatalog,

        SQLServerRepository,
        SQLServerStagingDatabase,
        DefaultNetworkDomain,

        ProclarityURL,

        MailServer,
        AdminEmailAddress,

       
        OLAPConnectionString,
        SCAConnectionString,
        CurrentUser,
  
        ReportLogging,

        ApplicationBannerImage,
        ApplicationBannerTitle,
        OLAPConnection,
        //Till here
        MetadataConnection,
        AnalyticsRepositoryConnection,
        StageConnection,
        XMLConnection,

        // EPM config items        
        PortalService,
        PortalURL,
        PortalID,

        //Extractor Service Values
        ExtractorRealTimeLatency,
        ExtractorSleepTime,
        ExtractorServerName,

        // SCA Service Values
        ServiceHost,
        ServiceDirectory,

        //Application Profile Service Server Name added
        ProfilerServiceServerName,
        ProfilerServiceSleepTime,
        //Audit 
        AuditPurgingRecords,
        AuditPurgingDays,

        // AnalysisServices
        AnalysisServicesProvider,
        AnalysisServicesDatasourceProvider,

        // WCF services operation timeout.
        OperationTimeout,

        ProfilerLockWaitTimeout,    // Task 1976321

        ProfilerServiceName,
        ExtractorServiceName,


        ProfileFreezingDays,
        ProfilerServiceDisplayName,
        ExtractorServiceDisplayName,


        IncrementSize,

        ApplicationVersion,

        QRDAFileSize,

        MeasureAuthoringEnvironment,
        //EPM Database connection string is holded in this.
        EPMUIStudioConnectionString,
        SSRSUsingWindowsIdentity,

        SQLCommandBatchSize,

        ReportProcessingExtractThreads,
        ReportProcessingTransferThreads,
        ReportProcessingExtractBatchSize,
        ReportProcessingTransferQueueLimit,
        ReportProcessingExtractRetryLimit,
        ReportProcessingTransferRetryLimit,
        ReportProcessingExtractTimeoutSeconds,
        ReportProcessingTransferTimeoutSeconds,
        ReportProcessingExtractOnError,

        PopulationSetsEngineProcessingThreads,
        PopulationSetsMultiThreadedProcessing,

        MicrostrategyWebServer,
        MicrostrategyIntelligenceServer,
        MicrostrategyIntelligenceServerPort,
        MicrostrategyProject,
        MicrostrategyUsingWindowsIdentity,
        MicrostrategyServicesURL,


        // Queue Processing
        MultiThreadedQueueProcessing,
        QueueProcessingThreads,
        QueueProcessingServiceSleepTime,

        //Indicators
        IndicatorMultiThreadedProcessing,

        // Profiler Version 2
        ProfilerVersion2,
        ProfilerThreads,
        ProfilerBatchSize,
        ProfilerMeasureThreads,
        QRDAProcessingThreads,
        ProfilerCommandTimeout,
        PowerBIServerURL,
        PowerBIServerRootFolder,
    }
    #endregion

    public class AppSettings
    {
        // SANMDComponent.ComponentName strings
        public const string ApplicationComponent = "SCA-APP";
        public const string ApplicationTraceComponent = "SCA-TL";
        public const string ApplicationReportLoggingComponent = "SCA-LR";
        public const string ApplicationInfectionControl = "SCA-INFCTL";

        public const string ApplicationAdvancedAnalytics = "CPM-ADVANLY";
        public const string ApplicationSunriseFinancialManager = "CPM-SFM";
        public const string ApplicationSunriseSurgicalCare = "CPM-SSC";

        /// <summary>
        /// Holds the connection string to the SANMDAppConfig Database
        /// </summary>
        private string ConnectionString { get; set; }


        /// <summary>
        /// Creates a new instance of the CpmSettings class
        /// </summary>
        /// <param name="connectionString">Connection String to the SANMDAppConfig Database</param>
        public AppSettings(string connectionString)
        {
            this.ConnectionString = connectionString;
        }


        /// <summary>
        /// Get a settings value out of the database via the passed in key
        /// </summary>
        /// <param name="key">ApplicationKey to access the settings value</param>
        /// <returns>Value of the setting</returns>
        public string GetSetting(string key, bool active = true)
        {
            string sql = $"SELECT AppSettingValue FROM SANMDAppConfig WHERE AppSettingKey=@Key {(active ? " AND IsActive=1" : string.Empty)}";
            return DataAccess.ExecuteScalar<string>(this.ConnectionString, sql, new List<SqlParameter>() { new SqlParameter("@Key", key) });
        }

        public string GetApplicationVersion()
        {
            string sql = "SELECT AppSettingValue FROM SANMDAppConfig WHERE AppSettingKey='ApplicationVersion'";
            return DataAccess.ExecuteScalar<string>(this.ConnectionString, sql, null);

        }

        /// <summary>
        /// Get a settings value out of the database via the passed in key(enumerated)
        /// </summary>
        /// <param name="key">ApplicationKey to access the settings value(enumerated)</param>
        /// <returns>Value of the setting</returns>
        public string GetSetting(AppConfigSettingKey key, bool active = true)
        {
            return GetSetting(key.ToString(), active);
        }

        /// <summary>
        /// Get a settings value out of the database via the passed in key(enumerated) returning value converted to type T
        /// </summary>
        /// <param name="key">ApplicationKey to access the settings value(enumerated)</param>
        /// <returns>Value of the setting converting to type T</returns>
        public T GetSetting<T>(AppConfigSettingKey key)
        {
            Type type = typeof(T);

            var value = GetSetting(key.ToString());
            try
            {
                return ConvertHelper.Convert<T>(value);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("AppSettings GetSetting<T> error casting value to type " + type.Name, ex);
            }
        }



        public bool GetSettingAsBoolean(AppConfigSettingKey key)
        {
            var val = GetSetting(key.ToString());
            return (string.IsNullOrEmpty(val) == false && (
                val.Equals("Y", StringComparison.CurrentCultureIgnoreCase) ||
                val.Equals("1") ||
                val.Equals("true", StringComparison.CurrentCultureIgnoreCase))) ? true : false;
        }

        public Dictionary<string, string> GetAllActiveSettings()
        {
            Dictionary<string, string> settings = null;
            string cmd = "SELECT DISTINCT AppsettingKey,AppsettingValue FROM SANMDAppConfig WHERE IsActive=1";
            DataTable dt = DataAccess.ExecuteSql(this.ConnectionString, cmd, null);
            settings = dt.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>(0), row => row.Field<string>(1));
            return settings;
        }

        /// <summary>
        /// Updates a settings value 
        /// </summary>
        /// <param name="key">enumerated key value</param>
        /// <param name="value">value to save with key</param>
        public void UpdateSetting(AppConfigSettingKey key, string value)
        {
            UpdateSetting(key.ToString(), value);
        }

        /// <summary>
        /// Updates a settings value 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateSetting(string key, string value)
        {
            string sql = "UPDATE SANMDAppConfig SET AppSettingValue = @Value WHERE AppSettingKey=@Key";
            string errors = string.Empty;
            DataAccess.ExecuteNonQuery(this.ConnectionString, System.Data.CommandType.Text, sql, new List<SqlParameter>() { new SqlParameter("@Value", value), new SqlParameter("@Key", key) }, out errors);
        }

        public int GetCommandTimeout()
        {
            int CommandTimeout;
            try
            {
                var value = GetSetting(AppConfigSettingKey.CommandTimeout);
                CommandTimeout = Convert.ToInt32(value);
            }
            catch (InvalidCastException)
            {
                CommandTimeout = 0;
            }
            catch (FormatException)
            {
                CommandTimeout = 0;
            }
            return CommandTimeout;
        }

        /// <summary>
        /// Method to determine if application component option is enabled
        /// </summary>
        /// <param name="ComponentName">Name of component in SANMDAppComponent</param>
        /// <returns>True is enabled (SANMDAppComponent.Active=1) otherwise false</returns>
        public bool IsApplicationComponentEnabled(string ComponentName)
        {
            if (string.IsNullOrEmpty(ComponentName))
                throw new ArgumentNullException("ComponentName", "AppSettings.GetApplicationComponent error - ComponentName is null");

            string sql = "SELECT Active,IntUpdtDtm FROM SANMDComponent WHERE ComponentName=@Name and IsApplicationFeature=1";
            string errors = string.Empty;
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@Name", ComponentName));
            var dt = DataAccess.ExecuteSql(this.ConnectionString, sql, parms);
            if (dt == null || dt.Rows.Count == 0)
                return false; // TODO SS throw new ApplicationException("AppSettings.GetApplicationComponent error - ComponentName " + ComponentName + " not valid or defined");

            var val = dt.Rows[0].Field<bool>("Active");
            return val;
        }

        public static string GetPasswordEncryptedConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (!connectionStringBuilder.IntegratedSecurity)
            {
                throw new InvalidOperationException("AppSettings error - Sql authentication not supported in application");
                //connectionStringBuilder.Password = Crypto.EncryptString(connectionStringBuilder.Password);
            }
            return connectionStringBuilder.ConnectionString;
        }

        public static string GetPasswordDecryptedConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (!connectionStringBuilder.IntegratedSecurity)
            {
                throw new InvalidOperationException("AppSettings error - Sql authentication not supported in application");
                //connectionStringBuilder.Password = Crypto.DecryptString(connectionStringBuilder.Password);
            }
            return connectionStringBuilder.ConnectionString;
        }



        /// <summary>
        /// Gets connection string for Metadata db from application config file
        /// This is supporting the previous implementations that rely on connection string pulling statically from web.config
        /// This approach will be obsolete in future releases
        /// </summary>
        /// <returns>connection string</returns>
        public static string GetSQLConnectionString()
        { return GetSQLConnectionString("SCAMD"); }

        /// <summary>
        /// Gets connection string by handle (name) from application config file
        /// This is supporting the previous implementations that rely on connection string pulling statically from web.config
        /// This approach will be obsolete in future releases
        /// </summary>
        /// <param name="name">Name of connection string in configuration file (.config)</param>
        /// <returns>connection string</returns>
        public static string GetSQLConnectionString(string name)
        {

            // Return empty string if the web.config does not exist Or 
            // If the connection string with name specified in the parameter is not defined in the config.
            // Returning empty string to the caller will raise an exception with the message
            // "Value cannot be null. Parameter name: connection". If this is not handled by the caller
            // the custom error page will be displayed with the above error message.
            if (ConfigurationManager.ConnectionStrings[name] == null)
                return string.Empty;

            // Return an empty string to the caller before passing the blank connection string value to
            // SqlConnectionStringBuilder constructor.
            // Returning empty string to the caller will raise an exception with the message
            // "Value cannot be null. Parameter name: connection". If this is not handled by the caller
            // the custom error page will be displayed with the above error message.
            if (string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[name].ConnectionString))
                return string.Empty;

            SqlConnectionStringBuilder connectionStringBuilder =
                    new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[name].ConnectionString);
            //if (!connectionStringBuilder.IntegratedSecurity)
            //    connectionStringBuilder.Password = Crypto.DecryptString(connectionStringBuilder.Password);

            return connectionStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Gets repository database connection string
        /// </summary>
        /// <param name="connectionString">Connection String to the metadata database to access AppConfig table</param>
        /// <returns>Acutecare db connection string</returns>
        public static string GetAcuteCareConnection(string metadataConnection)
        {
            return new AppSettings(metadataConnection).GetSetting(AppConfigSettingKey.AnalyticsRepositoryConnection);
        }
    }
}
