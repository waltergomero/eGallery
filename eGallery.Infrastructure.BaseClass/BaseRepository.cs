using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using eGallery.Infrastructure.BaseClass.ApplicationProperties;


namespace eGallery.Infrastructure.BaseClass
{
    public class BaseRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository" /> class.
        /// </summary>
        /// <param name="applicationProperties">The application properties.</param>
        public BaseRepository(IApplicationProperties applicationProperties)
        {
            this.ApplicationProperties = applicationProperties;
        }

        /// <summary>
        /// Gets or sets the application properties.
        /// </summary>
        /// <value>
        /// The application properties.
        /// </value>
        protected IApplicationProperties ApplicationProperties { get; set; }

        /// <summary>
        /// Check the value is null or not.
        /// </summary>
        /// <param name="value">The original.</param>
        /// <returns>System Object.</returns>
        protected object CastNullToDBNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }

            return value;
        }

        #region Connection

        /// <summary>
        /// Gets the connection strings Parameters to a Command.
        /// This method will assign a value of connection string.
        /// Retrieve connection string from the web.config. 
        /// </summary>
        public string AppConnectionString
        {
            get
            {
                return this.ApplicationProperties.ConnectionString;
            }
        }

        #endregion

        #region prepare Command
        /// <summary>
        /// This method is used to attach array of Parameters to a Command.
        /// This method will assign a value of Null to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of Parameters to be added to command</param>
        private void AttachParameters(SqlCommand command, IEnumerable<SqlParameter> commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (commandParameters == null)
            {
                return;
            }

            foreach (var p in commandParameters.Where(p => p != null))
            {
                // Check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput ||
                     p.Direction == ParameterDirection.Input) &&
                    (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// Joins a first name and a last name together into a single string.
        /// </summary>
        /// <param name="command">The command parameter.</param>
        /// <param name="connection">The connection parameter.</param>
        /// <param name="transaction">The transaction parameter.</param>
        /// <param name="commandType">The commandType parameter.</param>
        /// <param name="commandText">The commandText parameter.</param>
        /// <param name="commandParameters">The commandParameters for data values.</param>
        /// <returns>The joined names.</returns>
        private async Task<bool> PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, IEnumerable<SqlParameter> commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            var mustCloseConnection = false;
            //// If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                await connection.OpenAsync().ConfigureAwait(false);
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                }

                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;
            command.CommandTimeout = 0;
            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }

            return mustCloseConnection;
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Execute a stored procedure via a Command (that returns no result set) against the database.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync("PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <param name="commandParameters">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public async Task<int> ExecuteNonQuery(string storedprocedureName, params SqlParameter[] commandParameters)
        {
            return await this.ExecuteNonQuery(this.AppConnectionString, storedprocedureName, commandParameters);
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns no result set) against the database.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync("PublishOrders");
        /// </remarks>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public async Task<int> ExecuteNonQuery(string storedprocedureName)
        {
            return await this.ExecuteNonQuery(this.AppConnectionString, storedprocedureName);
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns no result set) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <param name="commandParameters">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public async Task<int> ExecuteNonQuery(string connectionString, string storedprocedureName, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                //// Call the overload that takes a connection in place of the connection string
                return await this.ExecuteNonQuery(connection, CommandType.StoredProcedure, storedprocedureName, commandParameters).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns no result set) against the specified 
        /// Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <param name="commandParameters">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public async Task<int> ExecuteNonQuery(SqlTransaction transaction, string storedprocedureName, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            await this.PrepareCommand(cmd, transaction.Connection, transaction, CommandType.StoredProcedure, storedprocedureName, commandParameters).ConfigureAwait(false);

            // Finally, execute the command
            var retval = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a Command (that returns no result set and takes no parameters) against the provided Transaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="storedprocedureName">The CommandType (stored procedure, text, etc.)</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public async Task<int> ExecuteNonQuery(SqlTransaction transaction, string storedprocedureName)
        {
            return await this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, storedprocedureName);
        }

        /// <summary>
        /// Execute a Command (that returns no result set) against the specified Connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(conn, CommandType.StoredProcedure, "PublishOrders", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="connection">A valid Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        private async Task<int> ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            var mustCloseConnection = await this.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters).ConfigureAwait(false);

            // Finally, execute the command
            var retval = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }

            return retval;
        }

        /// <summary>
        /// Execute a Command (that returns no result set and takes no parameters) against the database specified in 
        /// the connection string
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public Task<int> ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteNonQuery(connectionString, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a Command (that returns no result set) against the database specified in the connection string 
        /// using the provided parameters
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(connString, CommandType.StoredProcedure, "PublishOrders", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        public async Task<int> ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                //// Call the overload that takes a connection in place of the connection string
                return await this.ExecuteNonQuery(connection, commandType, commandText, commandParameters).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute a Command (that returns no result set and takes no parameters) against the provided Transaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        private Task<int> ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a Command (that returns no result set) against the specified Transaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer result = ExecuteNonQueryAsync(trans, CommandType.StoredProcedure, "GetOrders", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>An integer representing the number of rows affected by the command</returns>
        private async Task<int> ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            await this.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters).ConfigureAwait(false);

            // Finally, execute the command
            var retval = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        #endregion ExecuteNonQueryAsync

        #region ExecuteReader

        /// <summary>
        /// Connection is owned and managed by Helper/caller
        /// </summary>
        /// <returns>The Connection name value</returns>
        private enum SqlConnectionOwnership
        {
            /// <summary>Connection is owned and managed by Helper</summary>            
            Internal,

            /// <summary>Connection is owned and managed by the caller</summary>            
            External
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns a result set) against the database.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="storedprocedureName">A valid connection string for a Connection</param>
        /// <param name="commandParameters">The name of the stored procedure</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public async Task<SqlDataReader> ExecuteReader(string storedprocedureName, params SqlParameter[] commandParameters)
        {
            return await this.ExecuteReader(this.AppConnectionString, storedprocedureName, commandParameters);
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns a result set) against the database.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="storedprocedureName">A valid connection string for a Connection</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public async Task<SqlDataReader> ExecuteReader(string storedprocedureName)
        {
            return await this.ExecuteReader(this.AppConnectionString, storedprocedureName);
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns a result set) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <param name="commandParameters">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public async Task<SqlDataReader> ExecuteReader(string connectionString, string storedprocedureName, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            ////using (var connection = new SqlConnection(connectionString))
            {
                var connection = new SqlConnection(connectionString);
                await connection.OpenAsync().ConfigureAwait(false);

                //// Call the overload that takes a connection in place of the connection string
                return await this.ExecuteReader(connection, CommandType.StoredProcedure, storedprocedureName, commandParameters).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns a result set) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public async Task<SqlDataReader> ExecuteReader(string connectionString, string storedprocedureName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            ////using (var connection = new SqlConnection(connectionString))
            {
                var connection = new SqlConnection(connectionString);
                await connection.OpenAsync().ConfigureAwait(false);

                //// Call the overload that takes a connection in place of the connection string
                return await this.ExecuteReader(connection, CommandType.StoredProcedure, storedprocedureName, null).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns a result set) against the specified
        /// Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <param name="commandParameters">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public Task<SqlDataReader> ExecuteReader(SqlTransaction transaction, string storedprocedureName, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            return this.ExecuteReader(transaction, CommandType.StoredProcedure, storedprocedureName, commandParameters);
        }

        /// <summary>
        /// Execute a Command (that returns a result set and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataReader reader = ExecuteReaderAsync(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid connection string for a Connection</param>
        /// <param name="storedprocedureName">The CommandType (stored procedure, text, etc.)</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public async Task<SqlDataReader> ExecuteReader(SqlTransaction transaction, string storedprocedureName)
        {
            return await this.ExecuteReader(transaction, CommandType.StoredProcedure, storedprocedureName);
        }

        /// <summary>
        /// Execute a Command (that returns a result set and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataReader reader = ExecuteReaderAsync(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public Task<SqlDataReader> ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteReader(connectionString, commandType, commandText, null);
        }

        /// <summary>
        /// Create and prepare a Command, and call ExecuteReaderAsync with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">A valid Connection, on which to execute this command</param>
        /// <param name="transaction">A valid Transaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by Helper</param>
        /// <returns>DataReader containing the results of the command</returns>
        private async Task<SqlDataReader> ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, IEnumerable<SqlParameter> commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            var mustCloseConnection = false;
            //// Create a command and prepare it for execution
            var cmd = new SqlCommand();
            try
            {
                mustCloseConnection = await this.PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters).ConfigureAwait(false);

                // Create a reader
                SqlDataReader dataReader;
                cmd.CommandTimeout = 0;
                // Call ExecuteReader with the appropriate CommandBehavior
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                }
                else
                {
                    dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);
                }

                // Detach the SqlParameters from the command object, so they can be used again.
                // HACK: There is a problem here, the output parameter values are fletched 
                // when the reader is closed, so if the parameters are detached from the command
                // then the SqlReader can´t set its values. 
                // When this happen, the parameters can´t be used again in other command.
                var canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                    {
                        canClear = false;
                    }
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                {
                    connection.Close();
                }

                throw;
            }
        }

        /// <summary>
        /// Execute a Command (that returns a result set) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        private async Task<SqlDataReader> ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                await connection.OpenAsync().ConfigureAwait(false);

                // Call the private overload that takes an internally owned connection in place of the connection string
                return await this.ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal).ConfigureAwait(false);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves
                if (connection != null)
                {
                    connection.Close();
                }

                throw;
            }
        }

        /// <summary>
        /// Execute a Command (that returns a result set and takes no parameters) against the provided Connection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public Task<SqlDataReader> ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteReader(connection, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a Command (that returns a result set) against the specified Connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="connection">A valid Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        private Task<SqlDataReader> ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return this.ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
        }

        /// <summary>
        /// Execute a Command (that returns a result set and takes no parameters) against the provided Transaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataReader reader = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        public Task<SqlDataReader> ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteReader(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a Command (that returns a result set) against the specified Transaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   DataReader reader = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>A DataReader containing the result set generated by the command</returns>
        private Task<SqlDataReader> ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            // Pass through to private overload, indicating that the connection is owned by the caller
            return this.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set and takes no parameters) against the database
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalar("GetOrderCount",parameter);
        /// </remarks>
        /// <param name="storedprocedureName">A valid connection string for a Connection</param>
        /// <param name="commandParameters">The CommandType (stored procedure, text, etc.)</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public async Task<object> ExecuteScalar(string storedprocedureName, params SqlParameter[] commandParameters)
        {
            return await this.ExecuteScalar(this.AppConnectionString, storedprocedureName, commandParameters);
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set and takes no parameters) against the database
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalar("GetOrderCount");
        /// </remarks>
        /// <param name="storedprocedureName">A valid connection string for a Connection</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public async Task<object> ExecuteScalar(string storedprocedureName)
        {
            return await this.ExecuteScalar(this.AppConnectionString, storedprocedureName);
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set and takes no parameters) against the database
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalar(connectionString,"GetOrderCount",parameter);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="storedprocedureName">A valid stored procedure name.</param>
        /// <param name="commandParameters">The input parameter list.</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public Task<object> ExecuteScalar(string connectionString, string storedprocedureName, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            return this.ExecuteScalar(connectionString, CommandType.StoredProcedure, storedprocedureName, commandParameters);
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="storedprocedureName">The CommandType (stored procedure, text, etc.)</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public Task<object> ExecuteScalar(string connectionString, string storedprocedureName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            return this.ExecuteScalar(connectionString, CommandType.StoredProcedure, storedprocedureName);
        }

        /// <summary>
        /// Execute a stored procedure via a Command (that returns a 1x1 result set) against the specified
        /// Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalarAsync(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="storedprocedureName">The name of the stored procedure</param>
        /// <param name="commandParameters">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public Task<object> ExecuteScalar(SqlTransaction transaction, string storedprocedureName, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            if (string.IsNullOrEmpty(storedprocedureName))
            {
                throw new ArgumentNullException("storedprocedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((commandParameters != null) && (commandParameters.Length > 0))
            {
                // Call the overload that takes an array of SqlParameters
                return this.ExecuteScalar(transaction, CommandType.StoredProcedure, storedprocedureName, commandParameters);
            }
            //// Otherwise we can just call the SP without params
            return this.ExecuteScalar(transaction, CommandType.StoredProcedure, storedprocedureName);
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set) against the specified Connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalarAsync(conn, CommandType.StoredProcedure, "GetOrderCount", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="connection">A valid Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        private async Task<object> ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();

            var mustCloseConnection = await this.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters).ConfigureAwait(false);

            // Execute the command & return the results
            var retval = await cmd.ExecuteScalarAsync().ConfigureAwait(false);

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if (mustCloseConnection)
            {
                connection.Close();
            }

            return retval;
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        private Task<object> ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteScalar(connectionString, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalarAsync(connString, CommandType.StoredProcedure, "GetOrderCount", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        private async Task<object> ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            //// Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                // Call the overload that takes a connection in place of the connection string
                return await this.ExecuteScalar(connection, commandType, commandText, commandParameters).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result and takes no parameters) against the provided Transaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalarAsync(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        private Task<object> ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteScalar(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 record) against the specified Transaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalarAsync(trans, CommandType.StoredProcedure, "GetOrderCount", new Parameter("@product id", 24));
        /// </remarks>
        /// <param name="transaction">A valid Transaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of Parameters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public async Task<object> ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            //// Create a command and prepare it for execution
            var cmd = new SqlCommand();
            await this.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters).ConfigureAwait(false);

            //// Execute the command & return the results
            var retval = await cmd.ExecuteScalarAsync().ConfigureAwait(false);

            //// Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a Command (that returns a 1x1 result set and takes no parameters) against the provided Connection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  integer orderCount = (integer)ExecuteScalarAsync(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">A valid Connection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or command</param>
        /// <returns>An object containing the value in the 1x1 result set generated by the command</returns>
        public Task<object> ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return this.ExecuteScalar(connection, commandType, commandText, null);
        }

        #endregion ExecuteScalarAsync       

 
    }
}
