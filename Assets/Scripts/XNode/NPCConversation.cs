using TMPro;
using UnityEngine;
using XNode;
using UnityEngine.UI;

public class NPCConversation : MonoBehaviour
{
    [SerializeField] private DialogueGraph dialogue;
    [SerializeField] private TextMeshProUGUI spokenLine;
    [SerializeField] private Transform responseButtonPanel;
    [SerializeField] private GameObject buttonPrefab;
    void Start()
    {
       foreach(Node item in dialogue.nodes)
        {
            if(item is EntryNode)
            {
                dialogue.current = item.GetPort("exit").Connection.node as DialogueNodeBase;
                break;
            }
        }
        ParseNode();
    }

    private void ParseNode()
    {
        if (dialogue.current == null || spokenLine == null) return;

        switch (dialogue.current.GetDialogueType)
        {
            case "NPC":
                spokenLine.text = dialogue.current.dialogueSpoken;
                SpawnResponseButtons();
                break;
            case "Response":
                ClearButtons();
                NextNode("exit");
                break;
        }
    }

    public void NextNode(string fieldName)
    {
        foreach(NodePort port in dialogue.current.Ports)
        {
            if(port.fieldName == fieldName)
            {
                dialogue.current = port.Connection.node as DialogueNodeBase;
                break;
            }
        }
        ParseNode();
    }
    private void SpawnResponseButtons()
    {
        foreach(NodePort port in dialogue.current.Ports)
        {
            if(port.Connection == null || port.IsInput)
            {
                continue;
            }

            if(port.Connection.node is ResponseDialogue)
            {
                ResponseDialogue rd = port.Connection.node as ResponseDialogue;

                Button b = Instantiate(buttonPrefab, responseButtonPanel).GetComponent<Button>();
                b.onClick.AddListener(() => NextNode(port.fieldName));
                b.GetComponentInChildren<TextMeshProUGUI>().text = rd.dialogueSpoken.ToString();
            }
        }
    }


    private void ClearButtons()
    {
        Transform[] children = responseButtonPanel.GetComponentsInChildren<Transform>();

        for(int i = children.Length - 1; i >= 0; i--)
        {
            if (children[i] != responseButtonPanel)
            {
                Destroy(children[i].gameObject);
            }
        }
    }
}
