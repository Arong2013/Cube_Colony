Shader "Custom/URP/QuadTextureShader"
{
    Properties
    {
        _Tex1 ("Top Left", 2D) = "white" {}
        _Tex2 ("Top Right", 2D) = "white" {}
        _Tex3 ("Bottom Left", 2D) = "white" {}
        _Tex4 ("Bottom Right", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_Tex1); SAMPLER(sampler_Tex1);
            TEXTURE2D(_Tex2); SAMPLER(sampler_Tex2);
            TEXTURE2D(_Tex3); SAMPLER(sampler_Tex3);
            TEXTURE2D(_Tex4); SAMPLER(sampler_Tex4);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float3 normalWS = normalize(IN.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float NdotL = saturate(dot(normalWS, lightDir));
                float3 lightColor = _MainLightColor.rgb;

                half4 texColor;

                if (uv.x < 0.5 && uv.y < 0.5) // Bottom Left
                    texColor = SAMPLE_TEXTURE2D(_Tex3, sampler_Tex3, uv * 2);
                else if (uv.x >= 0.5 && uv.y < 0.5) // Bottom Right
                    texColor = SAMPLE_TEXTURE2D(_Tex4, sampler_Tex4, (uv - float2(0.5, 0)) * 2);
                else if (uv.x < 0.5 && uv.y >= 0.5) // Top Left
                    texColor = SAMPLE_TEXTURE2D(_Tex1, sampler_Tex1, (uv - float2(0, 0.5)) * 2);
                else // Top Right
                    texColor = SAMPLE_TEXTURE2D(_Tex2, sampler_Tex2, (uv - float2(0.5, 0.5)) * 2);

                float3 litColor = texColor.rgb * lightColor * NdotL;
                return half4(litColor, 1.0);
            }

            ENDHLSL
        }

        // 그림자 캐스팅용 패스
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShadowCasterPass.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = GetShadowCasterPosition(positionWS, normalWS);
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_Target
            {
                return 0;
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
