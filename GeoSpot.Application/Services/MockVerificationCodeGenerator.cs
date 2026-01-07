using GeoSpot.Application.Services.Interfaces;

namespace GeoSpot.Application.Services;

public class MockVerificationCodeGenerator : IVerificationCodeGenerator
{
    private const char Digit = '0';
    
    public string GenerateCode(int numberOfDigits)
    {
        return new string(Digit, numberOfDigits);
    }
}