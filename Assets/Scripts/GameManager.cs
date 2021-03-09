using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> holder = new List<GameObject>();
    public List<GameObject> tile = new List<GameObject>();
    private string word;
    public string currentGuess { get; set; }
    public bool isSolved;

    void Start()
    {
        GameObject canvasMain = GameObject.Find("Canvas_Main");

        Transform textYouWin = canvasMain.transform.Find("Text_YouWin");
        Text textYW = textYouWin.GetComponent<Text>();
        textYW.enabled = false;

        Transform textInst = canvasMain.transform.Find("Text_Instructions");
        Text textI= textInst.GetComponent<Text>();
        textI.enabled = true;

        word = readWordFromFile();
        Debug.Log("UNscrambled:  " + word);
        string scrambledWord = scrambleWord();
        isSolved = false;

        for (int i = 0; i < word.Length; i++)
        {
            GameObject myHolder = Instantiate(holder[i]);
            myHolder.transform.SetParent(canvasMain.transform, false);
            myHolder.transform.localPosition = new Vector3(-450 + i * 150, 0, 0);
            myHolder.name = "Holder_" + i;
        }

        for (int i = 0; i < word.Length; i++)
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
        string scrambledWord = word;
        Random.InitState((int)System.DateTime.Now.Ticks);
        int charactersInPlace = scrambledWord.Length;
        int attempts = 0;
        int maxAttempts = 1000;
        while (charactersInPlace > 1 && attempts < maxAttempts)
        {
            scrambledWord = new string(word.ToCharArray().OrderBy(s => (Random.Range(0, 2) % 2) == 0).ToArray());
            charactersInPlace = calculateCharactersInPlace(scrambledWord);
            attempts++;
            //Debug.Log("Attempts:  "+attempts+"  Characters in place:"+charactersInPlace);
        }

        currentGuess = scrambledWord;
        //Debug.Log("  scrambled:  "+scrambledWord);
        return scrambledWord;
    }

    private int calculateCharactersInPlace(string _word)
    {
        int charactersInPlace = 0;
        for (int i = 0; i < _word.Length; i++)
        {
            if (_word[i] == word[i])
            {
                charactersInPlace++;
            }
        }

        return charactersInPlace;
    }

    private string readWordFromFile()
    {
        TextAsset wordList = (TextAsset)Resources.Load("words7", typeof(TextAsset));
        string[] lines = (wordList.text.Split('\n'));
        int index = Random.Range(0, lines.Length);
        word = lines[index];
        word = word.ToUpper();
        word = word.Replace("\r", "").Replace("\n", "");

        return word;
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
        if (word == currentGuess)
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
        else
        {
            isSolved = false;
        }

        return isSolved;
    }
}