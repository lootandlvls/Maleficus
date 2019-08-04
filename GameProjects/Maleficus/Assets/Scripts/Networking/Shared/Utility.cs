using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class Utility
{
    public const string EMAIL_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
    public const string USERNAME_AND_DISCRIMINATOR_PATTERN = @"^[a-zA-Z0-9]{4,20}#[0-9]{4}$";
    public const string USERNAME_PATTERN = @"^[a-zA-Z0-9]{4,20}$";
    /*
    ^                         Start anchor
    (?=.*[A-Z])               Ensure string has one uppercase letter.
    (?=.*[!@#$&*])            Ensure string has one special case letter.
    (?=.*[0-9])               Ensure string has one digit.
    (?=.*[a-z].*[a-z].*[a-z]) Ensure string has three lowercase letters.
    .{8,20}                      Ensure string is of length 8-20.
    $                         End anchor.
    */
    public const string PASSWORD_PATTERN = "^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,20}$";
    public const string RANDOM_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static bool IsEmail(string email)
    {
        if(email != null)
        {
            return Regex.IsMatch(email, EMAIL_PATTERN);
        }
        else
        {
            return false;
        }
        
    }

    public static bool IsUsername(string username)
    {
        if (username != null)
        {
            return Regex.IsMatch(username, USERNAME_PATTERN);
        }
        else
        {
            return false;
        }
    }

    public static bool IsPassword(string password)
    {
        if(password != null)
        {
            return Regex.IsMatch(password, PASSWORD_PATTERN);
        }
        else
        {
            return false;
        }
    }

    public static bool IsUsernameAndDiscriminator(string username)
    {
        if (username != null)
        {
            return Regex.IsMatch(username, USERNAME_AND_DISCRIMINATOR_PATTERN);
        }
        else
        {
            return false;
        }
    }

    public static string GenerateRandom(int length)
    {
        Random r = new Random();
        return new string(System.Linq.Enumerable.Repeat(RANDOM_CHARS, length).Select(s => s[r.Next(s.Length)]).ToArray());
    }

    public static string Sha256FromString(string toEncrypt)
    {
        var message = Encoding.UTF8.GetBytes(toEncrypt);
        SHA256Managed hashString = new SHA256Managed();

        string hex = "";
        var hashValue = hashString.ComputeHash(message);
        foreach(byte x in hashValue)
        {
            hex += String.Format("{0:x2}", x);
        }

        return hex;
    }
}