using System;
using System.Collections.Generic;

[Serializable]
public class I_User
{
    public string id;
    public UserData userData;
    public List<SocialStat> socialStats;
    public List<PendingSocialStat> pendingSocialStats;
    public List<SentSocialStat> sentSocialStats;
    public PersonalStats personalStats;
    public List<Critteron> critterons;
    public List<RoomOwned> roomOwned;
    public long lastClosedTime;

    [Serializable]
    public class UserData
    {
        public string name;
        public int level;
        public int experience;
        public int money;
        public string currentCritteron;
    }

    [Serializable]
    public class SocialStat
    {
        public string friendID;
    }

    [Serializable]
    public class PendingSocialStat
    {
        public string friendID;
    }

    [Serializable]
    public class SentSocialStat
    {
        public string friendID;
    }


    [Serializable]
    public class PersonalStats
    {
        public int globalSteps;
        public int daysStreak;
        public int weekSteps;
        public int combatWins;
        public int critteronsOwned;
        public int percentHotel;
    }

    [Serializable]
    public class Critteron
    {
        public string critteronID;
        public int level;
        public int exp;
        public float currentLife;
        public StartInfo startInfo;

        [Serializable]
        public class StartInfo
        {
            public int stepAsPartner;
            public int usedAttacks;
            public int combatWins;
            public int timeAnaerobic;
            public int stepsDoingParade;
        }
    }

    [Serializable]
    public class RoomOwned
    {
        public string roomID;
    }
}
