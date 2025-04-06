using System;
using System.Collections.Generic;

[Serializable]
public class I_GameInfo   {
    public string id;
    public List<Critteron> critterons;
    public List<Room> rooms;
    public List<Mark> marks;
    public WeekRewards weekRewards;
    public int cureTime;
    public int markTime;


    [Serializable]
    public class Critteron  {
        public string critteronID;
    }

    [Serializable]
    public class Room
    {
        public string roomID;
    }

    [Serializable]
    public class Mark
    {
        public string markID;
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