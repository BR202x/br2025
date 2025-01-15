using UnityEngine;

public class MenuPausaManager : MonoBehaviour
{
    public GameObject canvasPausa; // Referencia al GameObject del menú de pausa
    private bool canvasPausaOn;    // Estado del menú de pausa

    void Start()
    {        
        canvasPausaOn = canvasPausa.activeSelf;
        ActualizarEstado();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            canvasPausaOn = !canvasPausaOn;
            canvasPausa.SetActive(canvasPausaOn);

            ActualizarEstado();

            Debug.Log($"Estado del menú de pausa: {(canvasPausaOn ? "Activado" : "Desactivado")}");
        }
    }

    private void ActualizarEstado()
    {
        if (canvasPausaOn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ControladorScripts.instance.MovimientoJugador(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            ControladorScripts.instance.MovimientoJugador(true);
        }
    }

    public void Volver()
    {
        canvasPausaOn = false;
        canvasPausa.SetActive(false);
        ActualizarEstado();
    }
}
