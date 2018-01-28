using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceListCash.Classes
{
    /// <summary>
    ///  Инициализируем свойства из config.txt
    /// </summary>
    class ConfigTxt
    {

        public string PrintName { get; set; }

        public int TimeAutoPrint { get; set; } 

        public ConfigTxt()
        {
            Properties.Settings App = new Properties.Settings();
            PrintName = App.NamePrinter;
            TimeAutoPrint = App.Timeautoprint;
           
            //try
            //{
            //    string[,] strArr = ReadParam();
            //    if (strArr.Length == 0)
            //    {
            //        Log.Write("Ошибка в файле конфигурации Внимание!");
            //        MessageBoxResult result = MessageBox.Show("Ошибка в файле конфигурации Внимание!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //        Application.Current.Shutdown();
            //    }
            //    for (int i = 0; i < strArr.Length / 2; i++)
            //    {
            //        switch (strArr[i, 0].ToLower())
            //        {
            //            case "printname":
            //                PrintName = strArr[i, 1];
            //                break;
            //            case "timeautoprint":
            //                TimeAutoPrint = Convert.ToInt32(strArr[i, 1]) ;
            //                break;
            //        }
            //    }
            //}
            //catch (Exception )
            //{
            //    throw;
            //}
        }


        /// <summary>
        /// Метод читает все параметры из файла config.txt и возращает коллекцию.
        /// </summary>
        /// <returns></returns>
        public string[,] ReadParam()
        {
            try
            {
                char[] delimeter = { '=' };
                string str;
                List<string> strList = new List<string>();
                string[,] strArr;
                int k = 0;
                StreamReader streamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\config.txt");
                while (!streamReader.EndOfStream)
                {
                    str = streamReader.ReadLine();
                    if (!str.StartsWith("//"))
                    {
                        strList.AddRange(str.Split(delimeter));
                    }
                }
                strArr = new string[strList.Count / 2, 2];
                for (int j = 0; j < 2; j++)
                {
                    if (j == 1)
                    { k = 1; }

                    for (int i = 0; i < strList.Count / 2; i++)
                    {
                        strArr[i, j] = strList[k];
                        k += 2;
                    }
                }
                return strArr;

            }
            catch (Exception ex)
            {
                Log.Write(ex, "Ошибка  файла config.txt ");
                MessageBoxResult result = MessageBox.Show("Ошибка  файла config.txt , обратитесь к  системному администратору ( " + ex + " )", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return new string[0, 0];
            }
        }




    }
}
