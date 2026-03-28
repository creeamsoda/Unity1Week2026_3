Shader "Custom/DollBodyShader_Masked"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        
        // --- 追加部分 ---
        _MaskMap("Mask (RenderTexture)", 2D) = "black" {} // C#から渡すRenderTexture
        _OverlayColor("Overlay Color", Color) = (1, 0, 0, 1) // マスク領域に塗る色
        _OverlayIntensity("Overlay Intensity", Range(0, 1)) = 1.0 // 色替えの強さ
        // ----------------
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // --- 追加部分 ---
            TEXTURE2D(_MaskMap);
            SAMPLER(sampler_MaskMap);
            // ----------------

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                // --- 追加部分 ---
                half4 _OverlayColor;
                float _OverlayIntensity;
                // ----------------
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. 元のテクスチャの色を計算
                half4 baseTexColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                // 2. マスクテクスチャをサンプリング（同じUVを使用）
                half4 mask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, IN.uv);

                // 3. マスクのR（赤）チャンネルを「色替えの強さ」として利用
                // mask.r が 0なら元の色、1ならOverlayColorになる
                float blendFactor =  mask.r * _OverlayIntensity;

                // 4. lerp関数で色を混ぜる
                half3 finalRGB = lerp(baseTexColor.rgb, _OverlayColor.rgb, blendFactor);

                // 5. 透明度は元の画像のものを維持
                return half4(finalRGB, baseTexColor.a);
            }
            ENDHLSL
        }
    }
}