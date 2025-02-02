using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class RotacionVfx : MonoBehaviour
{
    public bool sinContacto = true; // Controla si no hay contacto
    public bool cosContacto = false; // Controla si hay contacto
    private Vector3 rotacionExterna; // Rotación introducida externamente

    private void Update()
    {
        VisualEffect vfx = GetComponent<VisualEffect>();

        // Ajustar la escala al padre (solo X y Z)
        if (transform.parent != null)
        {
            Vector3 escalaPadre = transform.parent.localScale;
            vfx.SetVector3("Escala", new Vector3(escalaPadre.x* 0.31416f, escalaPadre.z * 0.31416f, 0.15f));
        }

        if (sinContacto)
        {
            Quaternion rotacion = transform.rotation;
            vfx.SetVector3("Rotacion", rotacion.eulerAngles);
        }
        else if (cosContacto)
        {            
            vfx.SetVector3("Rotacion", rotacionExterna); // Asegúrate de que este valor es correcto
        }
    }
    public void EstablecerRotacion(Vector3 nuevaRotacion)
    {        
        rotacionExterna = nuevaRotacion; // Actualizar la rotación externa
    }

}
