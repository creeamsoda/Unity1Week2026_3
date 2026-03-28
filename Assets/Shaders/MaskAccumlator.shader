Shader "Custom/MaskAccumulator"
{
    Properties
    {
        _MainTex ("Accumulated Mask (Existing)", 2D) = "black" {}
        _CurrentRectTex ("Current Rects (New)", 2D) = "black" {}
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_CurrentRectTex); SAMPLER(sampler_CurrentRectTex);

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // これまでに塗られた領域 (1 = 塗られている)
                float oldMask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).r;
                
                // 今回のコライダ領域 (1 = コライダの内側)
                float currentRects = SAMPLE_TEXTURE2D(_CurrentRectTex, sampler_CurrentRectTex, IN.uv).r;

                // 「今回のコライダの外側」は (1.0 - currentRects)
                float newArea = 1.0 - currentRects;

                // 既存の領域か、新しい領域のどちらかが 1 なら 1 にする
                return float4(max(oldMask, newArea), 0, 0, 1);
            }
            ENDHLSL
        }
    }
}