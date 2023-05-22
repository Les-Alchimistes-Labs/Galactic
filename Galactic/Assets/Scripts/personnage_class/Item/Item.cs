using System;
using System.Collections;
using System.Collections.Generic;


namespace personnage_class.Personage
{
    public abstract class Item : Update
    {
        protected int Damage;
        protected int Expiry;
        protected int Heal;
        protected bool IsEdible;
        protected int Max_expiry;
        protected float Boost;
        public EnumsItem Type  {protected set; get; }
        public String Name  {set; get; }
        
        protected Item(string name,int expiry, bool isEdible, int heal, EnumsItem type, float boost = 1)
            {
        
                Expiry = expiry;
                IsEdible = isEdible;
                Heal = heal;
                Type = type;
                Boost = boost;
                Damage = 0;
                Max_expiry = expiry;
                Name = name;
            }
        
        protected Item (int expiry , int damage ,EnumsItem type, float boost = 1)
            {
                Damage = damage;
                Boost = boost;
                Expiry = expiry;
                IsEdible = false;
                Heal = 0;
                Type = type;
                Boost = boost;
            }
            
        
        public abstract void Update();
        
        public int GetBoost()
            {
                return (int) ( Boost);
            }
        
        public int GetExpiry()
            {
                return Expiry;
            }
        
        
        public bool GetIsEdible()
            {
                return IsEdible;
            }
            
        public int GetDamage()
            {
                return  (int) Damage;
            }
            
        public int GetHeal()
            {
                return (int) Heal;
            }
        
    
    }
}