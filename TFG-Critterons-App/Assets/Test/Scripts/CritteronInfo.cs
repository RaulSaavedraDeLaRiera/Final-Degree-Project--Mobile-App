using System;
using System.Collections.Generic;

[Serializable]
public class CritteronInfo
{
    public string id;
    public string name;
    public string mesh;
    public int levelUnlock;
    public int life;
    public int basicDamage;
    public int defense;
    public List<Attack> attacks;

    [Serializable]
    public class Attack
    {
        public string name;
        public int damage;
        public float percent;
        public int type;

        public Attack(string name, int damage, float percent, int type)
        {
            this.name = name;
            this.damage = damage;
            this.percent = percent;
            this.type = type;
        }
    }
}
