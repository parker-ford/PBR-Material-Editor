Shader "Parker/PBR"
{
    Properties
    {
        _DiffuseMap ("Diffuse Map", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _DisplacementMap ("Displacement Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./ParkerPBR.cginc"
            #include "./ParkerUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal: TEXCOORD1;
                float3 worldPos: TEXCOORD2;
            };

            sampler2D _DiffuseMap;
            sampler2D _NormalMap;
            sampler2D _DisplacementMap;

            float _NormalStrength;
            float _DisplacementStrength;

            float4 _LightColor;
            float3 _LightDirection;
            float _LightIntensity;

            float4 _DiffuseColor;
            float _Roughness;
            float _Reflectance;
            float _Subsurface;

            int _NDF;
            int _Geometry;
            int _Diffuse;
            int _DebugView;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float3 n = normalize(i.worldNormal);
                float3 l = normalize(_LightDirection);
                float3 v = normalize(_WorldSpaceCameraPos - i.worldPos);

                float ndotl = clampedDot(l, normalize(i.worldNormal));

                brdfParameters params;
                params.roughness = _Roughness;
                params.diffuseColor = _DiffuseColor;
                params.reflectance = _Reflectance;
                params.subsurface = _Subsurface;

                brdfSettings settings;
                settings.ndf = _NDF;
                settings.geometry = _Geometry;
                settings.diffuse = _Diffuse;
                settings.debug = _DebugView;

                brdfResult brdf = PBR_BRDF(n,l,v, params, settings);

                float3 lightIn =  _LightColor.rgb * _LightIntensity;

                float3 lightOut = lightIn * (brdf.specular + brdf.diffuse * _DiffuseColor) * ndotl;

                return float4(lightOut, 1.0);
            }
            ENDCG
        }
    }
}
