//Created by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  Nov/2022
//Modified by: Madhan KAMALAKANNAN,  Dec/2022

using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
//using Microsoft.AspNet.Identity;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
//using IdentityUser = Microsoft.AspNet.Identity.IdentityUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using onlinesupermartSQLElasticDB;
using onlinesupermartSQLElasticDB.Models;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
using System;
///using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationLibrary
{
    public class Authentication
    {
        public AzureonlinesupermartDbContext azureonlinesupermartDbContext { get; set; }
        public TokenServices tokenServices;
        public HttpContext httpContext { get; set; }
        public HttpRequest httpRequest { get; set; }
        public AspNetUsers identityUser { get; set; }
      public IConfiguration config { get; set; }
        public Authentication(AzureonlinesupermartDbContext azureonlinesupermartDbContext, HttpContext httpContext  )
        {
            this.azureonlinesupermartDbContext = azureonlinesupermartDbContext;
            this.httpContext = httpContext;
            this.httpRequest = httpContext.Request;
            config = SqlDatabaseUtils.GetConfigaration();
            tokenServices = new TokenServices();
        }

        public bool IsAuthenticated { get; set; }
       
        public Token SignIn(string UserName, string Password)
        {
            IsAuthenticated = false;
             if (UserName != null && Password != null)
            {   
              if(CheckPasswordAsync(UserName, Password).Result) 
                {
                   var token = tokenServices.GenerateEncryptedToken(config["issuer"], "Authorization", config["symmetricKey"], config["claimType"], UserName);
                    if (httpContext != null)
                    {
                        httpContext.Request.Headers.Remove("identityUser");
                        httpContext.Request.Headers.Remove("Authorization");
                        httpContext.Request.Headers.Remove("TokenPart");
                        httpContext.Request.Headers.Add("identityUser", UserName);
                        httpContext.Request.Headers.Add("Authorization", token.CipherToken);//add token data;
                        httpContext.Request.Headers.Add("TokenPart", token.TokenPart);//add token data;
                    }
                  IsAuthenticated = true;
                    return token;
                } 
            }
            return null;
        }
        
        public bool SignOut()
        {
            try
            {
                IsAuthenticated = false;
                if (httpContext != null && httpRequest != null)
                {
                    var authorizationHeader = httpRequest.Headers.SingleOrDefault(x => x.Key == "Authorization");
                   
                    httpContext.Request.Headers.Remove("identityUser");
                    httpContext.Request.Headers.Remove("Authorization");
                    httpContext.Request.Headers.Remove("TokenPart");
                }
                 
            } catch (Exception ex) { return false; }
            return true;
        }

        public bool RegisterUserOk(AspNetUsers aspNetUsersModel, string Password)
        {
            if (aspNetUsersModel != null && Password != null)
            {
                try
                {
                    AspNetUsers identityUser = aspNetUsersModel;
                    if (azureonlinesupermartDbContext.AspNetUsers.FirstOrDefault(x => x.UserName == aspNetUsersModel.UserName || x.Email == aspNetUsersModel.Email) == null)
                    {
                        var identityResult = CreateAsync(identityUser, Password).GetAwaiter().GetResult();
                        return true;
                    }
                 
                }
                  catch(Exception ex)
                {
                    return false;
                }
               
            }
            return false;
        }
        public bool IsTokenValid(Token token)
        {
            var param = new TokenValidationParameters();

            param.AuthenticationType = "Authorization";
            param.IssuerSigningKey = config["symmetricKey"];
            param.Issuer = config["issuer"];
            param.CipherToken = token.CipherToken;
            return tokenServices.IsTokenValid(token, param);
        }
        public  async Task<IdentityResult> CreateAsync(AspNetUsers identityUser, string password)
        { 
            if (identityUser == null)
            {
                throw new ArgumentNullException(nameof(identityUser));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            } 
             
          
            try
            { 
                identityUser.ConcurrencyStamp = Guid.NewGuid().ToString();
                identityUser.NormalizedEmail = identityUser.NormalizedEmail == null ? identityUser.Email : identityUser.NormalizedEmail;
                identityUser.TwoFactorEnabled = false;
                identityUser.LockoutEnabled = true;
                identityUser.AccessFailedCount = 0;
                //CheckPasswordAsync(identityUser, password);
                await UpdatePasswordHash(identityUser, password);
                SetSecurityStampAsync(identityUser, NewSecurityStamp());
                azureonlinesupermartDbContext.AspNetUsers.Add(identityUser);
                azureonlinesupermartDbContext.SaveChanges(); return IdentityResult.Success;
            }
            catch (Exception ex) { }

            var error = new IdentityError[1];
            error[0].Code  = "error";
            error[0].Description = "error";
            return IdentityResult.Failed(error);   
        }
        public virtual async Task<bool> CheckPasswordAsync(string UserName, string password)
        {
            AspNetUsers identityUser = azureonlinesupermartDbContext.AspNetUsers.FirstOrDefault(x => x.UserName == UserName || x.Email == UserName);
            return await CheckPasswordAsync( identityUser,  password);
        }
        public virtual async Task<bool> CheckPasswordAsync(AspNetUsers identityUser, string password)
        { 
            
            if (identityUser == null)
            {
                return false;
            }

          
            //var hash = passwordHasher.HashPassword(identityUser,password);
           
            SetSecurityStampAsync(identityUser, NewSecurityStamp());

            var result =   await VerifyPasswordAsync( identityUser, password);
           // if (result == PasswordVerificationResult.SuccessRehashNeeded)
           // {
             //   await UpdatePasswordHash(identityUser, password); 
            ///}

            var success = result != PasswordVerificationResult.Failed;
           
            return success;
        }
        public async Task<PasswordVerificationResult> UpdatePasswordHash(AspNetUsers identityUser, string password)
        {
            //// PasswordHasher<AspNetUsers> passwordHasher = new PasswordHasher<AspNetUsers>();
            //var hasedPassword = passwordHasher.HashPassword(identityUser, password);
            // var encryptedTokenString = AESOperation.EncryptString(param);
            var param = new TokenParameters();

            param.AuthenticationType =  "Password";
            param.IssuerSigningKey = config["symmetricKey"];
            param.Issuer = ""; // /config["issuer"];
            param.Token = password;//AESOperation.RandomString(32, false);
            // = GenerateToken(cliamType, cliamTypeValue);

            //token += tokenIssuer + authenticationType;
            var encryptedTokenString = AESOperation.EncryptString(param);
           // Token token = new Token();
            //token.TokenPart = param.Token;
           // token.CipherToken = encryptedTokenString;

            identityUser.PasswordHash = encryptedTokenString;
                azureonlinesupermartDbContext.AspNetUsers.Update(identityUser);

            return PasswordVerificationResult.Success;
        }
       
        public virtual async Task<PasswordVerificationResult> VerifyPasswordAsync(AspNetUsers identityUser,string password)
        {
            
            try
            {
                TokenParameters tokenParams = new TokenParameters();
                tokenParams.CipherToken = identityUser.PasswordHash;
                tokenParams.IssuerSigningKey = config["symmetricKey"];


                tokenParams.Issuer = "";  
                tokenParams.Token = null;
                //decipher
                var decryptedTokenString = AESOperation.DecryptString(tokenParams);
                //validate
                StringBuilder tokenTobeVerified = new StringBuilder();
                tokenTobeVerified.Append("");
                tokenTobeVerified.Append(password);
                tokenTobeVerified.Append("Password");
                if (tokenTobeVerified.ToString() == decryptedTokenString) { return PasswordVerificationResult.Success; }
            }
            catch (Exception ex) { }
             
            return PasswordVerificationResult.Failed;
        }
        public virtual Task SetSecurityStampAsync(AspNetUsers identityUser, string stamp)
        {
         
            if (identityUser == null)
            {
                throw new ArgumentNullException("user");
            }
            identityUser.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        private string Token()
        {
            if (httpRequest != null)
            {
                var authorizationHeader = httpRequest.Headers.SingleOrDefault(x => x.Key == "Authorization");
                if(authorizationHeader.Key!= null)
                {
                    var authenticationHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader.Value);

                    if (authenticationHeaderValue == null || !string.Equals(authenticationHeaderValue.Scheme, "Authorization", StringComparison.InvariantCultureIgnoreCase))
                        return null;

                    return authenticationHeaderValue.Parameter;
                }
            }
            return "";
        }

        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
 
            RandomNumberGenerator.Fill(bytes);
 
            return ToBase32(bytes);
        }
        public static string ToBase32(byte[] input)
        {
             const string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StringBuilder sb = new StringBuilder();
            for (int offset = 0; offset < input.Length;)
            {
                byte a, b, c, d, e, f, g, h;
                int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
            }

            return sb.ToString();
        }
        private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        {
            uint b1, b2, b3, b4, b5;

            int retVal;
            switch (input.Length - offset)
            {
                case 1: retVal = 2; break;
                case 2: retVal = 4; break;
                case 3: retVal = 5; break;
                case 4: retVal = 7; break;
                default: retVal = 8; break;
            }

            b1 = (offset < input.Length) ? input[offset++] : 0U;
            b2 = (offset < input.Length) ? input[offset++] : 0U;
            b3 = (offset < input.Length) ? input[offset++] : 0U;
            b4 = (offset < input.Length) ? input[offset++] : 0U;
            b5 = (offset < input.Length) ? input[offset++] : 0U;

            a = (byte)(b1 >> 3);
            b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
            c = (byte)((b2 >> 1) & 0x1f);
            d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
            e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
            f = (byte)((b4 >> 2) & 0x1f);
            g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
            h = (byte)(b5 & 0x1f);

            return retVal;
        }
        //private IUserPasswordStore<IdentityUser> GetPasswordStore()
        //{
        //    //var cast = null;// Store as IUserPasswordStore<IdentityUser>;
        //    //if (cast == null)
        //    //{
        //    //    throw new NotSupportedException(Resources.StoreNotIUserPasswordStore);
        //    //}
        //    return null;
        //}


        //private async Task<IdentityResult> UpdatePasswordHash(string newPassword, bool validatePassword = true)
        //{
        //    if (validatePassword)
        //    {


        //        var validate = userManager.CheckPasswordAsync(identityUser, newPassword);
        //        if (!validate.Result)
        //        {
        //            IdentityError[] ie1 = new IdentityError[1];
        //            ie1.SetValue("error", 0);
        //            return IdentityResult.Failed(ie1);
        //        }
        //    }
        //    Microsoft.AspNet.Identity.PasswordHasher passwordHasher = new PasswordHasher();

        //    var hash =  passwordHasher.HashPassword(newPassword);
        //    identityUser.
        //   //await passwordStore.SetPasswordHashAsync(identityUser, hash, CancellationToken);

        //   await UpdateSecurityStampInternal(identityUser);
        //    return IdentityResult.Success;
        //}

        //private IUserRoleStore<TUser> GetUserRoleStore()
        //{
        //    var cast = null;// Store as IUserRoleStore<TUser>;
        //    //if (cast == null)
        //    //{
        //    //    throw new NotSupportedException(Resources.StoreNotIUserRoleStore);
        //    //}
        //    return cast;
        //}
    }
}
