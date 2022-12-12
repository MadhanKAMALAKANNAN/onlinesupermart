//Created by: Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com,  Nov/2022

using System;
namespace AuthenticationLibrary
{
    public class TokenParameters
    {
        public string AuthenticationType { get; set; }
        public string IssuerSigningKey { get; set; }
        public string Issuer { get; set; }
        public string Token { get; set; }
        public string CipherToken { get; set; }
    }
}
 

