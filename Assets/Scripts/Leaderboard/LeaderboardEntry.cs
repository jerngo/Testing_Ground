using System;

[Serializable]
public class LeaderboardEntry
{
    public int id;
    public string firstName;
    public string lastName;
    public string username;
    public int age;
}

[Serializable]
public class UsersResponse
{
    public LeaderboardEntry[] users;
    public int total;
    public int skip;
    public int limit;
}