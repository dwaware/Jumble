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
    private List<string> word = new List<string>();
    public string currentGuess { get; set; }
    public bool isSolved { get; set; }

    void Start()
    {
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
            myTile.transform.SetParent(canvasMain.transform,false);
            myTile.transform.localPosition = new Vector3(-450 + i * 150, 0, 0);
            myTile.name = "Tile_" + i;

            Tile tileScript = myTile.GetComponent<Tile>();
            tileScript.origPos = myTile.transform.localPosition;
            tileScript.tileIndex = i;

            Text myText = myTile.GetComponentInChildren<Text>();
            myText.text = scrambledWord[i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ExitGame();
        }
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
        TextAsset wordList = (TextAsset)Resources.Load("words7", typeof(TextAsset));
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
            }
        }

        return isSolved;
    }
}