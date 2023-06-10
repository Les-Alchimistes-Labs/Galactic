using System.Collections.Generic;

namespace personnage_class.Personage.Monsters
{
    public class LittelMonster : Monster
    {
        public override EnumType Type() => EnumType.LittleMonster;

        public LittelMonster(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int level = 0)
            : base(name, (int)(life * 0.5), (int)(maxlife * 0.5), (int)(damage * 0.5), boost, (int)(2),
                (int)(level * 0.5)) // all 0.5 without boost
        {
            inFight = false;
            Inventory[0] = new Food("Banana", 50, 3);
            Inventory[1] = new Food("Cheese", 50, 3);

        }

        public override int Target(List<Personnage> heros , int find)
        {
            var target = heros[0];
            int pos = 0;
            // trouver le médecin 
            if (find is >= 0 and < 25)
                for (int i = 1; i < heros.Count - 1; i++)
                {
                    if (heros[i].Get_damage() > target.Get_damage())
                    {
                        target = heros[i];
                        pos = i;
                    }
                }
            else
                for (int i = 1; i < heros.Count - 1; i++)
                {
                    if (heros[i].Getlife < target.Getlife)
                    {
                        target = heros[i];
                        pos = i;
                    }
                }

            return pos;
        }
    }
}