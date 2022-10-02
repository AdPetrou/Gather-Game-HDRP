using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Inventory.Scriptables 
{
    [System.Serializable]
    public struct LootDistribution
    {
        public ItemClass item;
        public int distribution;

        public LootDistribution(ItemClass _item, int _distribution)
        {
            item = _item;
            distribution = _distribution;
        }
    }

    [CreateAssetMenu(fileName = "LootTable")]
    public class LootTable : ScriptableObject
    {
        #region Variables
        public LootDistribution[] inputTable;
        private LootDistribution[] lootTable;
        public LootDistribution[] accessTable 
        {
            get
            {
                if(lootTable == null)
                {
                    lootTable = setLootTable();
                }

                return lootTable;
            } 

            private set
            {
                lootTable = value;
            } 
        }
        #endregion

        #region Unity Functions
        private void OnValidate()
        {
            lootTable = setLootTable();
        }

        private void Awake()
        {
            lootTable = setLootTable();
        }
        #endregion

        #region Methods
        public LootDistribution[] setLootTable()
        {
            LootDistribution[] tempTable = new LootDistribution[inputTable.Length];
            tempTable[0] = inputTable[0];

            for (int i = 1; i < inputTable.Length; i++)
                tempTable[i] = new LootDistribution(inputTable[i].item, inputTable[i].distribution + tempTable[i - 1].distribution);

            return tempTable;
        }

        public ItemClass getRandomItem()
        {
            int index = getRandomNumber();
            for(int i = 0; i < accessTable.Length; i++)
                if (index <= accessTable[i].distribution)
                    return accessTable[i].item;

            return null;
        }

        public int getRandomNumber()
        {
            return Random.Range(0, accessTable[accessTable.Length - 1].distribution);
        }
        #endregion
    }
}
