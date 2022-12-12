//Created by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  Nov/2022

using System;
///using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using System.Collections.Generic;
 using System.Text;
///sing Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using JwtSecurityToken1 = System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
namespace AuthenticationLibrary
{
    public enum TokenLogMessages
    {
        IDX10682,
        IDX10679, IDX10610, IDX10611, IDX10607, IDX10607ID
    }

    public class TokenServices
    {
        public int MaximumTokenSizeInBytes = 256000;
       
        public readonly int DefaultTokenLifetimeInMinutes = 60;

        public Func<byte[], string, string> decompressTokenType;
        public TokenServices()
        {
            
        }
        public  string GenerateToken(string cliamType, string cliamTypeValue)
        {
            var jwtHeader = new JwtHeader();

            var jwtPayload = new JwtPayload();
            jwtPayload.AddClaim(new System.Security.Claims.Claim(cliamType, cliamTypeValue));

            var token = new JwtSecurityToken(jwtHeader, jwtPayload);

            return token.ToString() ;//await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
 
        public Token GenerateEncryptedToken(string tokenIssuer, string authenticationType, string symmetricKey, string cliamType, string cliamTypeValue)
        {
       
            try
            {
                var param = new TokenParameters();

                param.AuthenticationType = authenticationType;// "Authorization";
                param.IssuerSigningKey = symmetricKey;
                param.Issuer = tokenIssuer;
                param.Token   = AESOperation.RandomString(32,false);
              // = GenerateToken(cliamType, cliamTypeValue);
                
                //token += tokenIssuer + authenticationType;
                var encryptedTokenString = AESOperation.EncryptString(param);
                Token token = new Token();
                token.TokenPart = param.Token;
                token.CipherToken = encryptedTokenString;

                return token; 
            }
            catch (Exception ex) { }
            return null;
        }
        
        public bool IsTokenValid(Token token,TokenValidationParameters tokenValidationParams)//string symmetricKey, string tokenIssuer, string authenticationType, string cliamType, string cliamTypeValue)
        {
            try
            {
                TokenParameters tokenParams = new TokenParameters();
                tokenParams.CipherToken = tokenValidationParams.CipherToken;
                //tokenParams.AuthenticationType = tokenValidationParams.AuthenticationType;
                //tokenParams.Issuer = tokenValidationParams.Issuer;
                tokenParams.IssuerSigningKey= tokenValidationParams.IssuerSigningKey;
                tokenParams.Token = null;
                //decipher
                var decryptedTokenString = AESOperation.DecryptString(tokenParams);
                //validate
                StringBuilder tokenTobeVerified = new StringBuilder();
                tokenTobeVerified.Append(tokenValidationParams.Issuer);
                tokenTobeVerified.Append(token.TokenPart);
                tokenTobeVerified.Append(tokenValidationParams.AuthenticationType);
                if(tokenTobeVerified.ToString() == decryptedTokenString) { return true; }
                else
                {
                    return false;
                }
            } 
            catch (Exception ex) { }
            return false;   
        }

      /*  public class TokenValidationParameters
        {
            public string AuthenticationType { get; set; }
            public string IssuerSigningKey { get; set; }
            public string Issuer { get; set; }
            public string Token { get; set; }
        }
      */
    }

}