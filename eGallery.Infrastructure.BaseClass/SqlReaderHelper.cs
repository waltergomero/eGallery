using System;
using System.Data.SqlClient;

namespace eGallery.Infrastructure.BaseClass
{
    public static class SqlReaderHelper
    {
        /// <summary>
        /// It is a Generic Method to get and check the dbnull value for the sql datareader fields. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this SqlDataReader dataReader, string fieldName)
        {
            if (dataReader.ColumnExists(fieldName))
            {
                if (dataReader[fieldName] == DBNull.Value)
                    return default(T);
                return (T)dataReader[fieldName];
            }
            else
                return default(T);

        }
        public static bool ColumnExists(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
