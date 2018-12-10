using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMann
{
    class Item
    {
        #region Enums
        public enum ItemType
        {
            generic,
            armor,
            weapon,
            magic,
            spellbook
        }
        #endregion

        #region Fields
        private string _name;
        private ItemType _itemType;
        private string _damDesc;
        
        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public ItemType itemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }
        public string damDesc
        {
            get { return _damDesc; }
            set { _damDesc = value; }
        }
        #endregion
    }
}
