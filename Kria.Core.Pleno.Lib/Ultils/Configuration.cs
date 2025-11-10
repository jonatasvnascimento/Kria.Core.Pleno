using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.Ultils
{
    public class Configuration(IConfiguration configuration) : IConfigurationDAO
    {
        public string? PegarChave(string chave) => configuration.GetSection(chave).Value ?? string.Empty;
    }
}
