using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance) Debug.LogError("GameManager is null");
            return instance;
        }
    }
    #endregion

    [SerializeField, Tooltip("The amount of options that will be instantiated")]
    private int optionAmount = 4;
    [SerializeField, Tooltip("Whether multiple of the same option are allowed to exist at once")]
    private bool repeatColours = true;

    [Space]

    [Header("UI")]
    [SerializeField, Tooltip("The text displaying the name of the colour to be found")]
    private TextMeshProUGUI targetText;
    [SerializeField, Tooltip("The transform of the parent holding all colour options")]
    private Transform optionGroup;
    [SerializeField, Tooltip("The prefab instantiated as a colour option")]
    private Transform optionPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Setup();
    }

    /// <summary>
    /// Causes a new target colour to be selected and updates the options
    /// </summary>
    private void Setup()
    {
        //Destroys all existing options
        for (int i = 0; i < optionGroup.childCount; i++)
            Destroy(optionGroup.GetChild(i).gameObject);

        //Turns the colours array from the ColourManager instance into a list
        //for easier referencing and sorting elements
        List<ColourManager.ColourInfo> colours = new List<ColourManager.ColourInfo>(ColourManager.instance.colours);

        //Selects a target colour
        ColourManager.ColourInfo targetColour = colours[RandomIndex(colours)];

        //Assigns the target colour value to the target text field
        targetText.color = targetColour.value;

        //Removes and re-adds the target colour to colours to make it the last index
        colours.Remove(targetColour);
        colours.Add(targetColour);

        //Creates a list containing all colour names besides the target colour name
        List<string> colourNames = new List<string>();
        for (int i = 0; i < colours.Count - 1; i++)
            colourNames.Add(colours[i].name);

        //Assigns a random colour name to the target text field besides the target colour name
        targetText.text = colourNames[RandomIndex(colourNames)];

        //Initialises a list of text fields for the names of each option's colour
        List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();

        //Instantiates all options and assigns their text fields to optionTexts
        for (int i = 0; i < optionAmount; i++)
            optionTexts.Add(Instantiate(optionPrefab, optionGroup).GetComponentInChildren<TextMeshProUGUI>());

        //Forgoes each option from having a different colours if there aren't enough
        if (!repeatColours && colours.Count < optionAmount)
        {
            repeatColours = true;
            Debug.LogWarning("There are less colours than there are options");
        }

        //Assigns each option with a name and button event
        for (int i = 0; i < optionAmount; i++)
        {
            //Finds the random index of the current option
            int currentOptionIndex = RandomIndex(optionTexts);

            //Finds the button of the current option
            Button optionButton = optionTexts[currentOptionIndex].transform.parent.GetComponentInChildren<Button>();

            //The first selected option is the correct answer
            if (i == 0)
            {
                //Sets the option button as the correct answer
                optionButton.onClick.AddListener(CorrectColour);

                //Sets the option text to the target colour's name
                optionTexts[currentOptionIndex].text = targetColour.name;
            }
            //All other options are incorrect answers
            else
            {
                //Sets the option button as an incorrect answer
                optionButton.onClick.AddListener(IncorrectColour);

                //Sets the option text to any colour's name besides the target's
                optionTexts[currentOptionIndex].text = colourNames[RandomIndex(colourNames)];

                //Prevents colour names from repeating among the options
                if (!repeatColours) colourNames.Remove(optionTexts[currentOptionIndex].text);
            }

            //Removes the option so it can't have its name and colour value assigned again
            optionTexts.RemoveAt(currentOptionIndex);
        }
    }

    /// <summary>
    /// Functionality occurning when the player selects the correct option
    /// </summary>
    private void CorrectColour()
    {
        Debug.Log("Correct answer");
        Setup();
    }

    /// <summary>
    /// Functionality occurning when the player selects the incorrect option
    /// </summary>
    private void IncorrectColour()
    {
        Debug.Log("Incorrect answer");
        Setup();
    }

    /// <summary>
    /// Picks a random index from a list
    /// </summary>
    /// <typeparam name="T">The type used in the list</typeparam>
    /// <param name="list">The list from which a random index will be returned</param>
    /// <returns></returns>
    private int RandomIndex<T>(List<T> list)
    {
        return Random.Range(0, list.Count);
    }
}
