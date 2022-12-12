//Created by: Madhan KAMALAKANNAN,  Aug/2022
// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AuthenticationLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using onlinesupermartSQLElasticDB;
using onlinesupermartSQLElasticDB.Models;

public class AesOperation
{
    /*
    public static string EncryptString(string key, string plainText)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV =  iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public static string DecryptString(string key, string cipherText)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV =  iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
    */
}

class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");

        // var key = "87uOhklWUy0101r1m&@glNqli1892014";

        //Console.WriteLine("Please enter a secret key for the symmetric algorithm.");  
        //var key = Console.ReadLine();  

        //Console.WriteLine("Please enter a string for encryption");
        // var str = Console.ReadLine();
        // var encryptedString = AESOperation.EncryptString(key, str);
        // Console.WriteLine($"encrypted string = {encryptedString}");

        //var decryptedString = AESOperation.DecryptString(key, encryptedString);
        // Console.WriteLine($"decrypted string = {decryptedString}");

        // Console.ReadKey();

        AuthenticationLibraryTest authenticationLibraryTest = new AuthenticationLibraryTest();
        authenticationLibraryTest.AuthenticationLibraryMthod();

    }
}

public class AuthenticationLibraryTest
{
    public void AuthenticationLibraryMthod()
    {
        Sharding sharding = new Sharding();
        ShardMapManager shardMapManager = sharding.GetShardManager(sharding.config["SQLServerName"], sharding.config["ShardMapManagerDatabaseName"], sharding.config["connectionString"]); //, true, loggerList);
        Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext> dbContextOptionsBuilder =
          new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AzureonlinesupermartDbContext>();
        var connectionString = sharding.config["connectionString"];
        var lastDbName = ShardManagementUtils.GetLastShardMapDBName(shardMapManager, sharding.config["shardName"]);
        connectionString = connectionString.Replace("onlinesupermartSQLElasticDB", lastDbName);
        dbContextOptionsBuilder.UseSqlServer(connectionString);
        using (AzureonlinesupermartDbContext azureonlinesupermartDbContext = new AzureonlinesupermartDbContext(dbContextOptionsBuilder.Options))
        {
            var httpContext = new DefaultHttpContext();

            Authentication authentication = new Authentication(azureonlinesupermartDbContext, httpContext);

            Console.WriteLine("Is authentication object not  null:{0}", authentication);

            AspNetUsers aspnetUsers = new AspNetUsers();
            aspnetUsers.Email = "AspNetUsers1@email.com";
            aspnetUsers.NormalizedEmail = aspnetUsers.Email;
            aspnetUsers.EmailConfirmed = true;
            aspnetUsers.UserName = aspnetUsers.Email;
            aspnetUsers.TwoFactorEnabled = false;

            //new user regist test
            var newUserRegOk = authentication.RegisterUserOk(aspnetUsers, "Pas");
            var signOut = authentication.SignOut();
            Console.WriteLine("Is SignOut Successfull:{0}", signOut);


            //signin Pass test with correct password
            var token = authentication.SignIn(aspnetUsers.UserName, "Pas1");
            if (token != null)
            {

                Console.WriteLine("SignIn Successfull with tokens={0}   , token partial : {1}:", token.CipherToken, token.TokenPart);

                //token validity test
                var isTokenValid = authentication.IsTokenValid(token);
                Console.WriteLine("Is Token Valid?{0}" + isTokenValid);

            }
        }
    }
}
        