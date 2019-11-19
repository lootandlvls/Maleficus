using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleficusVariables : MonoBehaviour
{
    /// <summary>
    /// Contains variable members.
    /// </summary>

    #region LocalSavedData
    // accessable variables after deserialization
    public static Local_Account user;
    public static List<Local_Spell> spells;
    public static List<Local_SinglePlayer> singleplayers;
    public static Local_Achievement achievements;
    #endregion
}
