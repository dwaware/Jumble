using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragNDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        this.gameObject.transform.SetAsLastSibling();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        GameObject canvasMain = GameObject.Find("Canvas_Main");        
        float scaleFactor = canvasMain.transform.localScale.x;
        rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            //Debug.Log("-------DROPPED-------");
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            Tile thisScript = this.GetComponent<Tile>();
            GameObject droppedObject = eventData.pointerDrag;
            Tile droppedObjectScript = droppedObject.GetComponent<Tile>();

            Text droppedText = droppedObject.GetComponentInChildren<Text>();
            Text thisText = this.GetComponentInChildren<Text>();

            int droppedIndex = droppedObjectScript.tileIndex;
            //Debug.Log("droppedIndex:  " + droppedIndex);
            int targetIndex = thisScript.tileIndex;
            //Debug.Log("targetIndex:  " + targetIndex);

            droppedObjectScript.tileIndex = targetIndex;
            thisScript.tileIndex = droppedIndex;

            GameObject canvasMain = GameObject.Find("Canvas_Main");
            GameManager gmScript = canvasMain.GetComponent<GameManager>();
            //Debug.Log("Current guess before:  "+gmScript.currentGuess);

            //Debug.Log("dropped text:  "+droppedText.text.ToString());
            //Debug.Log("target  text:  "+thisText.text.ToString());

            string newGuess = gmScript.currentGuess;

            newGuess = newGuess.Remove(targetIndex, 1);
            newGuess = newGuess.Insert(targetIndex, droppedText.text.ToString());

            newGuess = newGuess.Remove(droppedIndex, 1);
            newGuess = newGuess.Insert(droppedIndex, thisText.text.ToString());

            gmScript.currentGuess = newGuess;

            //Debug.Log("Current guess  after:  " + gmScript.currentGuess);

            this.transform.localPosition = droppedObjectScript.origPos;
            thisScript.origPos = this.rectTransform.localPosition;

            droppedObjectScript.origPos = droppedObject.transform.localPosition;

            gmScript.isJumbleSolved();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");

        Tile thisScript = this.GetComponent<Tile>();
        this.transform.localPosition = thisScript.origPos;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }
}
