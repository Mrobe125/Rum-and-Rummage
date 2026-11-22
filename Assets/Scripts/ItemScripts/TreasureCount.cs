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

    while (gold < targetGold)
    {
      gold++;
      UpdateGoldText();

      yield return new WaitForSeconds(0.02f);
    }
    counting = false;
  }

  public bool CanAfford(int amount)
  {
    return gold >= amount;
  }

  public bool TrySpendGold(int amount)
  {
    if (gold < amount || counting)
    {
      return false;
    }

    StartCoroutine(SpendTreasure(amount));
    return true;
  }

  private IEnumerator SpendTreasure(int amount)
  {
    counting = true;

    int targetGold = gold - amount;
    while (gold > targetGold)
    {
      gold--;
      UpdateGoldText();

      yield return new WaitForSeconds(0.02f);
    }
    counting = false;
    Debug.Log("Gold Spent Ya Broke");
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
