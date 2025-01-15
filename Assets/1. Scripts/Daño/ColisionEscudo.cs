using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    [Header("Depuraci�n")]
    public bool mostrarLog;
    [Space]
    public bool da�o = false;
    private ControladorUIEnemigo controladorUI;

    private void Start()
    {
        controladorUI = FindFirstObjectByType<ControladorUIEnemigo>();
        if (controladorUI == null)
        {
            if (mostrarLog) { Debug.LogError("[ColisionEscudo] No se encontr� un ControladorUIEnemigo en la escena."); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo") && !da�o)
        {
            if (mostrarLog) { Debug.Log($"El Shield entr� en contacto con: {other.gameObject.name}"); }

            ControladorVidaEnemigo enemigo = other.GetComponent<ControladorVidaEnemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDa�o();
                da�o = true;

                if (controladorUI != null)
                {
                    controladorUI.ActualizarUI(enemigo);
                }
            }
        }
    }
}
