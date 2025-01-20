using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ImpactFrameEffect : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader impactShader;
        [Range(0f, 1f)] public float impactAmount = 0.5f; // Controla la cantidad de impacto
    }

    public Settings settings = new Settings();
    private Material impactMaterial;

    // Subclase de ScriptableRenderPass
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material impactMaterial;
        private float impactAmount;

        public CustomRenderPass(Material material, float amount)
        {
            impactMaterial = material;
            impactAmount = amount;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents; // Aplica después de la transparencia
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Comprobamos si el material existe
            if (impactMaterial != null)
            {
                var cmd = CommandBufferPool.Get("ImpactFrameEffect");

                // Hacer un blit de la textura con el material (efecto)
                cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, impactMaterial);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

    private CustomRenderPass customRenderPass;

    // Implementamos el método Create() requerido por ScriptableRendererFeature
    public override void Create()
    {
        if (settings.impactShader != null && impactMaterial == null)
        {
            // Crear material solo si no está creado aún
            impactMaterial = new Material(settings.impactShader);
        }

        // Crear un nuevo pase de renderizado
        customRenderPass = new CustomRenderPass(impactMaterial, settings.impactAmount);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (customRenderPass != null)
        {
            // Agregamos el pase de renderizado al renderer
            renderer.EnqueuePass(customRenderPass);
        }
    }
}