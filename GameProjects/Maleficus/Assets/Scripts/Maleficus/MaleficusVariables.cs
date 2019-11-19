using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maleficus
{
    /// <summary>
    /// Contains variable members.
    /// </summary>
    public static class MaleficusVariables
    {
        #region LocalSavedData
        // accessable variables after deserialization
        public static Local_Account user;
        public static List<Local_Spell> saved_spells;
        public static List<Local_SinglePlayer> singleplayers;
        public static Local_Achievement achievements;
        #endregion
    }
}