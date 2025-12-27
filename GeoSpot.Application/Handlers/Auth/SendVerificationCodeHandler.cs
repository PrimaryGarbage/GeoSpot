using System.Security.Cryptography;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using MediatR;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Handlers.Auth;

public record SendVerificationCodeRequest(SendVerificationCodeRequestDto RequestDto) : IRequest;

public class SendVerificationCodeHandler : IRequestHandler<SendVerificationCodeRequest>
{
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly ISmsService _smsService;
    private readonly ICacheService _cacheService;
    private readonly VerificationCodeConfigurationSection _configuration;

    public SendVerificationCodeHandler(IVerificationCodeRepository verificationCodeRepository, ISmsService smsService, 
        ICacheService cacheService, IOptions<VerificationCodeConfigurationSection> configuration)
    {
        _verificationCodeRepository = verificationCodeRepository;
        _smsService = smsService;
        _cacheService = cacheService;
        _configuration = configuration.Value;
    }

    public async Task Handle(SendVerificationCodeRequest request, CancellationToken ct)
    {
        const string smsMessageTemplate = "GeoSpot service. You verification code is: {0}";
        TimeSpan codeGenerationCooldown = TimeSpan.FromSeconds(_configuration.LifespanSeconds);
        
        VerificationCodeModel? existingCode = await _cacheService.GetAsync<VerificationCodeModel>(
            CacheKeys.VerificationCodeModel(request.RequestDto.PhoneNumber));
        
        if(existingCode is not null && DateTime.UtcNow - existingCode.CreatedAt < codeGenerationCooldown)
            throw new BadRequestException($"Previous verification code request was made in less than {_configuration.LifespanSeconds} seconds");
        
        int[] digits = new int[_configuration.NumberOfDigits];
        for (int i = 0; i < digits.Length; ++i)
            digits[i] = RandomNumberGenerator.GetInt32(0, 10);
        
        string code = string.Join(null, digits);
        
        CreateVerificationCodeModel createModel = new()
        {
            PhoneNumber = request.RequestDto.PhoneNumber,
            VerificationCode = code
        };
        
        VerificationCodeModel createdCode = await _verificationCodeRepository.CreateVerificationCodeAsync(createModel, ct);
        
        await _cacheService.SetAsync(CacheKeys.VerificationCodeModel(request.RequestDto.PhoneNumber), createdCode, codeGenerationCooldown);
        
        await _smsService.SendSmsAsync(request.RequestDto.PhoneNumber, string.Format(smsMessageTemplate, code), ct);
    }
}