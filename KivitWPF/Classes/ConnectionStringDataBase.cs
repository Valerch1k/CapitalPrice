using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceListCash.Classes
{
    /// <summary>
    /// Содержит методы для изменения , удаления и возврата строки подключения 
    /// </summary> 
    class ConnectionStringDataBase
    {
        private static object sync = new object();

        /// <summary>
        ///  Перезаписывает строку подключения к базе данных
        /// </summary>
        /// <param name="fullText"> строка подключения </param>
        public static void Write(string fullText)
        {
            try
            {
                // Путь .\\Resources
                string pathToLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PriceListCashConfig");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("ConnectionDataBase.txt", AppDomain.CurrentDomain.FriendlyName));
                lock (sync)
                {
                    File.WriteAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK);
                Log.Write(ex, "Ошибка  при записи строки подключения ");
                // Перехватываем все и ничего не делаем
            }
        }
        /// <summary>
        /// Возвращает строку подключения 
        /// </summary>
        /// <returns> строка подключения</returns>
        public static string Read()
        {
            return File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\PriceListCashConfig\\ConnectionDataBase.txt");
        }

        /// <summary>
        /// Удаляет строку подключения
        /// </summary>
        public static void Delete() 
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\PriceListCashConfig\\ConnectionDataBase.txt");
        }

        /// <summary>
        /// метод вызываюзщий диалоговое окно подключения к базе данных и изменяет строку подключения 
        /// </summary>
        public static void ChangeConnect()
        {
            DataConnectionDialog dcd = new DataConnectionDialog();
            DataConnectionConfiguration dcs = new DataConnectionConfiguration(null);
            dcs.LoadConfiguration(dcd);

            if (DataConnectionDialog.Show(dcd) == System.Windows.Forms.DialogResult.OK)
            {
                // load tables
                using (SqlConnection connection = new SqlConnection(dcd.ConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM sys.Tables", connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.HasRows);
                        }
                    }
                }
            }
            else
            {
                Environment.Exit(0);
            }
            dcs.SaveConfiguration(dcd);
            Write(dcd.ConnectionString);     
        }
    }
}
