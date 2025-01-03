Shader "Custom/Blur"{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurRadius("Blur Radius", Range(0.0, 10.0)) = 1.0
        _AlphaWeight("Alpha Weight",Range(0.0,1.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 200
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _BlurRadius;
            float _BlurTexelSize;
            float _AlphaWeight;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            // Function to calculate Gaussian weight
            float Gaussian(float x, float deviation)
            {
                return exp(-(x * x) / (2 * deviation * deviation)) / (deviation * sqrt(2 * UNITY_PI));
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 MainColor = tex2D(_MainTex, i.uv); MainColor.a *= _AlphaWeight;
                if (MainColor.a == 0) discard;
                if (_BlurRadius == 0) return MainColor;

                float2 texelSize = 1.0 / _ScreenParams.xy;

                float4 color = float4(0, 0, 0, 0);
                float totalWeight = 0.0;

                for (int x = -5; x <= 5; x++)
                {
                    float2 offset = float2(x, 0) * texelSize * _BlurRadius;
                    float4 samp = tex2D(_MainTex, i.uv + offset);
                    if (samp.a > 0) 
                    {
                        float weight = Gaussian(x, _BlurRadius);
                        color += samp * weight;
                        totalWeight += weight;
                    }
                }

                for (int y = -5; y <= 5; y++) 
                {
                    float2 offset = float2(0, y) * texelSize * _BlurRadius;
                    float4 samp = tex2D(_MainTex, i.uv + offset);
                    if (samp.a > 0) {
                        float weight = Gaussian(x, _BlurRadius);
                        color += samp * weight;
                        totalWeight += weight;
                    }
                }
                color /= totalWeight;
                color.a *= _AlphaWeight;
                return color;
            }
            ENDCG
        }
    }
}