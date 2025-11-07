using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Interfaces.DAO
{
    public interface IConfigurationDao
    {
        public string? PegarChave(string key);
        public string? PegarConnectionString();
        public string? PegarDataBase();
    }
}
