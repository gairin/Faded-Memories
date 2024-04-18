using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;

public class SentenceSlots : MonoBehaviour {
    private Image image;private ElementContainer elementContainer;
    private InputModalWindow inputModalWindow;
    private UIManager uiManager;
    private int i;
    private List<string> answers;
    private string jsonFilePath = "Assets/GameData/SlotsAnswers.json";
    private string answer;
    private EventTrigger trigger;
    
    public void Start() {
        elementContainer = GameObject.Find("Element Container").GetComponent<ElementContainer>();
        uiManager = elementContainer.uiManager;

        image = transform.GetChild(0).GetComponent<Image>();

        i = int.Parse(gameObject.name.Replace("Space ", ""));

        answers = LoadAnswers(jsonFilePath);

        answer = answers[i - 1];

        trigger = image.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnPointerEnter(); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnPointerExit(); });
        trigger.triggers.Add(entryExit);

        EventTrigger.Entry entryClick = new EventTrigger.Entry();
        entryClick.eventID = EventTriggerType.PointerClick;
        entryClick.callback.AddListener((data) => { OnPointerClick(); });
        trigger.triggers.Add(entryClick);
    }

    private List<string> LoadAnswers(string path) {
        List<string> loadedAnswers = new List<string>();
        
        string jsonData = File.ReadAllText(path);
        WordsData wordsData = JsonUtility.FromJson<WordsData>(jsonData);
        loadedAnswers.AddRange(wordsData.answers);

        return loadedAnswers;
    }

    private void OnPointerEnter() {
        image.color = new Color32(255, 255, 100, 255);
    }

    private void OnPointerExit() {
        image.color = new Color32(190, 190, 190, 255);
    }

    private void OnPointerClick() {
        inputModalWindow = uiManager.CreateInputModal("Preencha o espaço vazio", "Preencher");
        inputModalWindow.SetInputField((inputResult) => { OnInputDone(inputResult); });
    }

    private void OnInputDone(string inputResult) {
        if (inputResult == answer) {
            GameObject textObject = new GameObject("Answer " + i.ToString());
        
            TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();

            textComponent.text = answer;
            textComponent.fontSize = 24;
            textComponent.color = Color.black;
            textComponent.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF"); // Carrega uma fonte para o TMPro
            
            RectTransform rectTransform = textObject.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(200f, 50f);
            rectTransform.localPosition = new Vector3(35f, -6f, 0f);
            rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            textObject.transform.SetParent(transform, false);

            trigger.triggers.Clear();
        }
    }

    [System.Serializable]
    public class WordsData {
        public string[] answers;
    }
}