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
    public class UploadRepository: BaseRepository, IUploadRepository
    {
        private readonly string connectionString;

        public UploadRepository(IApplicationProperties applicationProperties) : base(applicationProperties)
        {
            connectionString = applicationProperties.ConnectionString;
        }

        public async Task SaveUploadedImages(int ImageId, int CategoryId, string ImageName, string UserEmail, string Format, string FolderName)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                var dynamicParameters = new DynamicParameters();
                if (ImageId > 0)
                {
                    dynamicParameters.Add("@p_intImageId", ImageId);
                }
                dynamicParameters.Add("@p_intCategoryId", CategoryId);
                dynamicParameters.Add("@p_chrImageName", ImageName);
                dynamicParameters.Add("@p_chrUserEmail", UserEmail);
                dynamicParameters.Add("@p_chrFormat", Format);
                dynamicParameters.Add("@p_chrFolderName", FolderName);
                if (ImageId == 0)
                    await sqlConnection.ExecuteAsync("usp_ImageAdd", dynamicParameters, commandType: CommandType.StoredProcedure);
                else
                    await sqlConnection.ExecuteAsync("usp_ImageUpdate", dynamicParameters, commandType: CommandType.StoredProcedure);
            }
        }

        public string GetUserFolderName(string UserEmail)
        {
            string folderName = "";
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.OpenAsync();
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@p_chrUserEmail", UserEmail);
                return folderName = sqlConnection.Execute("usp_ImageGetUserFoldername", dynamicParameters, commandType: CommandType.StoredProcedure).ToString();
            }


            ////var parameters = new SqlParameter[1];
            ////parameters[0] = new SqlParameter("@p_chrUserEmail", UserEmail);
            ////using (SqlDataReader dataReader = ExecuteReader("usp_ImageGetUserFoldername", parameters))
            ////{
            ////    while (dataReader.Read())
            ////    {
            ////        folderName = dataReader.GetValueOrDefault<string>("FolderName");
            ////    }
            ////}
            ////return folderName.ToString();
        }

    }
}
