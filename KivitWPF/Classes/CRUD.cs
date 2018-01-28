using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceListCash.Classes
{
    /// <summary>
    /// Класс для работы с таблицами sql
    /// </summary>
    class CRUD
    {

        static SqlConnection mySqlConnection = new SqlConnection(DBConStrs.ConnectionString());

        /// <summary>
        /// Метод принимает запроси возвращает таблицу DataTable
        /// </summary>
        /// <param name="query">запрос Sql</param>
        /// <returns>DateTable</returns>
        public static  DataTable SelectToDateTable(string query) 
        {
            try
            {

                SqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = query;
                SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter();
                mySqlDataAdapter.SelectCommand = mySqlCommand;
                DataSet myDataSet = new DataSet();
                mySqlConnection.Open();
                mySqlDataAdapter.Fill(myDataSet, "table");
                mySqlConnection.Close();
                DataTable myDataTable = myDataSet.Tables["table"];
                return myDataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
                Log.Write(ex, "Ошибка  при выполнения запроса !!! ");
                ConnectionStringDataBase.Delete();
                Environment.Exit(0);
                throw ex;          
            }
            finally
            {
                    if (mySqlConnection != null && mySqlConnection.State != ConnectionState.Closed)
                    {
                        mySqlConnection.Dispose();
                    }
            }
        }

        /// <summary>
        /// Выполняет запрос SQL 
        /// </summary>
        /// <param name="query"> запрос SQL</param>
        public static void QuerySQL(string query)
        {
            try
            {

                SqlCommand sqlComm = new SqlCommand();
                sqlComm = mySqlConnection.CreateCommand();
                sqlComm.CommandText = query;
                mySqlConnection.Open();
                sqlComm.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
                Log.Write(ex, "Ошибка  при выполнения запроса !!!");
                ConnectionStringDataBase.Delete();
                Environment.Exit(0);
                throw ex;
            }
            finally
            {
                if (mySqlConnection != null && mySqlConnection.State != ConnectionState.Closed)
                {
                    mySqlConnection.Dispose();
                }
            }
        }
        
    }
}
