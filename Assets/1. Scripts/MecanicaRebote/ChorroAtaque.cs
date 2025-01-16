using System.Collections;
using UnityEngine;

public class ChorroAtaque : MonoBehaviour
{
    [Header("depuracion")]
    public bool mostrarLog = true;

    #region Variables

    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;
    private Coroutine pourRoutine = null;

    [Header("Configuracion del Chorro")]
    public Vector3 direccionLinea = Vector3.down;
    public float velocidadAnimacion = 5f;
    public float longitudChorro = 10f;
    public int layerEscudo = 8;

    [Header("Configuracion del LineRenderer")]
    public float anchoInicial = 0.1f;
    public float anchoFinal = 0.05f;

    [Header("Configuracion de las Particulas")]
    public float tamanoParticulas = 0.01f;
    public GameObject prefabParticulaRebote;

    private GameObject particulaReboteInstanciada;
    private Camera mainCamera;

    #endregion

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();

        mainCamera = FindFirstObjectByType<Camera>();

        if (mainCamera == null)
        {
            if (mostrarLog) { Debug.LogError("No se encontro ninguna Main Camera en la escena."); }
        }
    }

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = anchoInicial;
        lineRenderer.endWidth = anchoFinal;

        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);

        if (splashParticle != null)
        {
            var mainModule = splashParticle.main;
            mainModule.startSize = tamanoParticulas;

            if (splashParticle.isPlaying)
            {
                splashParticle.Stop();
            }
        }
    }

    private void Update()
    {
        
    }

    public void Begin()
    {
        pourRoutine = StartCoroutine(HandlePour());
    }

    private IEnumerator HandlePour()
    {
        while (true)
        {
            Vector3[] puntos = CalcularReboteYColision();

            lineRenderer.positionCount = puntos.Length;
            for (int i = 0; i < puntos.Length; i++)
            {
                AnimateToPosition(i, puntos[i]);
            }

            yield return null;
        }
    }

    private Vector3[] CalcularReboteYColision()
    {
        Vector3 origen = transform.position;
        Vector3 direccion = direccionLinea.normalized;

        var puntos = new System.Collections.Generic.List<Vector3> { origen };

        RaycastHit hit;
        Ray ray = new Ray(origen, direccion);

        if (Physics.Raycast(ray, out hit, longitudChorro))
        {
            if (hit.collider.gameObject.layer == layerEscudo)
            {
                Vector3 puntoRebote = hit.point;
                puntos.Add(puntoRebote);

                if (mostrarLog) { Debug.Log($"Impacto inicial con el Escudo: {hit.collider.gameObject.name}"); }

                InstanciarParticulaRebote(puntoRebote);

                if (mainCamera != null)
                {
                    Vector3 direccionCamara = mainCamera.transform.forward;
                    RaycastHit segundoHit;
                    Ray segundoRay = new Ray(puntoRebote, direccionCamara);

                    if (Physics.Raycast(segundoRay, out segundoHit, longitudChorro))
                    {
                        Vector3 endPoint = segundoHit.point;
                        puntos.Add(endPoint);

                        ControlarParticula(endPoint);

                        if (mostrarLog) { Debug.Log($"Impacto tras el rebote con: {segundoHit.collider.gameObject.name}"); }
                    }
                    else
                    {
                        Vector3 endPoint = puntoRebote + (direccionCamara.normalized * longitudChorro);
                        puntos.Add(endPoint);

                        ControlarParticula(endPoint);

                        if (mostrarLog) { Debug.Log("No se detecto colision tras el rebote. Linea extendida."); }
                    }
                }
            }
            else
            {
                Vector3 endPoint = hit.point;
                puntos.Add(endPoint);

                ControlarParticula(endPoint);

                if (mostrarLog) { Debug.Log($"Impacto con: {hit.collider.gameObject.name}"); }

                DestruirParticulaRebote();
            }
        }
        else
        {
            Vector3 endPoint = ray.GetPoint(longitudChorro);
            puntos.Add(endPoint);

            ControlarParticula(endPoint);

            if (mostrarLog) { Debug.Log("No se detecto ningun impacto inicial."); }

            DestruirParticulaRebote();
        }

        return puntos.ToArray();
    }

    private void InstanciarParticulaRebote(Vector3 posicion)
    {
        if (prefabParticulaRebote != null && particulaReboteInstanciada == null)
        {
            particulaReboteInstanciada = Instantiate(prefabParticulaRebote, posicion, Quaternion.identity);
        }
    }

    private void DestruirParticulaRebote()
    {
        if (particulaReboteInstanciada != null)
        {
            Destroy(particulaReboteInstanciada);
            particulaReboteInstanciada = null;

            if (mostrarLog) { Debug.Log("Particula de rebote eliminada."); }
        }
    }

    private void ControlarParticula(Vector3 posicion)
    {
        if (splashParticle != null)
        {
            splashParticle.transform.position = posicion;

            if (!splashParticle.isPlaying)
            {
                splashParticle.Play();
            }
        }
    }

    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        if (index < lineRenderer.positionCount)
        {
            lineRenderer.SetPosition(index, targetPosition);
        }
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        if (index < lineRenderer.positionCount)
        {
            Vector3 current = lineRenderer.GetPosition(index);
            Vector3 newPosition = Vector3.MoveTowards(current, targetPosition, Time.deltaTime * velocidadAnimacion);
            lineRenderer.SetPosition(index, newPosition);
        }
    }

    public void End()
    {
        if (pourRoutine != null)
        {
            StopCoroutine(pourRoutine);
        }

        lineRenderer.positionCount = 2;
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);

        if (splashParticle != null && splashParticle.isPlaying)
        {
            splashParticle.Stop();
        }

        DestruirParticulaRebote();

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (pourRoutine != null)
        {
            StopCoroutine(pourRoutine);
        }

        lineRenderer.positionCount = 2;

        if (splashParticle != null && splashParticle.isPlaying)
        {
            splashParticle.Stop();
        }

        DestruirParticulaRebote();
    }
}
