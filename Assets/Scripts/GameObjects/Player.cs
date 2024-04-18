using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {
    private ElementContainer elementContainer;
    private Notebook notebook;
    private DialogueTrigger dialogueTrigger;
    private DialogueManager dialogueManager;
    private UIManager uiManager;
    private CluesManager cluesManager;
    private LevelManager levelManager;


    private float horizontal;
    public float speed = 6f;
    public Rigidbody2D rb;
    private float screenLimitLeft = -9f;
    private float screenLimitRight = 9f;
    private bool inTrigger = false;
    private string otherName = "";
    private string triggerType;
    private bool haltMovement = false;
    public GameObject PopUp;
   

    void Start() {
        elementContainer = GameObject.Find("Element Container").GetComponent<ElementContainer>();

        notebook = elementContainer.notebook;
        cluesManager = elementContainer.cluesManager;
        dialogueTrigger = elementContainer.dialogueTrigger;
        dialogueManager = this.GetComponent<DialogueManager>();
        uiManager = elementContainer.uiManager;
        levelManager = elementContainer.levelManager;
    }

    void Update() {
        if (dialogueManager.IsDialoguing || notebook.IsOpen || uiManager.modalOpen) {
            haltMovement = true;
        } else { haltMovement = false; }

        if (!haltMovement) {
            horizontal = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontal * speed, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            if (!dialogueManager.IsDialoguing && !uiManager.modalOpen) {
                notebook.ToggleNotebook();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && inTrigger && !dialogueManager.IsDialoguing && !uiManager.modalOpen) {            
            if (triggerType == "NPC") {
                dialogueTrigger.StartDialogue();
            }

            else if (triggerType == "Item") {
                CluesManager.Item item = cluesManager.FindItem(otherName);
                string info = "\n" + item.itemName + "\n" + item.description + "\n" + item.keyword;

                uiManager.CreateSimpleModal("Você coletou " + item.itemName + info, "Item pego!");
                cluesManager.CollectItem(item);
            }
        }

        // Verificação para troca de cenas
        if (transform.position.x >= screenLimitRight) {
            if (!string.IsNullOrEmpty(levelManager.currentLevel.rightName)) {
                levelManager.ExitRight();
            }

            // Impedir de ir mais para a direita
        }

        if (transform.position.x <= screenLimitLeft) {
            if (!string.IsNullOrEmpty(levelManager.currentLevel.leftName)) {
                levelManager.ExitLeft();
            }
            
            // Impedir de ir mais para a esquerda
        }
        
        // Verificar também se está no trigger para tal.
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            if (!string.IsNullOrEmpty(levelManager.currentLevel.upName)) {
                levelManager.ExitUp();
            }
        }

        // Verificar também se está no trigger para tal.
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            if (!string.IsNullOrEmpty(levelManager.currentLevel.downName)) {
                levelManager.ExitDown();
            }
        }

        if (inTrigger == true)
        {
            PopUp.SetActive(true);
        }

        else 
        {
            PopUp.SetActive(false);
        }


    }
    
    // Refazer, isso é digno de r/programminghorror
    private void OnTriggerEnter2D(Collider2D other) {
        triggerType = other.tag;
        inTrigger = true;
        otherName = other.gameObject.name;
    }

    private void OnTriggerExit2D(Collider2D other) {
        triggerType = null;
        inTrigger = false;
        otherName = "";

        // solução temporária. acho que não tem jeito, vou ter que refazer essa parte.
        if (triggerType == "Item") {
            Destroy(other.gameObject);
        }

        // while ontriggerstay AND ontriggerexit não chamado?
        // não tenho uma solução ainda.
    }
}