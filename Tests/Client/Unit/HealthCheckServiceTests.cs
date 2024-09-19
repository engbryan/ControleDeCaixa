using EntryControl.POS.Core.Models;
using EntryControl.POS.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EntryControl.Tests.Services
{
    public class HealthCheckServiceTests
    {
        //[Fact]
        //public void OnOptionsChanged_DeveReconfigurarTimerComNovoIntervalo_E_LogarAlteracao()
        //{
        //    // Arrange
        //    var initialOptions = new ExternalServiceOptions
        //    {
        //        HealthCheckInterval = 1000
        //    };
        //    var updatedOptions = new ExternalServiceOptions
        //    {
        //        HealthCheckInterval = 5000
        //    };

        //    var optionsMock = new Mock<IOptionsMonitor<ExternalServiceOptions>>();
        //    optionsMock.Setup(o => o.CurrentValue).Returns(initialOptions);

        //    var loggerMock = new Mock<ILogger<TimerService>>();
        //    var apiMock = new Mock<IEntryApi>();

        //    var service = new TimerService(optionsMock.Object, loggerMock.Object);

        //    // Act
        //    optionsMock.Setup(o => o.CurrentValue).Returns(updatedOptions);
        //    service.OnOptionsChanged(updatedOptions);

        //    // Assert
        //    Assert.Equal(5000, optionsMock.Object.CurrentValue.HealthCheckInterval);
        //}


    }
}
