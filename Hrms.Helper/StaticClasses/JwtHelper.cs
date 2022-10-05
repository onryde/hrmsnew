using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Hrms.Helper.StaticClasses
{
    public static class JwtHelper
    {
        public static string GetJwtToken(JwtSettings jwtSettings, EmployeeTokenDto user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));
            var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.EncryptionKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Name", user.Name),
                    new Claim("Role", user.Role),
                    new Claim("Id", user.EmployeeId.ToString()),
                    new Claim("Guid", user.Guid),
                }),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(signingKey,
                    SecurityAlgorithms.HmacSha256),
                EncryptingCredentials = new EncryptingCredentials(                    
                    encryptionKey,
                    JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var finalToken = tokenHandler.WriteToken(token);

            return finalToken;
        }

        public static EmployeeTokenDto DecodeJwtToken(JwtSettings jwtSettings, string token)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtSettings.SigningKey));
            var encryptionKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtSettings.EncryptionKey));
            
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                    jwtSettings.Audience
                },
                ValidIssuers = new string[]
                {
                    jwtSettings.Issuer
                },
                IssuerSigningKey = signingKey,
                TokenDecryptionKey = encryptionKey
            };
            
            SecurityToken validatedToken;
            var handler = new JwtSecurityTokenHandler();

            var claims = handler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            var claimData = new EmployeeTokenDto();

            foreach (Claim claim in claims.Claims)
            {
                switch (claim.Type)
                {
                    case "Name":
                        claimData.Name = claim.Value;
                        break;

                    case "Id":
                        claimData.EmployeeId = Int64.Parse(claim.Value);
                        break;

                    case "Role":
                        claimData.Role = claim.Value;
                        break;

                    case "Guid":
                        claimData.Guid = claim.Value;
                        break;
                }
            }

            return claimData;
        }
    }
}