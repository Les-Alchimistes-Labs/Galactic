using System.Collections.Generic;
using UnityEngine;

namespace personnage_class.Personage
{
    public abstract class Monster : Personnage
    {
        public Monster(string name, int life = 10, int maxlife = 20, int damage = 5, int boost = 1, int inventorySize = 8, int levelt = 0, int maxlevel = 0) : base(name, life, maxlife, damage, boost, inventorySize, levelt, maxlevel)
        {
        }

        public override EnumsPersonage TypePersonage() => EnumsPersonage.Monster;
        
        
        public List<Personnage> Players = new List<Personnage>();



        public override bool Add_Life(int i)
        {
            if (Life + i <= MaxLife)
            {
                Life += i;
                return true;
            }

            return false;
        }

        public override void Remove_Life(int i)
        {
            if (Life-i>0)
                Life -= i;
            else
            {
                Life = 0;
            }
        }

        public override void Update()
        {
            if (inFight)
            {
                if (Players.Count == 0)
                {
                    inFight = false;
                }
                else
                {
                    
                }
            }
        }

        public override void Attack(Personnage victim = null, List<Personnage> victims = null)
        {
            int crit = Random.Range(0, 100);
            if (crit < 10)
                victim.Take_Damage(Get_damage()*2);
            else 
                victim.Take_Damage(Get_damage());
        }

        public abstract void Target(List<Personnage> heros , int find);



    }
}