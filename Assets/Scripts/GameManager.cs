using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<GameObject> holder = new List<GameObject>();
    public List<GameObject> tile = new List<GameObject>();
    public List<GameObject> hint = new List<GameObject>();
    private List<string> word = new List<string>();
    public string currentGuess { get; set; }
    public bool isSolved { get; set; }
    public bool displayHint { get; set; }

    public enum Difficulty { Easy, Normal, Hard };
    private Difficulty _difficulty;

    public Difficulty CurrentDifficulty
    {
        get { return _difficulty; }
        set { _difficulty = value; }
    }

    public void OnSliderValueChanged(float value)
    {
        Debug.Log("On slider value changed!!!!!!!!!!!!!!!!");
        float sliderDifficulty = GameObject.Find("Slider_Difficulty").GetComponent<Slider>().value;
        CurrentDifficulty = (Difficulty)sliderDifficulty;
        Debug.Log("Current Difficulty:  "+CurrentDifficulty);

        PlayerPrefs.SetInt("Player Difficulty", (int)CurrentDifficulty);
        Debug.Log("Writing Difficulty to prefs as:  " + (int)CurrentDifficulty);

        GameObject.Find("Text_Handle").GetComponent<Text>().text = CurrentDifficulty.ToString();

        //GameObject.Find("Text_Handle").GetComponent<Text>().text = CurrentDifficulty;
    }

    void Start()
    {
        Debug.Log(" ### ### ### GAME START ### ### ###");

        Slider sliderDifficulty = GameObject.Find("Slider_Difficulty").GetComponent<Slider>();

        int difficultyAsInt = PlayerPrefs.GetInt("Player Difficulty");
        CurrentDifficulty = (Difficulty)difficultyAsInt;
        Debug.Log("Difficulty from prefs:  " + CurrentDifficulty);

        sliderDifficulty.value = (float)difficultyAsInt;

        GameObject.Find("Text_Handle").GetComponent<Text>().text = CurrentDifficulty.ToString();

        GameObject canvasMain = GameObject.Find("Canvas_Main");

        Transform textYouWin = canvasMain.transform.Find("Text_YouWin");
        Text textYW = textYouWin.GetComponent<Text>();
        textYW.enabled = false;

        Transform textInst = canvasMain.transform.Find("Text_Instructions");
        Text textI= textInst.GetComponent<Text>();
        textI.enabled = true;

        readWordFromFile();

        foreach (string candidate in word)
        {
            Debug.Log("UNscrambled candidate:  " + candidate);
        }

        string scrambledWord = scrambleWord();
        isSolved = false;
        Debug.Log("Setting isSolved to " + isSolved + " <<<");

        for (int i = 0; i < word[0].Length; i++)
        {
            GameObject myHolder = Instantiate(holder[i]);
            myHolder.transform.SetParent(canvasMain.transform, false);
            myHolder.transform.localPosition = new Vector3(-450 + i * 150, 0, 0);
            myHolder.name = "Holder_" + i;
        }

        for (int i = 0; i < word[0].Length; i++)
        {
            GameObject myTile = Instantiate(tile[i]);

            Tile tileScript = myTile.GetComponent<Tile>();
            tileScript.tileScale = new Vector3(1, 1, 1);
            Debug.Log(tileScript.tileScale);
            tile[i].transform.localScale = tileScript.tileScale;

            myTile.transform.SetParent(canvasMain.transform,false);
            myTile.transform.localPosition = new Vector3(-450 + i * 150, 0, 0);
            myTile.name = "Tile_" + i;

            tileScript.origPos = myTile.transform.localPosition;
            tileScript.tileIndex = i;

            Text myText = myTile.GetComponentInChildren<Text>();
            myText.text = scrambledWord[i].ToString();
        }
    }

    private void AllowTileInteraction(bool _interact)
    {
        foreach (GameObject individualTile in tile)
        {
            DragNDrop dndScript = individualTile.GetComponent<DragNDrop>();
            dndScript.enabled = _interact;
        }
        Debug.Log("Is tile drag and drop enabled:  " + _interact);
    }

    private string scrambleWord()
    {
        string scrambledWord = word[0];
        Random.InitState((int)System.DateTime.Now.Ticks);
        int charactersInPlace = scrambledWord.Length;
        int attempts = 0;
        int maxAttempts = 1000;
        while (charactersInPlace > 1 && attempts < maxAttempts && word.Contains(scrambledWord))
        {
            scrambledWord = new string(word[0].ToCharArray().OrderBy(s => (Random.Range(0, 2) % 2) == 0).ToArray());
            charactersInPlace = calculateCharactersInPlace(scrambledWord);
            attempts++;
            //Debug.Log("Attempts:  "+attempts+"  Characters in place:"+charactersInPlace);
        }

        currentGuess = scrambledWord;
        Debug.Log("  scrambled tiles:  "+scrambledWord);
        return scrambledWord;
    }

    private int calculateCharactersInPlace(string _word)
    {
        int charactersInPlace = 0;
        for (int i = 0; i < _word.Length; i++)
        {
            if (_word[i] == word[0][i])
            {
                charactersInPlace++;
            }
        }

        return charactersInPlace;
    }

    private void readWordFromFile()
    {
        string _word = "";
        Debug.Log("about to read word file using difficulty:  " + CurrentDifficulty);
        TextAsset wordList = (TextAsset)Resources.Load("words7_"+CurrentDifficulty, typeof(TextAsset));
        Debug.Log("Using word list:  " + wordList.name);
        string[] lines = (wordList.text.Split('\n'));
        int index = Random.Range(0, lines.Length);
        _word = lines[index];
        _word= _word.Replace("\r", "").Replace("\n", "");

        char[] sortedWordAsCharacters = _word.OrderBy(c => c).ToArray();
        string sortedWord = new string(sortedWordAsCharacters);
        //Debug.Log("Sorted word:  " + sortedWord);

        int candidates = 0;
        for (int i = 0; i < lines.Length; i++ )
        {
            string candidate = lines[i];
            candidate = candidate.Replace("\r", "").Replace("\n", "");
            char[] sortedCandidateAsCharacters = candidate.OrderBy(c => c).ToArray();
            string sortedCandidate = new string(sortedCandidateAsCharacters);
            if (sortedWord == sortedCandidate)
            {
                Debug.Log("Candidate:  " + candidate);
                candidates++;
                word.Add(candidate);
            }
        }
        Debug.Log("Total candidates for solution:  " + candidates);
    }

    public void SceneReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public bool isJumbleSolved()
    {
        isSolved = false;
        foreach (string currentTiles in word)
        {
            if (currentTiles == currentGuess)
            {
                isSolved = true;

                GameObject canvasMain = GameObject.Find("Canvas_Main");

                Transform textYouWin = canvasMain.transform.Find("Text_YouWin");
                Text textYW = textYouWin.GetComponent<Text>();
                textYW.enabled = true;

                Transform textInst = canvasMain.transform.Find("Text_Instructions");
                Text textI = textInst.GetComponent<Text>();
                textI.enabled = false;

                Debug.Log("Setting isSolved to " + isSolved + " <<<");
            }
        }

        return isSolved;
    }
}