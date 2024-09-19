using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Data.Repositories;
using EntryControl.POS.Domain.Entities;
using EntryControl.POS.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EntryControl.Tests.Services
{
    public class SynchronizeServiceTests
    {
        //[Fact]
        //public async Task Synchronize_DeveEnviarLancamentosNaoSincronizados_E_MarkAsSynchronized()
        //{
        //    // Arrange
        //    var entriesNotSynced = new List<Entry>
        //    {
        //        new Entry { ClientId = 1, Synchronized = false },
        //        new Entry { ClientId = 2, Synchronized = false }
        //    };

        //    var repositoryMock = new Mock<IEntryRepository>();
        //    repositoryMock.Setup(r => r.GetDirty()).Returns(entriesNotSynced.AsQueryable());
        //    var userProviderMock = new Mock<IUserService>();

        //    var apiMock = new Mock<IEntryApi>();
        //    apiMock.Setup(a => a.SynchronizeEntriesAsync(It.IsAny<Entry>())).ReturnsAsync(
        //            new ApiResponse<string>(
        //                new HttpResponseMessage(HttpStatusCode.OK),
        //                null,
        //                null
        //             )
        //        );

        //    var sendServiceMock = new Mock<ISendService>();
        //    //sendServiceMock.Setup(s => s.Send(It.IsAny<Entry>())).Returns(Task.FromResult(true));

        //    var service = new SynchronizeService(repositoryMock.Object,
        //        new Mock<IHealthCheckService>().Object,
        //        sendServiceMock.Object,
        //        new Mock<ILogger<SynchronizeService>>().Object);

        //    // Act
        //    await service.Synchronize();

        //    // Assert
        //    apiMock.Verify(a => a.SynchronizeEntriesAsync(It.IsAny<Entry>()), Times.Exactly(entriesNotSynced.Count));
        //    repositoryMock.Verify(r => r.MarkAsSynchronized(It.IsAny<int>()), Times.Exactly(entriesNotSynced.Count));
        //}


        //[Fact]
        //public async Task Synchronize_DeveManterStatusNaoSincronizadoQuandoFalha()
        //{
        //    // Arrange
        //    var entry = new Entry { ClientId = 1, Synchronized = false };
        //    var repositoryMock = new Mock<IEntryRepository>();
        //    repositoryMock.Setup(r => r.GetDirty()).Returns(new List<Entry> { entry }.AsQueryable());
        //    var userProviderMock = new Mock<IUserService>();
        //    var apiMock = new Mock<IEntryApi>();
        //    apiMock.Setup(a => a.SynchronizeEntriesAsync(It.IsAny<Entry>())).ThrowsAsync(new Exception("Erro de conexão"));

        //    var sendServiceMock = new Mock<ISendService>();

        //    var loggerMock = new Mock<ILogger<SynchronizeService>>();
        //    var service = new SynchronizeService(repositoryMock.Object,
        //        new Mock<IHealthCheckService>().Object,
        //        sendServiceMock.Object,
        //        loggerMock.Object);

        //    // Act
        //    await service.Synchronize();

        //    // Assert
        //    repositoryMock.Verify(r => r.MarkAsSynchronized(It.IsAny<int>()), Times.Never);
        //}




    }
}
