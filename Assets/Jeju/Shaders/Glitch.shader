Shader "Custom/GlitchEffectUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShakePower ("Shake Power", Float) = 0.5
        _ShakeRate ("Shake Rate", Range(0.0, 1.0)) = 0.5
        _ShakeSpeed ("Shake Speed", Float) = 5.0
        _ShakeBlockSize ("Shake Block Size", Float) = 30.5
        _ShakeColorRate ("Shake Color Rate", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" } // 불투명 객체용 태그
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // #pragma multi_compile_fog // 필요하다면 포그 활성화

            #include "UnityCG.cginc" // 유용한 Unity 셰이더 함수 포함

            // Properties에서 선언된 변수들
            sampler2D _MainTex;
            float4 _MainTex_ST; // Unity가 텍스처 타일링/오프셋을 위해 추가
            float _ShakePower;
            float _ShakeRate;
            float _ShakeSpeed;
            float _ShakeBlockSize;
            float _ShakeColorRate;

            // Godot의 random 함수와 동일
            float random(float seed) {
                return frac(sin(seed * 12345.678) * 43758.5453);
            }

            // 버텍스 셰이더 입력 구조체
            struct appdata
            {
                float4 vertex : POSITION; // 정점 위치
                float2 uv : TEXCOORD0;    // UV 좌표
            };

            // 버텍스 셰이더에서 프래그먼트 셰이더로 전달될 데이터 구조체 (Godot의 varying과 유사)
            struct v2f
            {
                float4 vertex : SV_POSITION; // 클립 공간 정점 위치
                float2 uv : TEXCOORD0;       // UV 좌표
                float enable_shift : TEXCOORD1; // Godot의 varying enable_shift
            };

            // 버텍스 셰이더
            v2f vert (appdata v)
            {
                v2f o;

                float adjusted_time = fmod(_Time.y, 5.0); // Godot의 TIME, mod() -> _Time.y, fmod()

                // Godot의 random 결과는 0~1 사이, bool(A < B)는 A<B가 참이면 1.0, 거짓이면 0.0을 반환하는 것과 유사
                // HLSL에서는 bool이 직접 float으로 캐스팅되지 않으므로, 조건부 연산자 사용
                o.enable_shift = (random(trunc(adjusted_time * _ShakeSpeed)) < _ShakeRate) ? 1.0 : 0.0;

                float offset_x = (random((trunc(v.vertex.y * _ShakeBlockSize) / _ShakeBlockSize) + adjusted_time) - 0.5) * _ShakePower * o.enable_shift;
                
                float4 modified_vertex = v.vertex;
                modified_vertex.x += offset_x;

                o.vertex = UnityObjectToClipPos(modified_vertex); // 로컬 공간 -> 클립 공간 변환
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // 텍스처 타일링/오프셋 적용

                return o;
            }

            // 프래그먼트 셰이더
            fixed4 frag (v2f i) : SV_Target
            {
                float adjusted_time = fmod(_Time.y, 5.0);
                float2 fixed_uv = i.uv;

                // Godot의 VERTEX.y와 유사한 효과를 UV.y로 표현 (버텍스 셰이더에서와 동일한 로직)
                // 이는 Godot에서 VERTEX와 UV가 프래그먼트 셰이더에서도 접근 가능했던 것을 모방
                // 엄밀히 말하면, 프래그먼트 셰이더에서는 버텍스 위치를 직접 알 수 없으므로 UV를 사용
                fixed_uv.x += (
                    random((trunc(i.uv.y * _ShakeBlockSize) / _ShakeBlockSize) + adjusted_time) - 0.5
                ) * _ShakePower * i.enable_shift;

                fixed4 pixel_color = tex2D(_MainTex, fixed_uv); // Godot의 texture() -> tex2D()

                // Godot의 mix() -> lerp()
                pixel_color.r = lerp(
                    pixel_color.r,
                    tex2D(_MainTex, fixed_uv + float2(_ShakeColorRate, 0.0)).r,
                    i.enable_shift
                );
                pixel_color.b = lerp(
                    pixel_color.b,
                    tex2D(_MainTex, fixed_uv + float2(-_ShakeColorRate, 0.0)).b,
                    i.enable_shift
                );

                // ALBEDO = pixel_color.rgb; -> Unlit 셰이더는 최종 색상을 반환
                // 알파 채널은 텍스처의 알파를 사용하거나 불투명이면 1.0
                // return fixed4(pixel_color.rgb, 1.0);
                return fixed4(pixel_color.rgb, tex2D(_MainTex, i.uv).a); // 원본 텍스처의 알파 사용
            }
            ENDCG
        }
    }
    FallBack "Diffuse" // 지원하지 않는 하드웨어에서는 기본 Diffuse 셰이더 사용
}