using MongoDB.Bson;
using System;

public class Model_Account
{
    public ObjectId _id { set; get; }

    public int ActiveConnection { set; get; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
    public string Email { set; get; }
    public string ShaPassword { set; get; }

    // flag for user is connected to ...game
    public byte Status { set; get; }
    public string Token { set; get; }
    public DateTime LastLogin { set; get; }

    public Account GetAccount()
    {
        return new Account() { Username = this.Username, ActiveConnection = this.ActiveConnection, Discriminator = this.Discriminator, Status = this.Status };
    }
}