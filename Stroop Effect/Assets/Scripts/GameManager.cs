using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("The amount of options that will be instantiated")]
    private int optionAmount = 4;

    [Space]

    [Header("UI")]
    [SerializeField, Tooltip("The text displaying the name of the colour to be found")]
    private TextMeshProUGUI targetText;
    [SerializeField, Tooltip("The transform of the parent holding all colour options")]
    private Transform optionGroup;
    [SerializeField, Tooltip("The prefab instantiated as a colour option")]
    private Transform optionPrefab;

    private void Start()
    {
        //Calls set up once the ColourManager instance has initialised
        ColourManager.instance.onColourManagerInit.AddListener(Setup);
    }

    private void Setup()
    {
        //Destroys all existing options
        for (int i = 0; i < optionGroup.childCount; i++)
            Destroy(optionGroup.GetChild(i).gameObject);

        //Initialises a list of colourDict's keys for referencing indexes
        List<string> colourKeys = new List<string>(ColourManager.instance.colourDict.Keys);
        //A random element of colourKeys is selected as the target colour
        targetText.text = colourKeys[RandomIndex(colourKeys)];
        //Copy of colourKeys but missing the target name
        List<string> exclusiveColourKeys = new List<string>(colourKeys);
        exclusiveColourKeys.Remove(targetText.text);

        //Initialises a list of text fields for the names of each option's colour
        List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();

        //Instantiates all options and assigns their text fields to optionTexts
        for (int i = 0; i < optionAmount; i++)
            optionTexts.Add(Instantiate(optionPrefab, optionGroup).GetComponentInChildren<TextMeshProUGUI>());

        //Assigns each option with a name and colour
        for (int i = 0; i < optionAmount; i++)
        {
            //Finds the random index of the current option
            int currentOptionIndex = RandomIndex(optionTexts);

            //The first selected option has the correct colour but can't have the corresponding name
            if (i == 0)
            {
                //The target colour
                optionTexts[currentOptionIndex].color = ColourManager.instance.colourDict[targetText.text];
                //Any name except the target name
                string key = exclusiveColourKeys[RandomIndex(exclusiveColourKeys)];
                optionTexts[currentOptionIndex].text = key;
            }
            //All other options have a random colour that isn't the correct one amd a random name
            else
            {
                //Any colour except the target colour
                string key = exclusiveColourKeys[RandomIndex(exclusiveColourKeys)];
                optionTexts[currentOptionIndex].color = ColourManager.instance.colourDict[key];
                //Random name
                optionTexts[currentOptionIndex].text = colourKeys[RandomIndex(colourKeys)];
            }

            //Removes the option so it can't have its name and colour assigned again
            optionTexts.RemoveAt(currentOptionIndex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <param name="list">The list from which a random index will be returned</param>
    /// <returns></returns>
    private int RandomIndex<T>(List<T> list)
    {
        return Random.Range(0, list.Count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <param name="list">The list from which a random index will be returned</param>
    /// <param name="exceptions">An array of elements not to be </param>
    /// <returns></returns>
    private int RandomIndex<T>(List<T> list, T[] exceptions)
    {
        List<T> exceptionList = new List<T>(list);
        for (int i = 0; i < exceptions.Length; i++)
            exceptionList.Remove(exceptions[i]);

        return Random.Range(0, exceptionList.Count);
    }
}
