using System;
using System.Collections.Generic;
using System.Text;
using eGallery.Contracts.Repositories;
using eGallery.Contracts.Models;
using eGallery.Infrastructure.BaseClass;
using eGallery.Infrastructure.BaseClass.ApplicationProperties;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace eGallery.Repository
{
    public class StatusRepository: BaseRepository, IStatusRepository
    {
        private readonly string connectionString;

        public StatusRepository(IApplicationProperties applicationProperties) : base(applicationProperties)
        {
            connectionString = applicationProperties.ConnectionString;
        }

        public async Task<StatusModel[]> StatusList()
        {
            //using (var sqlConnection = new SqlConnection(connectionString))
            //{
            //    await sqlConnection.OpenAsync();
            //    var query = await sqlConnection.QueryAsync<StatusModel[]>("usp_StatusList", null, commandType: CommandType.StoredProcedure);
            //    return query;
            //}
            List<StatusModel> status = new List<StatusModel>();
            StatusModel _status = null;
            var parameters = new SqlParameter[0];
            using (SqlDataReader dataReader = await this.ExecuteReader("usp_StatusList", parameters))
            {
                while (dataReader.Read())
                {
                    _status = new StatusModel();
                    _status.StatusId = dataReader.GetValueOrDefault<int>("StatusId");
                    _status.StatusName = dataReader.GetValueOrDefault<string>("StatusName");
                    _status.StatusTypeId = dataReader.GetValueOrDefault<int>("StatusTypeId");
                    status.Add(_status);
                }
            }
            return status.ToArray();
        }


        public async Task<StatusModel> StatusById(int StatusId)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@p_intStatusId", StatusId);
                var query = await sqlConnection.QuerySingleOrDefaultAsync<StatusModel>("usp_StatusById", dynamicParameters, commandType: CommandType.StoredProcedure);
                return query;
            }
        }

        public async Task SaveStatusData(string StatusName, int StatusId, int StatusType)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                var dynamicParameters = new DynamicParameters();
                if (StatusId > 0)
                {
                    dynamicParameters.Add("@p_intStatusId", StatusId);
                }

                dynamicParameters.Add("@p_chrStatusName", StatusName);
                dynamicParameters.Add("@p_intStatusTypeId", StatusType);

                if (StatusId == 0)
                    await sqlConnection.ExecuteAsync("usp_StatusAdd", dynamicParameters, commandType: CommandType.StoredProcedure);
                else
                    await sqlConnection.ExecuteAsync("usp_StatusUpdate", dynamicParameters, commandType: CommandType.StoredProcedure);
            }
        }

    }
}
