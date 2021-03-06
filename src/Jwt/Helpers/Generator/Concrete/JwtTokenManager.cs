﻿using Jwt.Helpers.Generator.Abstractions;
using Jwt.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.Helpers.Generator.Concrete
{
    /// <summary>
    /// This class includes implementations for jwt token generator.
    /// </summary>
    public class JwtTokenManager : IJwtTokenService
    {
        #region Fields
        private readonly JwtOptions jwtOptions;

        public JwtTokenManager(JwtOptions jwtOptions)
        {
            this.jwtOptions = jwtOptions;
        }
        #endregion

        #region Generate Jwt Token
        public async Task<string> GenerateJwtTokenAsync(Claim[] claims, Algorithms algorithm)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Audience = jwtOptions.Audience,
                Issuer = jwtOptions.ClaimsIssuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.LifeTimeMins),
                SigningCredentials = new SigningCredentials(securityKey, await SelectSecurityAlgorithm(algorithm))
            });
            return tokenHandler.WriteToken(token);
        }

        protected async Task<string> SelectSecurityAlgorithm(Algorithms algorithm)
        {
            string selectedSecurityAlgorithm;
            switch (algorithm)
            {
                case Algorithms.HmacSha256Signature:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
                    break;
                case Algorithms.HmacSha384:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
                    break;
                case Algorithms.HmacSha512:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
                    break;
                case Algorithms.RsaSha256Signature:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
                    break;
                case Algorithms.RsaSha384Signature:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
                    break;
                case Algorithms.RsaSha512Signature:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
                    break;
                case Algorithms.EcdsaSha256:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256";
                    break;
                case Algorithms.EcdsaSha384:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384";
                    break;
                case Algorithms.EcdsaSha512:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512";
                    break;
                case Algorithms.RsaSsaPssSha256:
                    selectedSecurityAlgorithm = "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1";
                    break;
                case Algorithms.RsaSsaPssSha384:
                    selectedSecurityAlgorithm = "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1";
                    break;
                default:
                    selectedSecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
                    break;
            }
            return selectedSecurityAlgorithm;
        }
        public async Task<string> GenerateJwtTokenAsync()
        {
            return await GenerateJwtTokenAsync(new Claim[0],Algorithms.HmacSha256Signature);
        }

        #endregion

        #region Generate Claims
        public async Task<Claim[]> GenerateClaimsAsync(List<ClaimDto> claimDto)
        {
            var claims = new List<Claim>();
            foreach (var claim in claimDto)
                claims.Add(new Claim(claim.Type, claim.Value));
            return claims.ToArray();
        }
        #endregion

        #region Generate Refresh Token
        public Task<string> GenerateRefreshTokenAsync(int size = 64)
        {
            var randomNumber = new byte[size];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Task.FromResult(Convert.ToBase64String(randomNumber));
            }
        }
        #endregion
    }
}
