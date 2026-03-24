Shader "Hidden/RectDrawer"
{
    Properties
    {
        _Center("Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Size("Size (UV)", Vector) = (0.2, 0.2, 0, 0)
        _Rotation("Rotation (Rad)", Float) = 0
        _Aspect("Aspect Ratio (W/H)", Float) = 1.0 // 追加
    }
    SubShader
    {
        Blend One One
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            float2 _Center;
            float2 _Size;
            float _Rotation;
            float _Aspect; // 追加

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // 1. 中心を基準にしつつ、横方向(X)をアスペクト比で補正
                // これにより、計算空間内ではスプライトが常に「正方形」として扱われる
                float2 st = uv - _Center;
                st.x *= _Aspect; 

                // 2. 補正した空間で回転（歪まない！）
                float s = sin(-_Rotation);
                float c = cos(-_Rotation);
                float2 rotatedUV;
                rotatedUV.x = st.x * c - st.y * s;
                rotatedUV.y = st.x * s + st.y * c;

                // 3. 判定用のサイズも横方向を補正して比較
                float2 halfSize = _Size * 0.5;
                float2 targetSize = float2(halfSize.x * _Aspect, halfSize.y);

                float isInside = step(abs(rotatedUV.x), targetSize.x) * step(abs(rotatedUV.y), targetSize.y);

                return half4(isInside, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}