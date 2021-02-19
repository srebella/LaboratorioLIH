using System;

public class GmailEmailSettings  
 {  
     public GmailEmailSettings(string username, string password)  
     {  
         if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))  
             throw new ArgumentException("Api Key must be set");  
  
         this.Server = "smtp.gmail.com";  
         this.Username = username;  
         this.Password = password;  
     }  
  
     public string Server { get; }  
     public string Username { get; }  
     public string Password { get; }  
 }  