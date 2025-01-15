using UnityEngine;

public class MenuPausaManager : MonoBehaviour
{
#region Variables

    [Header("Depuración")]
    public bool mostrarDebug = true;
    [Space]
    public GameObject canvasPausa;
    private bool canvasPausaOn;

#endregion

    void Start()
    {
        canvasPausaOn = canvasPausa.activeSelf;
        ActualizarEstado();
        if (mostrarDebug) Debug.Log($"[MenuPausaManager] Menú de pausa iniciado con estado: {(canvasPausaOn ? "Activado" : "Desactivado")}.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvasPausaOn = !canvasPausaOn;
            canvasPausa.SetActive(canvasPausaOn);
            ActualizarEstado();
            if (mostrarDebug) Debug.Log($"[MenuPausaManager] Estado del menú de pausa: {(canvasPausaOn ? "Activado" : "Desactivado")}.");
        }
    }

    private void ActualizarEstado()
    {
        if (canvasPausaOn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ControladorScripts.instance.MovimientoJugador(false);
            if (mostrarDebug) Debug.Log("[MenuPausaManager] Cursor desbloqueado, jugador detenido.");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ControladorScripts.instance.MovimientoJugador(true);
            if (mostrarDebug) Debug.Log("[MenuPausaManager] Cursor bloqueado, jugador habilitado.");
        }
    }

    public void Volver()
    {
        canvasPausaOn = false;
        canvasPausa.SetActive(false);
        ActualizarEstado();
        if (mostrarDebug) Debug.Log("[MenuPausaManager] Menú de pausa desactivado al volver.");
    }
}
