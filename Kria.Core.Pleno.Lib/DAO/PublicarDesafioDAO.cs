using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Entidades.DTO.DPedagioDTO;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.DAO
{
    public class PublicarDesafioDAO : IPublicarDesafioDAO
    {
        private readonly string _urlBase;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly HttpClient _httpClient;

        public PublicarDesafioDAO(IConfigurationDAO configuration, HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _urlBase = configuration.PegarChave("Api") ?? throw new ArgumentException("Chave 'Api' não encontrada nas configurações.");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task PublicarRegistroPedagio(Pedagio registroPedagio)
        {
            var resgistro = PedagioDTOMapper.Map(registroPedagio);

            var url = $"{_urlBase}/Candidato/PublicarDesafio";
            var json = JsonSerializer.Serialize(resgistro, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var respostaApi = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                Terminal.Mensagem($"Registro de pedágio publicado com sucesso.", "Enviado API: ", ConsoleColor.Magenta);
            else
                Terminal.Mensagem($"Erro ao publicar registro de pedágio ({response.StatusCode}): {respostaApi}", "Enviado API: ", ConsoleColor.Red);
        }
    }
}
