Shader "MisShaders/TerrainToon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LighterColor ("Light Color", Color) = (1,1,1,1)
        _MediumLightColor ("Medium Color", Color) = (1,1,1,1)
        _LowColor ("Low Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Toon fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        fixed4 _LighterColor;
        fixed4 _MediumLightColor;
        fixed4 _LowColor;
        fixed4 _ShadowColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }

        fixed4 LightingToon (SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed NdotL = dot(s.Normal, lightDir);

            fixed4 color;
            if(NdotL < -0.3) 
                color.rgb = s.Albedo * _ShadowColor.rgb * _LightColor0.rgb;
            else if(NdotL < 0.2)
                color.rgb = s.Albedo * _LowColor.rgb * _LightColor0.rgb;
            else if(NdotL < 0.6)
                color.rgb = s.Albedo * _MediumLightColor.rgb * _LightColor0.rgb;
            else
                color.rgb = s.Albedo * _LighterColor.rgb * _LightColor0.rgb;
            
            color.a = s.Alpha;
            return color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
