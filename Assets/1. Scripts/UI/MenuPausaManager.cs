using UnityEngine;

public class MenuPausaManager : MonoBehaviour
{
    #region Variables

    [Header("depuracion")]
    public bool mostrarDebug = true;
    [Space]
    public GameObject canvasPausa;
    private bool canvasPausaOn;

    #endregion

    void Start()
    {
        canvasPausaOn = canvasPausa.activeSelf;
        ActualizarEstado();
        if (mostrarDebug) Debug.Log($"[MenuPausaManager] Menu de pausa iniciado con estado: {(canvasPausaOn ? "Activado" : "Desactivado")}.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvasPausaOn = !canvasPausaOn;
            canvasPausa.SetActive(canvasPausaOn);
            ActualizarEstado();
            if (mostrarDebug) Debug.Log($"[MenuPausaManager] Estado del menu de pausa: {(canvasPausaOn ? "Activado" : "Desactivado")}.");
        }
    }

    private void ActualizarEstado()
    {
        if (canvasPausaOn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //ControladorScripts.instance.MovimientoJugador(false);
            ControladorScripts.instance.PausarJuego();
            if (mostrarDebug) Debug.Log("[MenuPausaManager] Cursor desbloqueado, jugador detenido.");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //ControladorScripts.instance.MovimientoJugador(true);
            ControladorScripts.instance.ReanudarJuego();
            if (mostrarDebug) Debug.Log("[MenuPausaManager] Cursor bloqueado, jugador habilitado.");
        }
    }

    public void Volver()
    {
        canvasPausaOn = false;
        canvasPausa.SetActive(false);
        ActualizarEstado();
        if (mostrarDebug) Debug.Log("[MenuPausaManager] Menu de pausa desactivado al volver.");
    }
}
