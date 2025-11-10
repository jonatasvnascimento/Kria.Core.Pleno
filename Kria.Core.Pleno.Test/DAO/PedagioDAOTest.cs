using FluentAssertions;
using Kria.Core.Pleno.Lib.Context;
using Kria.Core.Pleno.Lib.DAO;
using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Kria.Core.Pleno.Test.Integration
{
    public class PedagioDAOIntegrationTests
    {
        private readonly IPedagioDAO _dao;
        private readonly IMongoDbContext _context;
        private readonly ServiceProvider _serviceProvider;

        public PedagioDAOIntegrationTests()
        {
            _serviceProvider = TestDependencyInjection.BuildTestServices();
            using var scope = _serviceProvider.CreateScope();
            _dao = scope.ServiceProvider.GetRequiredService<IPedagioDAO>();
            _context = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
        }

        [Fact(DisplayName = "ObterTodos deve retornar registros existentes no MongoDB")]
        public void ObterTodos_DeveRetornarRegistros()
        {
            // Act
            var registros = _dao.ObterTodos().Take(10).ToList();

            // Assert
            registros.Should().NotBeNull();
            registros.Should().NotBeEmpty("deve haver dados na coleção TabTransacoes para o teste");
            registros.First().Should().BeOfType<TabTransacoes>();
        }

        [Fact(DisplayName = "ObterLote sem ultimaData deve trazer lote limitado")]
        public void ObterLote_SemUltimaData_DeveRetornarLote()
        {
            // Arrange
            int tamanho = 1000;

            // Act
            var lote = _dao.ObterLote(null, tamanho).ToList();

            // Assert
            lote.Should().NotBeNull();
            lote.Should().NotBeEmpty();
            lote.Count.Should().BeLessThanOrEqualTo(tamanho);
        }

        [Fact(DisplayName = "ObterLote com ultimaData deve retornar registros posteriores")]
        public void ObterLote_ComUltimaData_DeveRetornarRegistrosMaisNovos()
        {
            // Arrange
            var primeiroLote = _dao.ObterLote(null, 5).ToList();
            if (!primeiroLote.Any())
                return;

            var ultimaData = primeiroLote.Last().DtCriacao;

            // Act
            var segundoLote = _dao.ObterLote(ultimaData, 100).ToList();

            // Assert
            segundoLote.Should().AllSatisfy(r => r.DtCriacao.Should().BeAfter(ultimaData));
        }

        [Fact(DisplayName = "ObterLote deve retornar lista vazia se não houver novos dados")]
        public void ObterLote_SemNovosDados_DeveRetornarVazio()
        {
            // Arrange
            var ultimaData = DateTime.UtcNow.AddYears(10);

            // Act
            var resultado = _dao.ObterLote(ultimaData, 100);

            // Assert
            resultado.Should().BeEmpty();
        }
    }
}
