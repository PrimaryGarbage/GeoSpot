using System.Security.Cryptography;
using GeoSpot.Application.Services.Interfaces;

namespace GeoSpot.Application.Services;

[ExcludeFromCodeCoverage]
internal class VerificationCodeGenerator : IVerificationCodeGenerator
{
    public string GenerateCode(int numberOfDigits)
    {
        int[] digits = new int[numberOfDigits];
        for (int i = 0; i < digits.Length; ++i)
            digits[i] = RandomNumberGenerator.GetInt32(0, 10);
        
        return string.Join(null, digits);
    }
}