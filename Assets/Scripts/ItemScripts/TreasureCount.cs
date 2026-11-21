using UnityEngine;
using TMPro;
using System.Collections;

public class TreasureCount : MonoBehaviour
{
  public TextMeshProUGUI goldText;

  private int gold = 0;
  private bool counting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      UpdateGoldText();
    }

  public void AddGold(int amount)
  {
    if (!counting)
    {
      StartCoroutine(CountTreasure(amount));
    }
  }

  private IEnumerator CountTreasure(int amount)
  {
    counting = true;
    int targetGold = gold + amount;
    
    while (gold  < targetGold)
    {
      gold++;
      UpdateGoldText();

      yield return new WaitForSeconds(0.02f);
    }
    counting = false;
  }

    // Update is called once per frame
    private void UpdateGoldText()
    {
      if (goldText != null)
      {
        goldText.text = gold.ToString("000");
      }
    }
}
