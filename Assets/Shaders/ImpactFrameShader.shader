Shader "Custom/ImpactFrameShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _ImpactAmount ("Impact Amount", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Textura de la escena
            sampler2D _MainTex;
            float _ImpactAmount;

            // Función para convertir a escala de grises
            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                // Convertir a escala de grises
                half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));
                col.rgb = half3(gray, gray, gray);

                // Aplicar el impacto: mezcla con negro basado en _ImpactAmount
                col.rgb = lerp(col.rgb, half3(0, 0, 0), _ImpactAmount);

                return col;
            }

            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}