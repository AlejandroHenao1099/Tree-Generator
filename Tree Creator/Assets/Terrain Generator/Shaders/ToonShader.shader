Shader "MisShaders/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        _LighterColor ("Light Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Toon fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;

        fixed4 _Color;
        fixed4 _LighterColor;
        fixed4 _ShadowColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        fixed4 LightingToon (SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed NdotL = dot(s.Normal, lightDir);
            fixed4 color;
            if(NdotL < 0.5) 
                color.rgb = s.Albedo * _ShadowColor.rgb * _LightColor0.rgb;
            else
                color.rgb = s.Albedo * _LighterColor.rgb * _LightColor0.rgb;
            
            color.a = s.Alpha;
            return color;
        }
        ENDCG
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack "Diffuse"
}
