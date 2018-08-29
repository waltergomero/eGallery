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
    public class CategoryRepository: BaseRepository, ICategoryRepository
    {
        private readonly string connectionString;

        public CategoryRepository(IApplicationProperties applicationProperties) : base(applicationProperties)
        {
            connectionString = applicationProperties.ConnectionString;
        }

        public async Task<CategoryModel[]> CategoryList()
        {
            List<CategoryModel> category = new List<CategoryModel>();
            CategoryModel _category = null;
            var parameters = new SqlParameter[0];
            using (SqlDataReader dataReader = await this.ExecuteReader("usp_CategoryList", parameters))
            {
                while (dataReader.Read())
                {
                    _category = new CategoryModel();
                    _category.CategoryId = dataReader.GetValueOrDefault<int>("CategoryId");
                    _category.CategoryName = dataReader.GetValueOrDefault<string>("CategoryName");
                    _category.Description = dataReader.GetValueOrDefault<string>("Description");
                    _category.ParentCategoryId = dataReader.GetValueOrDefault<int>("ParentCategoryId");
                    _category.CategoryImage = dataReader.GetValueOrDefault<string>("CategoryImage");
                    _category.StatusId = dataReader.GetValueOrDefault<int>("StatusId");
                    category.Add(_category);
                }
            }
            return category.ToArray();
        }


        public async Task<CategoryModel> CategoryById(int CategoryId)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@p_intCategoryId", CategoryId);
                var query = await sqlConnection.QuerySingleOrDefaultAsync<CategoryModel>("usp_CategoryById", dynamicParameters, commandType: CommandType.StoredProcedure);
                return query;
            }
        }

        public async Task SaveCategoryData(int CategoryId, string CategoryName, string Description, string CategoryImage, int ParentCategoryId, int StatusId)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                var dynamicParameters = new DynamicParameters();
                if (StatusId > 0)
                {
                    dynamicParameters.Add("@p_intCategoryId", CategoryId);
                }

                dynamicParameters.Add("@p_chrCategoryName", CategoryName);
                dynamicParameters.Add("@p_chrDescription", Description);
                dynamicParameters.Add("@p_chrCategoryImage", CategoryImage);
                dynamicParameters.Add("@p_intParentCategoryId", ParentCategoryId);
                dynamicParameters.Add("@p_intStatusId", StatusId);

                if (StatusId == 0)
                    await sqlConnection.ExecuteAsync("usp_CategoryAdd", dynamicParameters, commandType: CommandType.StoredProcedure);
                else
                    await sqlConnection.ExecuteAsync("usp_CategoryUpdate", dynamicParameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
