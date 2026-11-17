using UnityEngine;
using TMPro;
using System.Collections;

public class TreasureCount : MonoBehaviour
{
  public TextMeshProUGUI goldText;

  private int gold = 0;
  private bool collected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      UpdateGoldText();
    }

    // Update is called once per frame
    void UpdateGoldText()
    {
        goldText.text = gold.ToString("000");
    }
  // Touch bag detected
  private void OnTriggerEnter(Collider other)
  {
    if (collected) return;

    if (other.CompareTag("Player"))
    {
      collected = true;

      // Hiding/Disable these for now to continue count
      GetComponent<Renderer>().enabled = false;
      GetComponent<Collider>().enabled = false;

      StartCoroutine(CountTreasure());
    }
  }

  IEnumerator CountTreasure()
  {
    int targetGold = gold + 100;

    while (gold < targetGold)
    {
      gold++;
      UpdateGoldText();

      yield return new WaitForSeconds(0.02f);
    }
  }
}
