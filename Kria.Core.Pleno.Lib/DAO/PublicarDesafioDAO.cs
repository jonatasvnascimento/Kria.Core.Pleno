using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
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

        public PublicarDesafioDAO(IConfigurationDao configuration, HttpClient httpClient)
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
            var url = $"{_urlBase}/Candidato/PublicarDesafio";
            var json = JsonSerializer.Serialize(registroPedagio, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var response = await _httpClient.PostAsync(url, content);
                var respostaApi = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Registro de pedágio publicado com sucesso.");
                }
                else
                {
                    Console.WriteLine($"Erro ao publicar registro de pedágio ({response.StatusCode}): {respostaApi}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro de conexão com a API: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }
        }
    }
}
