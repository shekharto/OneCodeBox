using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace CRUD.Transaction.CRUDApi.Core.Common
{
    public enum RetryableSqlErrors
    {
        SqlConnectionBroken = -1,
        SqlTimeout = -2,
        SqlOutOfMemory = 701,
        SqlOutOfLocks = 1204,
        SqlDeadlockVictim = 1205,
        SqlLockRequestTimeout = 1222,
        SqlTimeoutWaitingForMemoryResource = 8645,
        SqlLowMemoryCondition = 8651,
        SqlWordbreakerTimeout = 30053,
        SqlForceConnectionClose = 10054
    }

    public static class DataAccess
    {
        const int MAX_RETRY_COUNT = 3;
        public const string SQLCMDTOKENPHRASE = "[EXECSQLCMD]";
        private const string RETRYERRORPHRASE = "Attempt #{0}: {1}\r\n";
        private const string NONRETRYERRORPHRASE = "Non retryable SQL Exception:  {0}";

        /// <summary>
        /// Executes SQL and returns the first column of the first row returned
        /// </summary>
        /// <typeparam name="T">Type of Data being returned</typeparam>
        /// <param name="dbConnection">Db connection to use to execute the query</param>
        /// <param name="sql">sql to execute</param>
        /// <param name="sqlParams">sql parameters to use with the sql</param>
        /// <param name="retryLimit">sql statement will be retried this many times if encounters expected errors (RetryableSqlErrors)</param>
        /// <param name="commandTimeout">max length of time to allow commandd to execute (Default=0).  For non-zero commandtimeout, sqltimeout error will not retry command</param>
        /// <returns>First column's value (T) from first row</returns>
        public static T ExecuteScalar<T>(string dbConnection, string sql, List<SqlParameter> sqlParams, int retryLimit = MAX_RETRY_COUNT, int commandTimeout = 0)
        {
            int retryCount = 0;

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Sql to C# datatype mapping:  http://msdn.microsoft.com/en-us/library/cc716729(v=vs.110).aspx //
            // Implicit (allowed) c# cast http://msdn.microsoft.com/en-us/library/y5b434w4.aspx             //
            //////////////////////////////////////////////////////////////////////////////////////////////////

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.Clear();
                    cmd.Connection = conn;
                    cmd.CommandTimeout = commandTimeout;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    if (sqlParams != null)
                        cmd.Parameters.AddRange(sqlParams.ToArray());
                    DebugCmd(cmd);

                    while (retryCount <= retryLimit)
                    {
                        try
                        {
                            conn.Open();
                            var value = cmd.ExecuteScalar();
                            if (value == null || value is DBNull) return default(T); else return (T)value;

                        }
                        catch (SqlException exSQL)
                        {
                            if (!IsErrorRetryable(exSQL, cmd.CommandTimeout))
                            {
                                throw;
                            }
                            else
                            {
                                if (retryCount >= retryLimit)
                                {
                                    throw;
                                }
                                else
                                {
                                    retryCount++;
                                    System.Threading.Thread.Sleep(500);
                                }
                            }
                        }
                        finally
                        {
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }

                    } //## End While

                } //## End SqlCommand Using

            } //## End SqlConnection Using

            return default(T);
        }


        /// <summary>
        /// Executes a sql statement
        /// </summary>
        /// <param name="dbConnection">Db connection to use to execute the query</param>
        /// <param name="sql">sql to execute</param>
        /// <param name="sqlParams">sql parameters to use with the sql</param>
        /// <param name="cancelTpken">Cancel Token to monitor. If token is canceled it will abort the SqlCommand</param>
        /// <param name="rowsAffected">Number of rows affected when insert,update,delete</param>
        /// <param name="errors">Errors that occurred during execution of the sql</param>
        /// <returns>TRUE == Query Successful... FALSE == Query failed</returns>
        public static bool ExecuteNonQuery(string dbConnection, CommandType commandType, string sql, List<SqlParameter> sqlParams, CancellationToken cancelToken, out int rowsAffected, out string errors)
        {
            rowsAffected = int.MinValue;
            errors = string.Empty;
            int retryCount = 0;
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //## If cancel token is triggered, it will execute a .Cancel on the SqlCommand object
                    using (var cancelTokenCallBack = cancelToken.Register(cmd.Cancel))
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection = conn;
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = commandType;
                        cmd.CommandText = sql;

                        if (sqlParams != null)
                            cmd.Parameters.AddRange(sqlParams.ToArray());

                        DebugCmd(cmd);   //added to see values of command params for testing  NGP
                        while (retryCount < MAX_RETRY_COUNT)
                        {
                            try
                            {
                                conn.Open();
                                rowsAffected = cmd.ExecuteNonQuery();

                                if (retryCount > 0)
                                {
                                    errors += string.Format("Attempt: #{0}: Query Successful\r\n", (retryCount + 1));
                                }
                                return true;
                            }
                            catch (SqlException exSQL)
                            {//added for specific sql  exceptions do not want to retry if stored proc threw the error NGP
                                if (!IsErrorRetryable(exSQL, cmd.CommandTimeout))
                                {
                                    errors = ParseNonRetryError(exSQL, errors, ref retryCount);
                                    return false; //just get out don't retry
                                }
                                else
                                {
                                    errors = ParseRetryError(exSQL, errors, ref retryCount);
                                }

                            }
                            catch (Exception ex)
                            {
                                errors = ParseRetryError(ex, errors, ref retryCount);
                                if (cancelToken.IsCancellationRequested)
                                    return false;
                            }
                            finally
                            {
                                if (conn.State != ConnectionState.Closed)
                                    conn.Close();

                            }

                        } //## End While

                    } //## End CancelTokenCallback using

                } //## End SqlCommand Using

            } //## End SqlConnection Using

            return false;
        }

        /// <summary>
        /// Executes a sql statement
        /// </summary>
        /// <param name="dbConnection">Db connection to use to execute the query</param>
        /// <param name="sql">sql to execute</param>
        /// <param name="sqlParams">sql parameters to use with the sql</param>
        /// <param name="cancelTpken">Cancel Token to monitor. If token is canceled it will abort the SqlCommand</param>
        /// <param name="errors">Errors that occurred during execution of the sql</param>
        /// <returns>TRUE == Query Successful... FALSE == Query failed</returns>
        public static bool ExecuteNonQuery(string dbConnection, CommandType commandType, string sql, List<SqlParameter> sqlParams, CancellationToken cancelToken, out string errors)
        {
            int intRows;
            return ExecuteNonQuery(dbConnection, commandType, sql, sqlParams, cancelToken, out intRows, out errors);
        }


        /// <summary>
        /// Executes a sql statement
        /// </summary>
        /// <param name="dbConnection">Db connection to use to execute the query</param>
        /// <param name="sql">sql to execute</param>
        /// <param name="sqlParams">sql parameters to use with the sql</param>
        /// <param name="rowsAffected">Number of rows affected when insert,update,delete</param>
        /// <param name="errors">Errors that occurred during execution of the sql</param>
        /// <param name="commandTimeout">max length of time to allow commandd to execute (Default=0).  For non-zero commandtimeout, sqltimeout error will not retry command</param>
        /// <returns>TRUE == Query Successful... FALSE == Query failed</returns>
        public static bool ExecuteNonQuery(string dbConnection, CommandType commandType, string sql, List<SqlParameter> sqlParams, out int rowsAffected, out string errors, int commandTimeout = 0)
        {
            rowsAffected = int.MinValue;
            errors = string.Empty;
            int retryCount = 0;
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.Clear();
                    cmd.Connection = conn;
                    cmd.CommandTimeout = commandTimeout;
                    cmd.CommandType = commandType;
                    cmd.CommandText = sql;

                    if (sqlParams != null)
                        cmd.Parameters.AddRange(sqlParams.ToArray());

                    DebugCmd(cmd);   //added to see values of command params for testing  NGP
                    while (retryCount < MAX_RETRY_COUNT)
                    {
                        try
                        {
                            conn.Open();
                            rowsAffected = cmd.ExecuteNonQuery();

                            if (retryCount > 0)
                            {
                                // 22-Jul-15 jpm:  changed order of error accumulation to be latest first
                                errors = string.Format("Attempt: #{0}: Query Successful\r\n", (retryCount + 1)) + errors;
                            }
                            return true;
                        }
                        catch (SqlException exSQL)
                        {              //added for specific sql  exceptions do not want to retry if stored proc threw the error NGP
                            if (!IsErrorRetryable(exSQL, cmd.CommandTimeout))
                            {

                                errors = ParseNonRetryError(exSQL, errors, ref retryCount);
                                return false; //just get out don't retry
                            }
                            else
                            {
                                errors = ParseRetryError(exSQL, errors, ref retryCount);
                            }

                        }
                        catch (Exception ex)
                        {
                            errors = ParseRetryError(ex, errors, ref retryCount);
                        }
                        finally
                        {
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }

                    } //## End While

                } //## End SqlCommand Using

            } //## End SqlConnection Using

            return false;
        }

        /// <summary>
        /// Executes a sql statement
        /// </summary>
        /// <param name="dbConnection">Db connection to use to execute the query</param>
        /// <param name="sql">sql to execute</param>
        /// <param name="sqlParams">sql parameters to use with the sql</param>
        /// <param name="errors">Errors that occurred during execution of the sql</param>
        /// <returns>TRUE == Query Successful... FALSE == Query failed</returns>
        public static bool ExecuteNonQuery(string dbConnection, CommandType commandType, string sql, List<SqlParameter> sqlParams, out string errors)
        {
            int intRows;
            return ExecuteNonQuery(dbConnection, commandType, sql, sqlParams, out intRows, out errors);
        }




        /// <summary>
        /// Executes a query agaisnt the dbConnection passed in. Has support to cancel query if cancelToken is triggered.
        /// </summary>
        /// <param name="dbConnection">Database connection string</param>
        /// <param name="sql">SQL to execute</param>
        /// <param name="sqlParams">SQL Params</param>
        /// <param name="cancelToken"></param>
        /// <param name="maxRetryCount"></param>
        /// <returns>DataTable result from SQL statement</returns>
        public static DataTable ExecuteSql(string dbConnection, string sql, List<SqlParameter> sqlParams, CancellationToken cancelToken, int maxRetryCount)
        {
            if (string.IsNullOrEmpty(dbConnection))
                throw new ArgumentNullException("dbConnection", "DataAccess error - ConnectionString is null or undefined");

            int retryCount = 0;
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //## If cancel token is triggered, it will execute a .Cancel on the SqlCommand object
                    using (var cancelTokenCallBack = cancelToken.Register(cmd.Cancel))
                    {

                        while (true)
                        {
                            try
                            {
                                cmd.Parameters.Clear();
                                cmd.Connection = conn;
                                cmd.CommandTimeout = 0;
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = sql;

                                if (sqlParams != null)
                                    cmd.Parameters.AddRange(sqlParams.ToArray());

                                conn.Open();
                                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                {
                                    using (DataTable dt = new DataTable())
                                    {
                                        da.Fill(dt);
                                        return dt;
                                    }
                                }
                            }
                            catch
                            {
                                if (retryCount >= maxRetryCount)
                                {
                                    throw;
                                }
                                else
                                {
                                    retryCount++;
                                    System.Threading.Thread.Sleep(500);
                                }
                            }
                            finally
                            {
                                if (conn.State != ConnectionState.Closed)
                                    conn.Close();
                            }
                        } //## End While

                    } //## End cancel callback token

                } //## End SqlCommand Using

            } //## End SqlConnection Using

        }




        /// <summary>
        /// Executes SQL and returns a datatable
        /// </summary>
        /// <param name="dbConnection">Db connection to use to execute the query</param>
        /// <param name="sql">sql to execute</param>
        /// <param name="sqlParams">sql parameters to use with the sql</param>
        /// <param name="commandTimeout">max length of time to allow commandd to execute (Default=0).  For non-zero commandtimeout, sqltimeout error will not retry command</param>
        /// <returns>DataTable Results</returns>
        public static DataTable ExecuteSql(string dbConnection, string sql, List<SqlParameter> sqlParams, int commandTimeout = 0)
        {
            int retryCount = 0;
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    while (retryCount < MAX_RETRY_COUNT)
                    {
                        try
                        {
                            cmd.Parameters.Clear();
                            cmd.Connection = conn;
                            cmd.CommandTimeout = commandTimeout;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = sql;

                            if (sqlParams != null)
                                cmd.Parameters.AddRange(sqlParams.ToArray());

                            conn.Open();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                using (DataTable dt = new DataTable())
                                {
                                    da.Fill(dt);
                                    return dt;
                                }
                            }
                        }
                        catch (SqlException exSQL)
                        {
                            if (!IsErrorRetryable(exSQL, cmd.CommandTimeout))
                            {
                                throw;
                            }
                            else
                            {
                                if (retryCount >= MAX_RETRY_COUNT)
                                {
                                    throw;
                                }
                                else
                                {
                                    retryCount++;
                                    System.Threading.Thread.Sleep(500);
                                }
                            }
                        }
                        finally
                        {
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                    } //## End While

                } //## End SqlCommand Using

            } //## End SqlConnection Using

            return null;
        }

        //added to see command paramter values during debug
        public static string DebugCmd(SqlCommand cmd)
        {
            StringBuilder sb = new StringBuilder(1000);
            try
            {
                sb.Append(Environment.NewLine);
                sb.Append(SQLCMDTOKENPHRASE);
                sb.Append(cmd.CommandText);
                sb.Append("		");
                string strSep = "";

                foreach (SqlParameter p in cmd.Parameters)
                {
                    sb.Append(strSep);
                    sb.Append(DebugCmdParameterValue(p));
                    strSep = ",";
                }
            }
            catch { }
            sb.Append(Environment.NewLine);
            var strcmd = sb.ToString();
            WriteDebug("DataAccess", "DebugCmd", strcmd);
            return strcmd;
        }

        public static string DebugCmdParameterValue(SqlParameter p)
        {
            switch (p.SqlDbType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Binary:
                case SqlDbType.Bit:
                case SqlDbType.Decimal:
                case SqlDbType.Float:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.Real:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.TinyInt:
                case SqlDbType.VarBinary:
                    return ($"{p.ParameterName} = {p.Value}");
                default:
                    return ($"{p.ParameterName} = '{p.Value}'");
            }
        }
        public static string DebugCmdDeclareParameter(SqlParameter p)
        {
            switch (p.SqlDbType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Binary:
                case SqlDbType.Bit:
                case SqlDbType.Decimal:
                case SqlDbType.Float:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.Real:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.TinyInt:
                    return $"DECLARE {p.ParameterName} {p.SqlDbType};";
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                case SqlDbType.NChar:
                    return $"DECLARE {p.ParameterName} {p.SqlDbType}(MAX);";
                default:
                    return $"DECLARE {p.ParameterName} {p.SqlDbType};";
            }
        }

        public static void WriteDebug(string strObject, string strMethod, string strFormattedMessage, params object[] objMessageArgs)
        {

            if (strObject.IndexOf("{0}") >= 0)
            {
                //writes command an param values to Output window  in debug mode
                System.Diagnostics.Trace.WriteLine(string.Format("{0}", string.Format(strFormattedMessage, objMessageArgs)));
                return;
            }
            if (objMessageArgs != null && objMessageArgs.Length > 0)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("{0}.{1}:{2}",
                    strObject, strMethod,
                    string.Format(strFormattedMessage, objMessageArgs)));
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("{0}.{1}:{2}",
                    strObject, strMethod, strFormattedMessage));
            }
        }


        /// <summary>
        /// Disables all Non-clustered indexes that belong to a database table
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="databaseTableName">Database TableName</param>
        public static void DisableTableIndexes(string connectionString, string databaseTableName)
        {
            string errors = string.Empty;
            string sql;
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            sql = @"
                DECLARE @Indexes TABLE
                (
                    Num       int identity(1,1) primary key clustered,
                    TableName nvarchar(255),
                    IndexName nvarchar(255)
                )

                INSERT INTO @Indexes
                (
                    TableName,
                    IndexName
                )
                SELECT  sys.objects.name tableName,
                        sys.indexes.name indexName
                FROM    sys.indexes
                        JOIN sys.objects ON sys.indexes.object_id = sys.objects.object_id
                WHERE   sys.indexes.type_desc = 'NONCLUSTERED'
                        AND sys.objects.type_desc = 'USER_TABLE'
		                AND sys.objects.name = @TableToFind

                DECLARE @Max INT
                SET @Max = @@ROWCOUNT

                DECLARE @idx INT
                SET @idx = 1

                DECLARE @TblName NVARCHAR(255), @IdxName NVARCHAR(255)
                DECLARE @SQL NVARCHAR(MAX)

                WHILE @idx <= @Max
                BEGIN
                    SELECT @TblName = TableName, @IdxName = IndexName FROM @Indexes WHERE Num = @idx
                    SELECT @SQL = N'ALTER INDEX ' + @IdxName + N' ON ' + @TblName + ' DISABLE;'
	                EXEC sp_sqlexec @SQL    
                    SET @idx = @idx + 1
                END
            ";
            sqlParams.Add(new SqlParameter("@TableToFind", databaseTableName));
            DataAccess.ExecuteNonQuery(connectionString, CommandType.Text, sql, sqlParams, out errors);
        }


        /// <summary>
        /// Rebuilds all the indexes that belong to a database table
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="databaseTableName">Database Table to rebuild indexes on</param>
        public static void RebuildTableIndexes(string connectionString, string databaseTableName)
        {
            string errors = string.Empty;
            string sql;
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            sql = @"
                DECLARE @Indexes TABLE
                (
                    Num       int identity(1,1) primary key clustered,
                    TableName nvarchar(255),
                    IndexName nvarchar(255)
                )

                INSERT INTO @Indexes
                (
                    TableName,
                    IndexName
                )
                SELECT  sys.objects.name tableName,
                        sys.indexes.name indexName
                FROM    sys.indexes
                        JOIN sys.objects ON sys.indexes.object_id = sys.objects.object_id
                WHERE   sys.indexes.type_desc = 'NONCLUSTERED'
                        AND sys.objects.type_desc = 'USER_TABLE'
		                AND sys.objects.name = @TableToFind

                DECLARE @Max INT
                SET @Max = @@ROWCOUNT

                DECLARE @idx INT
                SET @idx = 1

                DECLARE @TblName NVARCHAR(255), @IdxName NVARCHAR(255)
                DECLARE @SQL NVARCHAR(MAX)

                WHILE @idx <= @Max
                BEGIN
                    SELECT @TblName = TableName, @IdxName = IndexName FROM @Indexes WHERE Num = @idx
	                SELECT @SQL = N'ALTER INDEX ' + @IdxName + N' ON ' + @TblName + ' REBUILD WITH (ONLINE = ON);'
	                EXEC sp_sqlexec @SQL    
                    SET @idx = @idx + 1
                END
            ";
            sqlParams.Add(new SqlParameter("@TableToFind", databaseTableName));
            DataAccess.ExecuteNonQuery(connectionString, CommandType.Text, sql, sqlParams, out errors);
        }

        /// <summary>
        /// CR 1391454 New method to execute Dataset with retry logic
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="cmd"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static DataSet ExecuteGetDataSet(string dbConnection, SqlCommand cmd, out string errors)
        {
            errors = string.Empty;
            int retryCount = 0;

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {

                DebugCmd(cmd);
                while (retryCount < MAX_RETRY_COUNT)
                {
                    try
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                return ds;
                            }
                        }
                    }
                    catch (SqlException exSQL)
                    {              //added for specific sql  exceptions do not want to retry if stored proc threw the error NGP

                        if (!IsErrorRetryable(exSQL, cmd.CommandTimeout))
                        {
                            if (retryCount > MAX_RETRY_COUNT)
                            {
                                throw;
                            }
                            else
                            {
                                string error = string.Format("Attempt #{0}: {1}", (retryCount + 1), exSQL.Message);
                                if (exSQL.InnerException != null && exSQL.InnerException.Message != null)
                                {
                                    error += " INNER: " + exSQL.InnerException.Message;
                                }
                                errors += error + "\r\n";
                                retryCount++;
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                        else
                            throw;

                    }
                    catch
                    {
                        if (retryCount > MAX_RETRY_COUNT)
                        {
                            throw;
                        }
                        else
                        {
                            retryCount++;
                            System.Threading.Thread.Sleep(500);
                        }
                    }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                            conn.Close();
                    }
                } //## End While


            } //## End SqlConnection Using

            return null;
        }

        /// <summary>
        /// Returns connection string replacing database name with the TargetDbName
        /// </summary>
        /// <param name="MetadataConnectionString">Connection string to Metadata connection</param>
        /// <param name="TargetDbName">Name of AcuteCare database</param>
        /// <returns></returns>
        public static string GetTargetDbConnectionString(string ConnectionString, string TargetDbName)
        {
            SqlConnectionStringBuilder bldr = new SqlConnectionStringBuilder(ConnectionString);
            bldr.InitialCatalog = TargetDbName;
            return bldr.ConnectionString;
        }

        private static bool IsErrorRetryable(SqlException exSql, int commandTimeout = 0)
        {

            RetryableSqlErrors retryErr;
            if (!Enum.IsDefined(typeof(RetryableSqlErrors), exSql.Number))
                return false;
            if (Enum.TryParse<RetryableSqlErrors>(exSql.Number.ToString(), out retryErr) == false)
                return false;
            // if Timeout occurred and commandtimeout is non-zero, do not retry
            if (retryErr == RetryableSqlErrors.SqlTimeout && commandTimeout != 0)
                return false;
            // error is Enum so retry
            return true;

        }

        #region ExceptionHandling

        private static string ParseRetryError(Exception ex, string errors, ref int retryCount)
        {
            //string message = string.Format(RETRYERRORPHRASE, (retryCount + 1), CPMLogger.ExceptionMessageToString(ex));
            //retryCount++;
            string message = string.Empty;
            return message;
        }
        private static string ParseNonRetryError(Exception ex, string errors, ref int retryCount)
        {
            //string message = string.Format(NONRETRYERRORPHRASE, CPMLogger.ExceptionMessageToString(ex));
            //retryCount++;
            string message = string.Empty;
            return message;
        }

        #endregion

    } //## End Class
}
