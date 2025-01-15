using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    [Header("Depuración")]
    public bool mostrarLog;
    [Space]
    public bool daño = false;
    private ControladorUIEnemigo controladorUI;

    private void Start()
    {
        controladorUI = FindFirstObjectByType<ControladorUIEnemigo>();
        if (controladorUI == null)
        {
            if (mostrarLog) { Debug.LogError("[ColisionEscudo] No se encontró un ControladorUIEnemigo en la escena."); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") && !daño)
        {
            if (mostrarLog) { Debug.Log($"El Shield entró en contacto con: {other.gameObject.name}"); }

            ControladorVidaEnemigo enemigo = other.GetComponent<ControladorVidaEnemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDaño();
                daño = true;

                if (controladorUI != null)
                {
                    controladorUI.ActualizarUI(enemigo);
                }
            }
        }
    }
}
