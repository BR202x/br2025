using UnityEngine;

public class ColisionEscudo : MonoBehaviour
{
    private ControladorUIEnemigo controladorUI;

    private void Start()
    {
        controladorUI = FindFirstObjectByType<ControladorUIEnemigo>();
        if (controladorUI == null)
        {
            Debug.LogError("[ColisionEscudo] No se encontr� un ControladorUIEnemigo en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            Debug.Log($"El Shield entr� en contacto con: {other.gameObject.name}");

            ControladorVidaEnemigo enemigo = other.GetComponent<ControladorVidaEnemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDa�o();

                if (controladorUI != null)
                {
                    controladorUI.ActualizarUI(enemigo);
                }
            }
        }
    }
}
