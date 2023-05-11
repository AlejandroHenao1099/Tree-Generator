Shader "Custom/SebastianTerrain"
{
    Properties
    {
        _GrassColor ("Grass Color", Color) = (1,1,1,1)
        _RockColor ("Rock Color", Color) = (1,1,1,1)
        _GrassSlopeThresold ("Grass Slope Thresold", Range(0,1)) = .5
        _GrassBlendAmount ("Grass Blend Amount", Range(0,1)) = .5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        half _MaxHeight;
        half _GrassSlopeThresold;
        half _GrassBlendAmount;
        fixed4 _GrassColor;
        fixed4 _RockColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float slope = 1- IN.worldNormal.y; //slope = 0 when terrain is completely flat
            float grassBlendHeight= _GrassSlopeThresold * (1 - _GrassBlendAmount);
            float grassWeight = 1- saturate((slope-grassBlendHeight)/(_GrassSlopeThresold-grassBlendHeight));
            o.Albedo = _GrassColor * grassWeight + _RockColor * (1-grassWeight);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
