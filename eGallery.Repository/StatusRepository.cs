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
                    _status.StatusConstant = dataReader.GetValueOrDefault<string>("StatusConstant");
                    status.Add(_status);
                }
            }
            return status.ToArray();
        }
    }
}
