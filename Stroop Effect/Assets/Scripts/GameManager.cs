using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

    [HideInInspector]
    public UnityEvent onPlay = new UnityEvent();

    [SerializeField, Tooltip("The amount of options that will be instantiated")]
    private int optionAmount = 4;
    [SerializeField, Tooltip("Whether multiple of the same option are allowed to exist at once")]
    private bool repeatColours = true;

    [Space]

    [SerializeField, Tooltip("The amount of time until the potential score reaches its lowest")]
    private float roundTime = 3f;
    [SerializeField, Tooltip("Multiplied by the remaining time to get the score")]
    private int scoreMultiplier = 100;
    [SerializeField, Tooltip("Added to the score so it doesn't reach 0")]
    private int minScore = 100;

    private float timer = 0f;
    private int roundScore = 0;
    private int completedRounds = 0;

    [Space]

    [SerializeField, Tooltip("The text displaying the name of the colour to be found")]
    private TextMeshProUGUI targetText;
    [SerializeField, Tooltip("The text displaying which round it is")]
    private TextMeshProUGUI roundText;
    [SerializeField, Tooltip("The text display the total score of all rounds so far")]
    private TextMeshProUGUI totalScoreText;
    [SerializeField, Tooltip("The text displaying the score of the current round")]
    private TextMeshProUGUI roundScoreText;
    [SerializeField, Tooltip("The transform of the image displaying the time left this round")]
    private Transform timerImage;
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
        onPlay.AddListener(Setup);
    }

    private void Update()
    {
        //Round timer goes down until it reaches 0 and scales the timer image with it
        if (timer < 0) timer = 0;
        else if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timerImage) timerImage.localScale = new Vector3(timer / roundTime, 1f, 1f);
        }

        //Sets the round score based on the timer and displays it in the UI
        roundScore = Mathf.RoundToInt(timer * 10) * scoreMultiplier + minScore;
        if (roundScoreText) roundScoreText.text = roundScore.ToString();
    }

    /// <summary>
    /// Runs when a new round is starts
    /// </summary>
    private void Setup()
    {
        //Updates the amount of rounds completed and displays it in the UI
        completedRounds++;
        if (roundText) roundText.text = $"Round {completedRounds.ToString()}";

        //Updates the displayed score
        if (totalScoreText) totalScoreText.text = GameInfo.score.ToString();

        //Resets the timer
        timer = roundTime;

        //Destroys all existing options
        for (int i = 0; i < optionGroup.childCount; i++)
            Destroy(optionGroup.GetChild(i).gameObject);

        //Turns the colours array from the ColourManager instance into a list
        //for easier referencing and sorting elements
        List<ColourManager.ColourInfo> colours = new List<ColourManager.ColourInfo>(ColourManager.instance.colours);

        //Selects a target colour
        ColourManager.ColourInfo targetColour = colours[RandomIndex(colours)];

        //Assigns the target colour value to the target text field
        if (targetText) targetText.color = targetColour.value;

        //Removes and re-adds the target colour to colours to make it the last index
        colours.Remove(targetColour);
        colours.Add(targetColour);

        //Creates a list containing all colour names besides the target colour name
        List<string> colourNames = new List<string>();
        for (int i = 0; i < colours.Count - 1; i++)
            colourNames.Add(colours[i].name);

        //Assigns a random colour name to the target text field besides the target colour name
        if (targetText) targetText.text = colourNames[RandomIndex(colourNames)];

        //Initialises a list of text fields for the names of each option's colour
        List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();

        //Instantiates all options and assigns their text fields to optionTexts
        for (int i = 0; i < optionAmount; i++)
            if (optionPrefab && optionGroup)
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
                //Sets the option text to the target colour's name
                optionTexts[currentOptionIndex].text = targetColour.name;

                //Sets the option button as the correct answer
                optionButton.onClick.AddListener(CorrectColour);
            }
            //All other options are incorrect answers
            else
            {
                //Sets the option text to any colour's name besides the target's
                optionTexts[currentOptionIndex].text = colourNames[RandomIndex(colourNames)];

                //Sets the option button as an incorrect answer
                optionButton.onClick.AddListener(IncorrectColour);

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
        //Updates the score and number of correct answers
        GameInfo.score += roundScore;
        GameInfo.correctAnswers++;

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
