using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceListCash.Classes
{
    class DBConStrs
    {
        /// <summary>
        /// Возвращает строку подключения
        /// </summary>
        /// <returns>string </returns>
        public static string ConnectionString()
        {
            try
            {
                ConfigTxt config = new ConfigTxt();
                return ConnectionStringDataBase.Read(); 
            }
            catch (Exception ex)
            {
                Log.Write(ex, " - Ошибкав с троке подключения !!!");
                throw ex;
            }
        }

    }
}
