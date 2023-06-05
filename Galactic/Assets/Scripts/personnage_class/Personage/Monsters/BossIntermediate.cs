
using System.Collections.Generic;
using UnityEngine;

namespace personnage_class.Personage.Monsters
{
    public class BossIntermediate : Monster
    {
        public override EnumType Type() => EnumType.IntermediateMonster;

        public BossIntermediate(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1,
            int inventorySize = 8, int level = 0) : base(name, (int)(maxlife * 2.5), (int)(maxlife * 2.5),
            (int)(damage * 1.5), boost, inventorySize, (int)(level * 1.5)) // life *2.5 damage *1.5 level *1.5 
        {
            inFight = false;
            Inventory[0] = new Gun(100, Damage, Boost);
            Change_Weapon_Equipped(0);

        }


        public override void Target(List<Personnage> heros, int find)
        {
            Personnage target = null;
            List<Personnage> targets = null;
            if (find < 20)
                targets = heros; //attaquer tous les ennemis
            if (find is >= 20 and < 35)
            {
                target = heros[0];
                for (int i = 1; i < heros.Count - 1; i++)
                {
                    if (heros[i].Get_damage() > target.Get_damage())
                        target = heros[i];
                }
            }
            else
            {
                target = heros[0];
                for (int i = 1; i < heros.Count - 1; i++)
                {
                    if (heros[i].Getlife < target.Getlife)
                        target = heros[i];
                }
            }

            Attack(target, targets);
        }
    }

}