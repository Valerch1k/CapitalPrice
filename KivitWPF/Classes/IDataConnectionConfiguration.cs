using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceListCash.Classes
{
    interface IDataConnectionConfiguration
    {
        string GetSelectedSource();
        void SaveSelectedSource(string provider);

        string GetSelectedProvider();
        void SaveSelectedProvider(string provider);
    }
}
