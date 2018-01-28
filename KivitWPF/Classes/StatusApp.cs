using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Printing;
using System.Windows;
using System.IO;
using System.Data.SqlClient;

namespace PriceListCash.Classes
{
    /// <summary>
    ///  Проверяет приложение на работоспособность
    /// </summary>
    class StatusApp
    {
        /// <summary>
        ///  Проверка подключения к БД
        /// </summary>
        /// <returns></returns>
        public static bool CheckConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStringDataBase.Read()))
                {
                    int dircount = 0;
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT count(*) FROM sys.Tables", connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dircount = Convert.ToInt32(reader[0]);
                        }
                    }
                    if (dircount > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет статус принтера  Online/Offline
        /// </summary>
        /// <returns></returns>
        public static bool PrinterStatusBool()
        {
            //ConfigTxt configTxt = new ConfigTxt();
            //ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            //ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            //bool result = false;
            //using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator())
            //{
            //    while (enumerator.MoveNext())
            //    {
            //        ManagementObject managementObject = (ManagementObject)enumerator.Current;
            //        if (managementObject["Name"].ToString() == configTxt.PrintName)
            //        {
            //            int num = int.Parse(managementObject["ExtendedPrinterStatus"].ToString());
            //            if (num == 7 || num == 9 || num == 11)
            //            {
            //                // num = 7 - Offline , 9 - Error , 11 - Not Available
            //                result = false;
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            result = true;
            //            break;
            //        }
            //    }
            //}
            //return result;

            ConfigTxt txt = new ConfigTxt();
            ManagementObjectCollection objects = new ManagementObjectSearcher("SELECT * FROM Win32_Printer").Get();
            foreach (ManagementObject obj2 in objects)
            {
                if (obj2["Name"].ToString() == txt.PrintName)
                {
                    switch (int.Parse(obj2["ExtendedPrinterStatus"].ToString()))
                    {
                        case 7:  //Offline
                        case 9:  //Error
                        case 11: //Not Available
                            return false;
                    }
                    return true;
                }
            }
            return false;


        }

        /// <summary>
        /// Проверяет существует ли шрифт штрихкода для CrystalReports
        /// </summary>
        public static void FontBarcod() 
        {
            string filePaths = "C:\\Windows\\Fonts\\IDAutomationHC39M.ttf";
            string paths = "" + AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\IDAutomationHC39M.ttf";
            if (!File.Exists(filePaths))
            {
                MessageBox.Show(" Отствует шрифт штрихкода  IDAutomationHC39M.ttf , пожалуйста скопируйте файл из папки " + paths + " в  папку C:\\Windows\\Fonts", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Write("Отствует шрифт штрихкода  IDAutomationHC39M.ttf");
                Environment.Exit(0);
            }
        }

    }
}
