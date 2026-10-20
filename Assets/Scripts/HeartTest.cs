using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartTest : MonoBehaviour
{
  public Image[] hearts;
  public Image redFlash;
  public GameOver gameOverManager;

  public int health = 3;
  public float flashDuration = 0.2f;

  private bool dead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (redFlash != null)
        {
          Color color = redFlash.color;
          color.a = 0f;
          redFlash.color = color;
        }
    }

  // Update is called once per frame
  void Update()
  {
    if (dead) return;
      //Damage tied to pressing "H" for now
      if (Input.GetKeyDown(KeyCode.H))
      {
        if (health > 0)
        {
          health--;

          if (hearts != null &&
              health < hearts.Length &&
              hearts[health] != null)
          {
            hearts[health].gameObject.SetActive(false);
          }
          //Screen Flash red when damage is taken
          if (redFlash != null)
          {
            StartCoroutine(FlashRed());
          }

          //Death "GameOver"
          if (health <= 0)
          {
            Die();
          }
        }
      }
  }

  IEnumerator FlashRed()
  {
    redFlash.gameObject.SetActive(true);

    Color color = redFlash.color;
    color.a = 0.5f;
    redFlash.color = color;

    yield return new WaitForSeconds(flashDuration);

    color.a = 0f;
    redFlash.color = color;
  }

  void Die()
  {
    dead = true;

    //Stop Moving if ded
    PlayerController playerController =
      GetComponent<PlayerController>();

    if (playerController != null)
      playerController.enabled = false;

    CharacterController characterController =
      GetComponent<CharacterController>();

    if (characterController != null)
      characterController.enabled = false;

    //Show GameOver Panel
    if (gameOverManager != null)
    {
      gameOverManager.ShowGameOver();
    }

    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }
}
