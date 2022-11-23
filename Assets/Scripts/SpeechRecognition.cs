using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using TMPro;

public class SpeechRecognition : MonoBehaviour
{

    public TextMeshProUGUI myTMP;

    private KeywordRecognizer reconocePalabras;
    private ConfidenceLevel confidencialidad = ConfidenceLevel.Low;
    private Dictionary<string, Accion> palabrasAccion = new Dictionary<string, Accion>();
    private SimpleCarController myCarController;

    //crear Delegado para la acción a ejecutar
    private delegate void Accion();

    // Start is called before the first frame update
    void Start()
    {
        myCarController = GetComponent<SimpleCarController>();

        palabrasAccion.Add("acelera", myCarController.MoveForward);
        palabrasAccion.Add("reversa", myCarController.MoveBackward);
        palabrasAccion.Add("izquierda", myCarController.SteerLeft);
        palabrasAccion.Add("derecha", myCarController.SteerRight);
        palabrasAccion.Add("todo izquierda", myCarController.SteerAllLeft);
        palabrasAccion.Add("todo derecha", myCarController.SteerAllRight);
        palabrasAccion.Add("alto", myCarController.StopCar);
        palabrasAccion.Add("adelante", myCarController.SteerStraightAhead);
        palabrasAccion.Add("velocidad alta", myCarController.MaxSpeed);
        palabrasAccion.Add("resetea", myCarController.ResetCar);

        reconocePalabras = new KeywordRecognizer(palabrasAccion.Keys.ToArray(), confidencialidad);
        reconocePalabras.OnPhraseRecognized += OnKeywordsRecognized;
        reconocePalabras.Start();
    }

    void OnDestroy()
    {
        if (reconocePalabras != null && reconocePalabras.IsRunning)
        {
            reconocePalabras.Stop();
            reconocePalabras.Dispose();
        }
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        myTMP.text = args.text;
        palabrasAccion[args.text].Invoke();
    }
}
