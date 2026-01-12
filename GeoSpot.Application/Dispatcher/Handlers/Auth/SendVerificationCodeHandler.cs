using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

public record SendVerificationCodeRequest(SendVerificationCodeRequestDto RequestDto) : IRequest<Empty>;

public class SendVerificationCodeHandler : IRequestHandler<SendVerificationCodeRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly ISmsService _smsService;
    private readonly ICacheService _cacheService;
    private readonly VerificationCodeConfigurationSection _configuration;
    private readonly IVerificationCodeGenerator _verificationCodeGenerator;

    public SendVerificationCodeHandler(GeoSpotDbContext dbContext, ISmsService smsService, ICacheService cacheService, 
        IOptions<VerificationCodeConfigurationSection> configuration, IVerificationCodeGenerator verificationCodeGenerator)
    {
        _dbContext = dbContext;
        _smsService = smsService;
        _cacheService = cacheService;
        _verificationCodeGenerator = verificationCodeGenerator;
        _configuration = configuration.Value;
    }

    public async Task<Empty> Handle(SendVerificationCodeRequest request, CancellationToken ct)
    {
        const string smsMessageTemplate = "GeoSpot service. You verification code is: {0}";
        TimeSpan codeLifespan = TimeSpan.FromSeconds(_configuration.LifespanSeconds);

        VerificationCodeEntity? existingCode = await _cacheService.GetAsync<VerificationCodeEntity>(
            CacheKeys.VerificationCodeEntity(request.RequestDto.PhoneNumber) ,ct);
        
        if(existingCode is not null && DateTime.UtcNow - existingCode.CreatedAt < codeLifespan)
            throw new BadRequestException($"Previous verification code request was made in less than {_configuration.LifespanSeconds} seconds");
        
        string code = _verificationCodeGenerator.GenerateCode(_configuration.NumberOfDigits);
        
        VerificationCodeEntity createdCode = new()
        {
            PhoneNumber = request.RequestDto.PhoneNumber,
            VerificationCode = code
        };
        
        _dbContext.VerificationCodes.Add(createdCode);
        
        await _dbContext.SaveChangesAsync(ct);
        
        await _cacheService.SetAsync(CacheKeys.VerificationCodeEntity(request.RequestDto.PhoneNumber), createdCode, codeLifespan, ct);
        
        await _smsService.SendSmsAsync(request.RequestDto.PhoneNumber, string.Format(smsMessageTemplate, code), ct);
        
        return Empty.Value;
    }
}