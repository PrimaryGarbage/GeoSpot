namespace GeoSpot.Application.Services.Interfaces;

public interface IVerificationCodeGenerator
{
    string GenerateCode(int numberOfDigits);
}