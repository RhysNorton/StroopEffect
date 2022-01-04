using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    #region Singleton
    public static ColourManager instance;
    public static ColourManager Instance
    {
        get
        {
            if (!instance) Debug.LogError("ColourManager is null");
            return instance;
        }
    }
    #endregion

    [SerializeField, Tooltip("The name and corresponding colour value of all possible target colours")]
    public ColourInfo[] colours;

    //Holds the name and colour value of a potential target colour
    [System.Serializable]
    public struct ColourInfo
    {
        public string name;
        public Color value;
    }

    private void Awake()
    {
        instance = this;
    }
}