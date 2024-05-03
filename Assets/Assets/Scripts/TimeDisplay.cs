using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    public enum TimeMode
    {
        Stopwatch, // Cronómetro
        Countdown // Conta atrás
    }

    public TimeMode timeMode = TimeMode.Stopwatch; // Modo de tempo que por defecto é cronómetro
    public float totalTime; // Tempo total 
    private float elapsedTime = 0f; // Tempo consumido 
    private float remainingTime = 0f; // Tempo restante 
    private float startTime; // Tempo inicial
    private Text timeText; // Tempo en texto

    /*
    Ao arrancar iniciamos o tempo e asociamos ao texto
    */
    void Start()
    {
        startTime = Time.time;

        timeText = GetComponent<Text>();
    }

    /*
    Método para reiniciar o tempo dependendo do tipo de timemode que teñamos
    */
    public void RestartTime()
    {
        elapsedTime = 0f;

        if (timeMode == TimeMode.Stopwatch)
        {
            startTime = Time.time;
        }
        else if (timeMode == TimeMode.Countdown)
        {
            remainingTime = totalTime;
            startTime = Time.time;
        }
    }

    /*
    Actualizamos en todo momento dependendo do tipo de timemode que teñamos
    */
    void Update()
    {
        elapsedTime += Time.deltaTime;

        float timeElapsed = Time.time - startTime;

        if (timeMode == TimeMode.Stopwatch)
        {
            elapsedTime = timeElapsed;
        }
        else if (timeMode == TimeMode.Countdown)
        {
            remainingTime = Mathf.Max(totalTime - timeElapsed, 0f);
        }

        elapsedTime = Mathf.Max(elapsedTime, 0f);

        UpdateTimeText();
    }

    /*
    Convertemos o tempo a texto
    */
    void UpdateTimeText()
    {
        // Convertir el tiempo (elapsedTime o remainingTime) a minutos y segundos
        int minutes = Mathf.FloorToInt((timeMode == TimeMode.Stopwatch) ? elapsedTime / 60f : remainingTime / 60f);
        int seconds = Mathf.FloorToInt((timeMode == TimeMode.Stopwatch) ? elapsedTime % 60f : remainingTime % 60f);

        // Actualizar el texto del objeto de texto con el tiempo transcurrido o restante
        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
