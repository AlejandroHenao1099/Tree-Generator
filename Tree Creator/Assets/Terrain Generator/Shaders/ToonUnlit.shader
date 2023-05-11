Shader "MisShaders/ToonUnlit"
{
    Properties
    {
        // [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}

        _Color ("Color", Color) = (1,1,1,1)

        _LighterColor ("Light Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        // Tags { "RenderType"="Opaque" }
        // LOD 100

        Pass
        {
            Tags {"LightMode"="ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" // for UnityObjectToWorldNormals
            #include "UnityLightingCommon.cginc" // for _LightColor0

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Interpolators
            {
                float4 pos : SV_POSITION;
                fixed4 diff : COLOR0; // diffuse lighting color
                float2 uv : TEXCOORD0;
                // fixed3 ambient : COLOR1;
                // SHADOW_COORDS(1) // put shadows data into TEXCOORD1
            };

            // sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _LighterColor;
            fixed4 _ShadowColor;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                // half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half nl = dot(worldNormal, _WorldSpaceLightPos0.xyz);

                fixed4 color;
                if(nl < 0.5)
                    o.diff = _ShadowColor * _LightColor0;
                    // color.rgb = s.Albedo * _ShadowColor.rgb * _LightColor0.rgb;
                else
                    o.diff = _LighterColor * _LightColor0;
                    // color.rgb = s.Albedo * _LighterColor.rgb * _LightColor0.rgb;

                // o.diff = nl * _LightColor0;

                // o.ambient = ShadeSH9(half4(worldNormal,1));
                // TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                fixed4 col = _Color;
                // fixed shadow = SHADOW_ATTENUATION(i);
                // fixed3 lighting = i.diff * shadow + i.ambient;
                fixed3 lighting = i.diff;
                col.rgb *= lighting;
                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
