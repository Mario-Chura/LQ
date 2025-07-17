using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MathQuestion : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mathQuestionPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI instructionsText; // Texto separado para instrucciones
    public Button[] answerButtons;
    
    [Header("Question Settings")]
    public int minNumber = 1;
    public int maxNumber = 20;
    
    [Header("Difficulty Settings")]
    public MathDifficulty currentDifficulty = MathDifficulty.Easy;

    [Tooltip("TextMeshProUGUI para colocar si la respuesta es correcta o no")] 
    [SerializeField] TextMeshProUGUI answerQuestion;
    [SerializeField] AudioSource audioSourceAnswer;
    [SerializeField] AudioClip audioAnswerCorrect;
    [SerializeField] AudioClip audioAnswerIncorrect;

    public enum MathDifficulty
    {
        Easy,      // Sumas simples (5 + 3)
        Medium,    // Operaciones mixtas (15 - 7, 4 × 6)
        Hard,      // Álgebra simple (x + 5 = 12, 2x = 16)
        Expert,    // Álgebra compleja (2x + 3 = 15, 3x - 7 = 20)
        Master     // Álgebra muy compleja (ecuaciones con fracciones, potencias)
    }
    
    private int correctAnswer;
    private bool isQuestionActive = false;
    private int selectedOptionIndex = 0;
    private int[] currentOptions = new int[4];
    private Key currentKey;
    
    void Update()
    {
        if (isQuestionActive)
        {
            HandleInput();
        }
    }
    
    private void HandleInput()
    {
        // Navegar con flechas arriba/abajo
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + 4) % 4;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedOptionIndex = (selectedOptionIndex + 1) % 4;
            UpdateSelection();
        }
        
        // Confirmar selección con Enter o Espacio
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SelectCurrentOption();
        }
        
        // También permitir números directos (1, 2, 3, 4)
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectedOptionIndex = 0; SelectCurrentOption(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { selectedOptionIndex = 1; SelectCurrentOption(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { selectedOptionIndex = 2; SelectCurrentOption(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { selectedOptionIndex = 3; SelectCurrentOption(); }
    }
    
    private void UpdateSelection()
    {
        // Actualizar colores de los botones para mostrar selección
        for (int i = 0; i < answerButtons.Length; i++)
        {
            ColorBlock colors = answerButtons[i].colors;
            if (i == selectedOptionIndex)
            {
                colors.normalColor = Color.yellow;
            }
            else
            {
                colors.normalColor = Color.white;
            }
            answerButtons[i].colors = colors;
        }
        
        // Solo mostrar instrucciones en el texto separado
        if (instructionsText != null)
        {
            instructionsText.text = $"Opción seleccionada: {selectedOptionIndex + 1} ({currentOptions[selectedOptionIndex]})\n\n" +
                                   "↑↓ Navegar entre opciones\n" +
                                   "Enter/Espacio: Confirmar selección\n" +
                                   "1 2 3 4: Selección directa con números\n\n" +
                                   $"Dificultad: {GetDifficultyName()}";
        }
    }
    
    private string GetDifficultyName()
    {
        switch (currentDifficulty)
        {
            case MathDifficulty.Easy: return "Fácil (Sumas)";
            case MathDifficulty.Medium: return "Medio (Operaciones)";
            case MathDifficulty.Hard: return "Difícil (Álgebra Simple)";
            case MathDifficulty.Expert: return "Experto (Álgebra Compleja)";
            case MathDifficulty.Master: return "MASTER (Álgebra Avanzada)";
            default: return "Desconocido";
        }
    }
    
    private void SelectCurrentOption()
    {
        int selectedAnswer = currentOptions[selectedOptionIndex];
        CheckAnswer(selectedAnswer);
    }
    
    public void SetCurrentKey(Key key)
    {
        currentKey = key;
    }
    
    public void ShowMathQuestion()
    {
        isQuestionActive = true;
        selectedOptionIndex = 0;
        Time.timeScale = 0f;
        PlayerSingleton.Instance.ShowAndUnlockCursor();
        
        // Determinar dificultad basada en el número de llaves recolectadas
        DetermineDifficulty();
        
        GenerateQuestion();
        mathQuestionPanel.SetActive(true);
        UpdateSelection();

        ///Desactivamos objetos de canvas para una imagen mas limpia
        ObjectsEnableDisableCanvas.Instance.DesactiveObjectsCanvasMathQuestion();
        answerQuestion.enabled = false;

    }

    private void DetermineDifficulty()
    {
        int keysCollected = PlayerSingleton.Instance.playerInventory.GetCurrentKeys();
        
        // Empezar desde difícil porque fácil/medio son muy fáciles
        switch (keysCollected)
        {
            case 0: 
                // 1ra llave: Álgebra simple
                currentDifficulty = MathDifficulty.Hard; 
                break;
            case 1: 
                // 2da llave: Álgebra compleja
                currentDifficulty = MathDifficulty.Expert; 
                break;
            case 2: 
                // 3ra llave: Álgebra MASTER (muy compleja)
                currentDifficulty = MathDifficulty.Master; 
                break;
            default: 
                currentDifficulty = MathDifficulty.Master; 
                break;
        }
    }
    
    private void GenerateQuestion()
    {
        switch (currentDifficulty)
        {
            case MathDifficulty.Easy:
                GenerateEasyQuestion();
                break;
            case MathDifficulty.Medium:
                GenerateMediumQuestion();
                break;
            case MathDifficulty.Hard:
                GenerateHardQuestion();
                break;
            case MathDifficulty.Expert:
                GenerateExpertQuestion();
                break;
            case MathDifficulty.Master:
                GenerateMasterQuestion();
                break;
        }
        
        GenerateAnswerOptions();
        AssignButtonTexts();
    }
    
    private void GenerateEasyQuestion()
    {
        // Sumas simples
        int num1 = Random.Range(minNumber, maxNumber);
        int num2 = Random.Range(minNumber, maxNumber);
        correctAnswer = num1 + num2;
        questionText.text = $"¿Cuánto es {num1} + {num2}?";
    }
    
    private void GenerateMediumQuestion()
    {
        int operation = Random.Range(0, 3); // 0=suma, 1=resta, 2=multiplicación
        
        switch (operation)
        {
            case 0: // Suma
                int a = Random.Range(10, 25);
                int b = Random.Range(5, 15);
                correctAnswer = a + b;
                questionText.text = $"¿Cuánto es {a} + {b}?";
                break;
                
            case 1: // Resta
                int c = Random.Range(15, 30);
                int d = Random.Range(5, c);
                correctAnswer = c - d;
                questionText.text = $"¿Cuánto es {c} - {d}?";
                break;
                
            case 2: // Multiplicación
                int e = Random.Range(3, 8);
                int f = Random.Range(2, 6);
                correctAnswer = e * f;
                questionText.text = $"¿Cuánto es {e} × {f}?";
                break;
        }
    }
    
    private void GenerateHardQuestion()
    {
        int algebraType = Random.Range(0, 2); // 0=x+a=b, 1=ax=b
        
        switch (algebraType)
        {
            case 0: // x + a = b, entonces x = b - a
                correctAnswer = Random.Range(3, 15);
                int addend = Random.Range(2, 10);
                int sum = correctAnswer + addend;
                questionText.text = $"Si x + {addend} = {sum}, ¿cuánto vale x?";
                break;
                
            case 1: // ax = b, entonces x = b/a
                correctAnswer = Random.Range(2, 10);
                int multiplier = Random.Range(2, 6);
                int product = correctAnswer * multiplier;
                questionText.text = $"Si {multiplier}x = {product}, ¿cuánto vale x?";
                break;
        }
    }
    
    private void GenerateExpertQuestion()
    {
        int expertType = Random.Range(0, 2); // 0=ax+b=c, 1=ax-b=c
        
        switch (expertType)
        {
            case 0: // ax + b = c, entonces x = (c - b) / a
                correctAnswer = Random.Range(2, 8);
                int coeff1 = Random.Range(2, 5);
                int constant1 = Random.Range(1, 8);
                int result1 = coeff1 * correctAnswer + constant1;
                questionText.text = $"Si {coeff1}x + {constant1} = {result1}, ¿cuánto vale x?";
                break;
                
            case 1: // ax - b = c, entonces x = (c + b) / a
                correctAnswer = Random.Range(3, 10);
                int coeff2 = Random.Range(2, 4);
                int constant2 = Random.Range(2, 8);
                int result2 = coeff2 * correctAnswer - constant2;
                questionText.text = $"Si {coeff2}x - {constant2} = {result2}, ¿cuánto vale x?";
                break;
        }
    }
    
    private void GenerateMasterQuestion()
    {
        int masterType = Random.Range(0, 4); // 4 tipos diferentes de problemas avanzados
        
        switch (masterType)
        {
            case 0: // Ecuaciones con coeficientes mayores: 5x + 12 = 47
                correctAnswer = Random.Range(3, 12);
                int coeff1 = Random.Range(4, 8);
                int constant1 = Random.Range(8, 20);
                int result1 = coeff1 * correctAnswer + constant1;
                questionText.text = $"Si {coeff1}x + {constant1} = {result1}, ¿cuánto vale x?";
                break;
                
            case 1: // Ecuaciones con división: (x + 5) ÷ 3 = 8, entonces x + 5 = 24, x = 19
                correctAnswer = Random.Range(4, 15);
                int addend = Random.Range(2, 8);
                int divisor = Random.Range(2, 5);
                int quotient = (correctAnswer + addend) / divisor;
                // Asegurar que la división sea exacta
                int sum = quotient * divisor;
                correctAnswer = sum - addend;
                questionText.text = $"Si (x + {addend}) ÷ {divisor} = {quotient}, ¿cuánto vale x?";
                break;
                
            case 2: // Potencias simples: x² = 25, x² = 36, x² = 49
                int[] perfectSquares = {4, 9, 16, 25, 36, 49, 64, 81, 100};
                int randomSquare = perfectSquares[Random.Range(0, perfectSquares.Length)];
                correctAnswer = (int)Mathf.Sqrt(randomSquare);
                questionText.text = $"Si x² = {randomSquare}, ¿cuánto vale x? (respuesta positiva)";
                break;
                
            case 3: // Ecuaciones más complejas: 2(x + 3) = 16, entonces 2x + 6 = 16, 2x = 10, x = 5
                correctAnswer = Random.Range(2, 10);
                int multiplier = Random.Range(2, 4);
                int innerAdd = Random.Range(1, 6);
                int finalResult = multiplier * (correctAnswer + innerAdd);
                questionText.text = $"Si {multiplier}(x + {innerAdd}) = {finalResult}, ¿cuánto vale x?";
                break;
        }
    }
    
    private void GenerateAnswerOptions()
    {
        currentOptions[0] = correctAnswer;
        currentOptions[1] = correctAnswer + Random.Range(1, 6);
        currentOptions[2] = correctAnswer - Random.Range(1, 5);
        currentOptions[3] = correctAnswer + Random.Range(-4, 8);
        
        // Asegurar que no sean negativos
        for (int i = 1; i < 4; i++)
        {
            if (currentOptions[i] < 1) 
                currentOptions[i] = correctAnswer + Random.Range(1, 8);
        }
        
        // Mezclar opciones
        for (int i = 0; i < currentOptions.Length; i++)
        {
            int temp = currentOptions[i];
            int randomIndex = Random.Range(i, currentOptions.Length);
            currentOptions[i] = currentOptions[randomIndex];
            currentOptions[randomIndex] = temp;
        }
    }
    
    private void AssignButtonTexts()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{currentOptions[i]}";
        }
    }
    
    private void CheckAnswer(int selectedAnswer)
    {
        if (selectedAnswer == correctAnswer)
        {
            Debug.Log($"¡Correcto! Dificultad: {currentDifficulty}");
            PlayerSingleton.Instance.playerInventory.CollectKey();
            if (currentKey != null)
            {
                currentKey.CollectKeySuccessfully();
                
                //manda mensaje si la respuesta es correcta
                if (answerQuestion != null)
                    answerQuestion.SetText("La respuesta es correcta...");
                if (audioSourceAnswer != null)
                    audioSourceAnswer.PlayOneShot(audioAnswerCorrect);
            }
        }
        else
        {
            Debug.Log($"Incorrecto. La respuesta era: {correctAnswer}");

            //manda mensaje si la respuesta es incorrecta
            if (answerQuestion != null)
                answerQuestion.SetText("La respuesta es incorrecta...");
            if (audioSourceAnswer != null)
                audioSourceAnswer.PlayOneShot(audioAnswerIncorrect);
        }

        CloseQuestion();

        if (answerQuestion != null)
        {
            answerQuestion.enabled = true;

            //despues de 3 seg se desactiva el estado de respuesta
            StartCoroutine(DisableAnswerStatusText());
        }
    }

    private void CloseQuestion()
    {
        isQuestionActive = false;
        mathQuestionPanel.SetActive(false);
        Time.timeScale = 1f;
        PlayerSingleton.Instance.HideAndLockCursor();

        ///Activamos objetos que habian sido desactivados
        ObjectsEnableDisableCanvas.Instance.ActiveObjectsCanvasMathQuestion();
    }

    //Desactivar estado de respuesta a la pregunta
    IEnumerator DisableAnswerStatusText()
    {
        yield return new WaitForSeconds(3f);
        
        answerQuestion.enabled = false;
    }
}