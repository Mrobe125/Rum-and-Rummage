using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
  public GameObject gameOverPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //Hide Panel
      if (gameOverPanel != null)
      {
        gameOverPanel.SetActive(false);
      }

      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

  public void ShowGameOver()
  {
    //Unlock Mouse
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    //Show Panel
    if (gameOverPanel != null)
    {
      gameOverPanel.SetActive(true);
    }
  }

  public void Retry()
  {
    //Reload button
    SceneManager.LoadScene(
      SceneManager.GetActiveScene().buildIndex);
  }

  public void QuitGame()
  {
    //Quit Game works for build only
    Application.Quit();
    Debug.Log("Quit Pressed");
  }
}
