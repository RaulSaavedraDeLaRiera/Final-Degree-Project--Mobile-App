using System;
using System.Collections.Generic;

[Serializable]
public class I_GameInfo   {
    public string id;
    public List<Critteron> critterons;
    public List<Forniture> forniture;
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
    public class WeekRewards     {
        public Dictionary<string, DayReward> days;

        [Serializable]
        public class DayReward  {
            public int reward1;
            public int reward2;
        }
    }
}