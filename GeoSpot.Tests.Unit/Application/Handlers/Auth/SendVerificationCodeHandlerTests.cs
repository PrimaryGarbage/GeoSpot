using GeoSpot.Application.Handlers.Auth;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GeoSpot.Tests.Unit.Application.Handlers.Auth;

public class SendVerificationCodeHandlerTests
{
    private readonly SendVerificationCodeHandler _hanlder;
    private readonly IVerificationCodeRepository _verificationCodeRepositoryMock;
    private readonly ISmsService _smsServiceMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly IOptions<VerificationCodeConfigurationSection> _optionsMock;

    public SendVerificationCodeHandlerTests()
    {
        _verificationCodeRepositoryMock = Substitute.For<IVerificationCodeRepository>();
        _smsServiceMock = Substitute.For<ISmsService>();
        _cacheServiceMock = Substitute.For<ICacheService>();
        _optionsMock = Substitute.For<IOptions<VerificationCodeConfigurationSection>>();
        
        _optionsMock.Value.Returns(new VerificationCodeConfigurationSection { LifespanSeconds = 10, NumberOfDigits = 6 });
            
        _hanlder = new SendVerificationCodeHandler(_verificationCodeRepositoryMock, _smsServiceMock, _cacheServiceMock, _optionsMock);
    }

    [Fact]
    public async Task Handle_WhenCalledWithValidInput_CreatesVerificationCodeAndSendsSms()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
}