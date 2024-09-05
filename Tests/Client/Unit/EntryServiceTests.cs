using EntryControl.Core.Entities;
using EntryControl.Providers;
using EntryControl.Repositories;
using EntryControl.Services.Impl;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EntryControl.Tests.Services
{
    public class EntryServiceTests
    {
        [Fact]
        public void Add_DeveSalvarLancamentoNoBancoDeDados_E_DispararEvento()
        {
            // Arrange
            var repositoryMock = new Mock<IEntryRepository>();
            var loggerMock = new Mock<ILogger<EntryService>>();
            var userProviderMock = new Mock<IUserProvider>();
            var service = new EntryService(repositoryMock.Object, userProviderMock.Object, loggerMock.Object);
            var type = "Débito";
            var ammount = 100m;
            var description = "Compra de materiais";
            bool eventTriggered = false;

            service.OnEntryAdded += (l) => eventTriggered = true;

            // Act
            service.Add(type, ammount, description);

            // Assert
            repositoryMock.Verify(r => r.Add(It.Is<Entry>(l =>
                l.Type == type &&
                l.Amount == ammount &&
                l.Description == description &&
                l.Synchronized == false &&
                l.EntryDate != default(DateTime))), Times.Once);

            Assert.True(eventTriggered, "O evento OnEntryAdded deve ser disparado após adicionar um lançamento.");
        }


        [Fact]
        public async Task ObterLancamentosSincronizados_DeveRetornarApenasLancamentosSincronizados()
        {
            // Arrange
            var repositoryMock = new Mock<IEntryRepository>();
            repositoryMock.Setup(r => r.GetSynchronized()).Returns(new List<Entry>
            {
                new Entry { ClientId = 1, Synchronized = true },
                new Entry { ClientId = 2, Synchronized = true }
            }.AsQueryable());
            var userProviderMock = new Mock<IUserProvider>();

            var service = new EntryService(repositoryMock.Object, userProviderMock.Object, new Mock<ILogger<EntryService>>().Object);

            // Act
            var result = await service.GetAllSinchronized();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, l => Assert.True(l.Synchronized));
        }
    }
}
