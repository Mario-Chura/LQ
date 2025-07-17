
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance { get; private set; }

    [Header("Game Data")]
    public static bool isPaused = false;
    public static bool isGameOver = false;

    [Header("Player Data")]
    public GameObject player;
    public PlayerInventory playerInventory;
    public ObjectThrowing playerObjectThrowing;

    [Header("Enemy Data")]
    public GameObject enemy;
    public EnemyController enemyController;

    [Header("Canvas")]
    public ControladorCanvas controladorCanvas;
    public GameObject pantallaPausa, pantallaPerdiste, pantallaGanaste;
    public AudioClipData winAudio;
    public AudioClipData gameOverAudio;
    
    void Awake()
    {
        // Implementacion del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene el GameManager entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        isPaused = false;
        isGameOver = false;
    }
    public void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowAndUnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");

        Time.timeScale = 0.0f; //para pausar el juego

        isPaused = true;

        ShowAndUnlockCursor();

        // show pause screen
        pantallaPausa.SetActive(true);

        // stop enemy animations
        enemyController.PauseAnimations();
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");

        Time.timeScale = 1.0f; //para reanudar el juego

        isGameOver = false;
        isPaused = false;

        // hide and lock cursor
        HideAndLockCursor();

        // hide pause screen
        pantallaPausa.SetActive(false);

        // resume enemy animations
        enemyController.ResumeAnimations();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");

        isGameOver = true;
        isPaused = true;

        ShowAndUnlockCursor();

        pantallaPerdiste.SetActive(true);

        //Soltar el audio
        AudioManager.Instance.PlaySound(gameOverAudio, player.transform.position, 1000, 300, 500);

    }

    public void WinGame()
    {
        Debug.Log("You win!");

        isGameOver = true;

        ShowAndUnlockCursor();

        pantallaGanaste.SetActive(true);

        //Soltar el audio
        AudioManager.Instance.PlaySound(winAudio, player.transform.position, 1000,300,500);
    }

    public void RestartGame()
    {
        Debug.Log("Game Restarted");

        isGameOver = false;
        isPaused = false;

        ShowAndUnlockCursor();

        pantallaPerdiste.SetActive(false);
        pantallaGanaste.SetActive(false);

        Destroy(Instance.gameObject); // destruir el singleton manualmente
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Debug.Log("Next Level");

        isGameOver = false;
        isPaused = false;

        pantallaGanaste.SetActive(false);

        Destroy(Instance.gameObject); // destruir el singleton manualmente
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MainMenu()
    {
        Debug.Log("Main Menu");

        //Time.timeScale = 1.0f; //para reanudar el juego en caso se halla pausado

        isGameOver = false;
        isPaused = false;

        pantallaPerdiste.SetActive(false);
        pantallaGanaste.SetActive(false);

        Destroy(Instance.gameObject); // destruir el singleton manualmente
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
