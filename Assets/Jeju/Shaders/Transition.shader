Shader "Custom/PixelTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TransitionTime ("Transition Time", Float) = 1.0
        _Resolution ("Resolution", Float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TransitionTime;
            float _Resolution;
            
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 96.233))) * 43758.5453);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 iResolution = _ScreenParams.xy;
                float2 fragCoord = i.screenPos.xy / i.screenPos.w * iResolution;
                float2 uv = fragCoord / iResolution;
                  float2 lowresxy = float2(
                    floor(fragCoord.x / _Resolution),
                    floor(fragCoord.y / _Resolution)
                );
                  if(_TransitionTime > rand(lowresxy))
                {
                    return fixed4(0.0, 0.0, 0.0, 1.0); // 검은색으로 변경
                }
                else
                {
                    return fixed4(0.0, 0.0, 0.0, 0.0); // 투명색 유지
                }
            }
            ENDCG
        }
    }
}