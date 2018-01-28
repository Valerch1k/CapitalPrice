using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace PriceListCash.Classes
{
    public static class MyPrintersOptions
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);

        public static string GetDefaultPrinterName()
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query);
            string result;
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject managementObject = (ManagementObject)enumerator.Current;
                    bool flag = ((bool?)managementObject["Default"]) ?? false;
                    if (flag)
                    {
                        result = (managementObject["Name"] as string);
                        return result;
                    }
                }
            }
            result = null;
            return result;
        }

    }
}
