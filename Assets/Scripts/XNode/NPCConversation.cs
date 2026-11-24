
using TMPro;
using UnityEngine;
using XNode;
using UnityEngine.UI;

public class NPCConversation : MonoBehaviour
{
  private DialogueGraph dialogue;

  [SerializeField] private TextMeshProUGUI spokenLine;
  [SerializeField] private Transform responseButtonPanel;
  [SerializeField] private GameObject buttonPrefab;

  [SerializeField] private Image panelImage;

  [Header("Gold System")]
  [SerializeField] private TreasureCount treasureCount;

  public void StartConversation(DialogueGraph graph)
  {
    dialogue = graph;

    foreach (Node item in dialogue.nodes)
    {
      if (item is EntryNode)
      {
        dialogue.current = item.GetPort("exit").Connection.node
            as DialogueNodeBase;
        break;
      }
    }

    ClearButtons();
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

        ResponseDialogue response =
            dialogue.current as ResponseDialogue;

        if (response != null)
        {
          // Check if this response requires gold.
          if (response.isPurchase)
          {
            if (treasureCount == null)
            {
              Debug.LogError("TreasureCount reference missing!");
              return;
            }

            // Attempt to spend the required gold.
            if (!treasureCount.TrySpendGold(response.goldCost))
            {
              // Player cannot afford the purchase.
              NextNode("insufficientFunds");
              return;
            }

            Debug.Log("Purchase successful!");
          }

          // Update quest image if available.
          if (response.questImage != null && panelImage != null)
          {
            panelImage.sprite = response.questImage;
          }
        }

        NextNode("exit");
        break;
    }
  }

  public void NextNode(string fieldName)
  {
    foreach (NodePort port in dialogue.current.Ports)
    {
      if (port.fieldName == fieldName)
      {
        dialogue.current = port.Connection.node
            as DialogueNodeBase;
        break;
      }
    }

    ParseNode();
  }

  private void SpawnResponseButtons()
  {
    foreach (NodePort port in dialogue.current.Ports)
    {
      if (port.Connection == null || port.IsInput)
        continue;

      if (port.Connection.node is ResponseDialogue)
      {
        ResponseDialogue rd =
            port.Connection.node as ResponseDialogue;

        Button b = Instantiate(buttonPrefab, responseButtonPanel)
            .GetComponent<Button>();

        b.onClick.AddListener(() => NextNode(port.fieldName));

        b.GetComponentInChildren<TextMeshProUGUI>().text =
            rd.dialogueSpoken;
      }
    }
  }

  private void ClearButtons()
  {
    Transform[] children =
        responseButtonPanel.GetComponentsInChildren<Transform>();

    for (int i = children.Length - 1; i >= 0; i--)
    {
      if (children[i] != responseButtonPanel)
      {
        Destroy(children[i].gameObject);
      }
    }
  }
}

