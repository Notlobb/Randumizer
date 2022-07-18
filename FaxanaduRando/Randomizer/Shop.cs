using System.Collections.Generic;

namespace FaxanaduRando
{
    public class Shop
    {
        public enum Id
        {
            EolisKeyShop,
            EolisItemShop,
            ApoluneKeyShop,
            ApoluneSecretShop,
            ApoluneItemShop,
            ForepawKeyShop,
            ForepawItemShop,
            MasconKeyShop,
            MasconItemShop,
            MasconSecretShop,
            VictimKeyShop,
            VictimItemShop,
            ConflateKeyShop,
            ConflateItemShop,
            DaybreakKeyShop,
            DaybreakItemShop,
            DartmoorKeyShop,
            DartmoorItemShop,
        }

        public List<ShopItem> Items { get; set; } = new List<ShopItem>();

        public Id ShopId { get; set; }
        public int MaxPrice { get; set; } = 2000;
        public bool IsKeyShop { get; set; } = false;

        public Shop(Id id, bool keyShop = false)
        {
            ShopId = id;
            IsKeyShop = keyShop;
        }

        public bool HasWeaponOrSpell()
        {
            foreach (var item in Items)
            {
                if (ShopRandomizer.spellIds.Contains(item.Id))
                {
                    return true;
                }
                if (ShopRandomizer.weaponIds.Contains(item.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
