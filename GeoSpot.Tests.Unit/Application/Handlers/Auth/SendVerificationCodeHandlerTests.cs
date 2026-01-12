using FluentAssertions;
using GeoSpot.Application.Dispatcher.Handlers.Auth;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GeoSpot.Tests.Unit.Application.Handlers.Auth;

public class SendVerificationCodeHandlerTests
{
    private readonly SendVerificationCodeHandler _handler;
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
        IVerificationCodeGenerator verificationCodeGeneratorMock = Substitute.For<IVerificationCodeGenerator>();
        
        _optionsMock.Value.Returns(new VerificationCodeConfigurationSection { LifespanSeconds = 10, NumberOfDigits = 6 });
            
        _handler = new SendVerificationCodeHandler(_verificationCodeRepositoryMock, _smsServiceMock, _cacheServiceMock, 
            _optionsMock, verificationCodeGeneratorMock);
    }

    [Fact]
    public async Task Handle_WhenCalledWithValidInputForTheFirstTime_CreatesVerificationCodeAndSendsSms()
    {
        // Arrange
        const string validPhoneNumber = "+123456789";
        const string verificationCode = "test_verification_code";
        SendVerificationCodeRequest request = new(new SendVerificationCodeRequestDto(validPhoneNumber));
        CancellationToken ct = CancellationToken.None;
        
        _cacheServiceMock.GetAsync<VerificationCodeModel>(Arg.Any<string>())
            .Returns(Task.FromResult<VerificationCodeModel?>(null));
        _verificationCodeRepositoryMock.CreateVerificationCodeAsync(Arg.Any<CreateVerificationCodeModel>(), ct)
            .Returns(new VerificationCodeModel { PhoneNumber = validPhoneNumber, VerificationCode = verificationCode });
        
        // Act
        await _handler.Handle(request, ct);
        
        // Assert
        await _cacheServiceMock.Received()
            .SetAsync(CacheKeys.VerificationCodeModel(validPhoneNumber), 
                Arg.Is<VerificationCodeModel>(x => x.PhoneNumber == validPhoneNumber && x.VerificationCode == verificationCode), 
                TimeSpan.FromSeconds(_optionsMock.Value.LifespanSeconds));
        
        await _smsServiceMock.Received(1).SendSmsAsync(request.RequestDto.PhoneNumber, Arg.Any<string>(), ct);
    }

    [Fact]
    public async Task Handle_WhenCalledWithInvalidCodeCooldownTime_ThrowsBadRequestException()
    {
        // Arrange
        const string validPhoneNumber = "+123456789";
        const string verificationCode = "test_verification_code";
        SendVerificationCodeRequest request = new(new SendVerificationCodeRequestDto(validPhoneNumber));
        CancellationToken ct = CancellationToken.None;

        _cacheServiceMock.GetAsync<VerificationCodeModel>(Arg.Any<string>(), ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new() { CreatedAt = DateTime.UtcNow, PhoneNumber = validPhoneNumber, VerificationCode = verificationCode}));
        
        // Act
        var action = async () => await _handler.Handle(request, ct);

        // Assert
        await action.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Handle_WhenCalledWithValidCodeCooldownTime_CreatesVerificationCodeAndSendsSms()
    {
        // Arrange
        const string validPhoneNumber = "+123456789";
        const string verificationCode = "test_verification_code";
        SendVerificationCodeRequest request = new(new SendVerificationCodeRequestDto(validPhoneNumber));
        CancellationToken ct = CancellationToken.None;
        TimeSpan codeLifespan = TimeSpan.FromSeconds(_optionsMock.Value.LifespanSeconds);

        _cacheServiceMock.GetAsync<VerificationCodeModel>(Arg.Any<string>())
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new() { CreatedAt = DateTime.UtcNow - codeLifespan, PhoneNumber = validPhoneNumber, VerificationCode = verificationCode}));
        _verificationCodeRepositoryMock.CreateVerificationCodeAsync(Arg.Any<CreateVerificationCodeModel>(), ct)
            .Returns(new VerificationCodeModel { PhoneNumber = validPhoneNumber, VerificationCode = verificationCode });

        // Act
        await _handler.Handle(request, ct);

        // Assert
        await _cacheServiceMock.Received()
            .SetAsync(CacheKeys.VerificationCodeModel(validPhoneNumber),
                Arg.Is<VerificationCodeModel>(x =>
                    x.PhoneNumber == validPhoneNumber && x.VerificationCode == verificationCode),
                TimeSpan.FromSeconds(_optionsMock.Value.LifespanSeconds));

        await _smsServiceMock.Received(1).SendSmsAsync(request.RequestDto.PhoneNumber, Arg.Any<string>(), ct);
    }
}