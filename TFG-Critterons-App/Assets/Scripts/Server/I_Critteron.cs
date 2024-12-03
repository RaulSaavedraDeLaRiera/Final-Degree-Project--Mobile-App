using System;

[Serializable]
public class I_Critteron
{
    public string id;
    public string name;
    public string mesh;
    public int levelUnlock;
    public int life;
    public int basicDamage;
    public int defense;
    public Attack[] attacks = new Attack[2];

    public I_Critteron(string id, string name, string mesh,
        int levelUnlock, int life, int basicDamage, int defense)
    {
        this.id = id;
        this.name = name;
        this.mesh = mesh;
        this.levelUnlock = levelUnlock;
        this.life = life;
        this.basicDamage = basicDamage;
        this.defense = defense;
    }

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
