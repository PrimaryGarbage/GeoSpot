using System.Security.Cryptography;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using MediatR;

namespace GeoSpot.Application.Handlers.Auth;

public record SendVerificationCodeRequest(SendVerificationCodeDto Dto) : IRequest;

public class SendVerificationCodeHandler : IRequestHandler<SendVerificationCodeRequest>
{
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    public SendVerificationCodeHandler(IVerificationCodeRepository verificationCodeRepository)
    {
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task Handle(SendVerificationCodeRequest request, CancellationToken ct)
    {
        int[] digits = new int[6];
        for (int i = 0; i < digits.Length; ++i)
            digits[i] = RandomNumberGenerator.GetInt32(0, 10);
        
        string code = string.Join(null, digits);
        
        CreateVerificationCodeModel createModel = new()
        {
            PhoneNumber = request.Dto.PhoneNumber,
            VerificationCode = code
        };
        
        await _verificationCodeRepository.CreateVerificationCodeAsync(createModel, ct);
    }
}