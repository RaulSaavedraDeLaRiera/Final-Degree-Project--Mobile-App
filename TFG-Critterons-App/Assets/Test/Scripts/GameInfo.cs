using System;
using System.Collections.Generic;

[Serializable]
public class GameInfo   {
    public string id;
    public List<Critteron> critterons;
    public List<Forniture> forniture;
    public List<User> users;
    public WeekRewards weekRewards;

    [Serializable]
    public class Critteron  {
        public string critteronID;
    }

    [Serializable]
    public class Forniture  {
        public string fornitureID;
    }

    [Serializable]
    public class User   {
        public string userID;
    }

    [Serializable]
    public class WeekRewards     {
        public Dictionary<string, DayReward> days;

        [Serializable]
        public class DayReward  {
            public int reward1;
            public int reward2;
        }
    }
}