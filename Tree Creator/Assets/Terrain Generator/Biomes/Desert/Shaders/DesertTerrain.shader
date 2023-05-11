Shader "Custom/DesertTerrain"
{
    Properties
    {
        // _Period ("Period X", float) = 1
        // _PeriodZ ("Period Z", float) = 1
        // _AmplitudeZ ("Amplitude", float) = 1
        // _MainColor("Main Color", Color) = (1,1,1,1)
        // _SecondaryColor("Secondary Color", Color) = (1,1,1,1)

        _SandColor ("Sand Color", Color) = (1,1,1,1)
        _RockColor ("Rock Color", Color) = (1,1,1,1)
        _SandSlopeThresold ("Sand Slope Thresold", Range(0,1)) = 0.5
        _SandBlendAmount ("Sand Blend Amount", Range(0,1)) = 0.5
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
        #define TAU 6.28318530718

        struct Input
        {
            fixed3 worldPos;
            fixed3 worldNormal;
        };

        // fixed _Period;
        // fixed _PeriodZ;
        // fixed _AmplitudeZ;

        half _SandSlopeThresold;
        half _SandBlendAmount;
        fixed4 _SandColor;
        fixed4 _RockColor;

        // fixed4 _MainColor;
        // fixed4 _SecondaryColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float slope = 1 - IN.worldNormal.y; //slope = 0 when terrain is completely flat
            float grassBlendHeight= _SandSlopeThresold * (1 - _SandBlendAmount);
            float grassWeight = 1- saturate((slope-grassBlendHeight)/(_SandSlopeThresold-grassBlendHeight));
            o.Albedo = _SandColor * grassWeight + _RockColor * (1-grassWeight);

            // fixed offset = sin(IN.worldPos.z * TAU * _PeriodZ) * _AmplitudeZ;
            // fixed coordinate = abs(sin((IN.worldPos.x * TAU * _Period) + offset));

            // fixed4 lerpColor = lerp(_MainColor, _SecondaryColor, coordinate);
            // fixed dotUp = dot(IN.worldNormal, fixed3(0,1,0));
            // fixed dotWind = dot(IN.worldNormal, fixed3(0,0,1));
            // fixed4 multiplier = _MainColor;
            // // o.Albedo = lerpColor;
            // if(dotWind > 0)
            //     o.Albedo = _MainColor;
            // else if(dotUp > 0.8)
            //     o.Albedo = lerpColor;
            // else
            //     o.Albedo = lerpColor;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
