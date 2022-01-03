using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColourManager : MonoBehaviour
{
    public UnityEvent onColourManagerInit = new UnityEvent();

    //An instance of Colour Manager for easy references
    public static ColourManager instance;

    [SerializeField, Tooltip("The name and corresponding colour value of all possible target colours")]
    private ColourInfo[] colours;

    //Holds the name and colour value of a potential target colour
    [System.Serializable]
    private struct ColourInfo
    {
        public string name;
        public Color colour;
    }

    //Dictionary pairs names with their colour values for easy referencing
    public Dictionary<string, Color> colourDict = new Dictionary<string, Color>();

    private void Awake()
    {
        //Singleton
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }

    private void Start()
    {
        //Initialise
        Init();
    }

    /// <summary>
    /// Initialises the ColourManager instance
    /// </summary>
    private void Init()
    {
        //Returns an error when there aren't enough colours to proceed
        if (colours.Length <= 1)
        {
            Debug.LogError("At least two colours must be assigned");
            return;
        }

        //All the target colours from the colours array are added to colourDict 
        for (int i = 0; i < colours.Length; i++)
        {
            colourDict.Add(colours[i].name, colours[i].colour);
        }

        //Invoke UnityEvent marking the end of initialisation
        onColourManagerInit.Invoke();
    }

    /// <summary>
    /// Determines whether optionColour is the same as the target colour
    /// </summary>
    /// <param name="targetName">Key of the target colour</param>
    /// <param name="optionColour">Colour compared against the target colour</param>
    /// <returns></returns>
    public bool CompareColours(string targetName, Color optionColour)
    {
        if (colourDict.ContainsKey(targetName) && colourDict[targetName] == optionColour)
        {
            return true;
        }
        return false;
    }
}