using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControladorAtaque : MonoBehaviour
{
    public static ControladorAtaque Instance { get; private set; }

    [Header("Depuración")]
    public bool mostrarLog;

    public HealthController playerhealth;
    public string recargarEscena;
    public GameObject panelTransicion;
    public bool estaVivo = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        estaVivo = true;

        playerhealth = GameObject.Find("Player").GetComponent<HealthController>();

        if (panelTransicion != null)
        {
            panelTransicion.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // playerhealth.DealDamage(1);
        }

        if (playerhealth.vidaActual == 0 && estaVivo)
        {
            AudioImp.Instance.Reproducir("PlayerDie");
            RecargarEscena(recargarEscena);
            estaVivo = false;
        }

        Shield shieldObject = FindFirstObjectByType<Shield>();
        if (shieldObject != null)
        {
            if (!shieldObject.gameObject.TryGetComponent<ColisionEscudo>(out _))
            {
                shieldObject.gameObject.AddComponent<ColisionEscudo>();
                if (mostrarLog) { Debug.Log("Se ha agregado el script ShieldCollisionLogger al prefab Shield."); }
            }
        }
        else
        {
            if (mostrarLog) { Debug.LogWarning("No se encontró un objeto llamado Shield en la escena."); }
        }

    }

    public void HacerDamagePlayer(int damage)
    {
        if (playerhealth != null)
        {
            AudioImp.Instance.Reproducir("PlayerHurt");
            playerhealth.DealDamage(damage);
        }
    }

    public void RecargarEscena(string nombreEscena)
    {
        StartCoroutine(CorutinaRecargarEscena(nombreEscena));
    }

    private IEnumerator CorutinaRecargarEscena(string nombreEscena)
    {
        if (panelTransicion != null)
        {
            panelTransicion.SetActive(true); // Activar el panel de transición
        }

        yield return new WaitForSeconds(3f); // Esperar 3 segundos

        SceneManager.LoadScene(nombreEscena); // Recargar la escena
    }
}
