Shader "Parker/PBR"
{
    Properties
    {
        _DiffuseMap ("Diffuse Map", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _DisplacementMap ("Displacement Map", 2D) = "white" {}
        _RoughnessMap ("Roughness Map", 2D) = "white" {}
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

            #define DEBUG_VIEW_DIFFUSE_MAP 6
            #define DEBUG_VIEW_NORMAL_MAP 7
            #define DEBUG_VIEW_DISPLACEMENT_MAP 8
            #define DEBUG_VIEW_ROUGHNESS_MAP 9
            #define DEBUG_VIEW_NORMAL 10

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 tangent : TEXCOORD3;
            };

            sampler2D _DiffuseMap;
            int _DiffuseMapSet;

            sampler2D _NormalMap;
            int _NormalMapSet;

            sampler2D _DisplacementMap;
            int _DisplacementMapSet;

            sampler2D _RoughnessMap;
            int _RoughnessMapSet;

            float _NormalStrength;
            float _DisplacementStrength;

            float4 _LightColor;
            float3 _LightDirection;
            float _LightIntensity;

            float4 _DiffuseColor;
            float _Roughness;
            float _Reflectance;
            float _Subsurface;

            int _UseDisplacementMap;
            int _UseNormalMap;
            int _UseDiffuseMap;
            int _UseRoughnessMap;

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
                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float3 l = normalize(_LightDirection);
                float3 v = normalize(_WorldSpaceCameraPos - i.worldPos);

                float3 normal = normalize(i.normal);
                float3 tangent = normalize(i.tangent);
                float3 bitangent = getBitangent(normal, tangent, i.tangent.w);

                float2 uv = i.uv;
                if(_DisplacementMapSet && _UseDisplacementMap){
                    uv = parallaxMap(uv, mul(v, float3x3(tangent, bitangent, normal)), _DisplacementMap, _DisplacementStrength);
                }

                if(_DebugView == DEBUG_VIEW_DIFFUSE) return _DiffuseMapSet ? tex2D(_DiffuseMap, uv) : float4(1,1,1,1);
                if(_DebugView == DEBUG_VIEW_NORMAL_MAP) return _NormalMapSet ? tex2D(_NormalMap, uv): float4(1,1,1,1);
                if(_DebugView == DEBUG_VIEW_DISPLACEMENT_MAP) return _DisplacementMapSet ? tex2D(_DiffuseMap, uv) : float4(1,1,1,1);
                if(_DebugView == DEBUG_VIEW_ROUGHNESS_MAP) return _RoughnessMapSet ? tex2D(_RoughnessMap, uv) : float4(1,1,1,1);


                float3 n = normal;
                if(_NormalMapSet && _UseNormalMap){
                    n = normalMap(normal, tangent, bitangent, uv, _NormalMap, _NormalStrength);
                }
                if(_DebugView == DEBUG_VIEW_NORMAL) return float4(n, 1);





                float3 diffuseMapSample = 1;
                if(_DiffuseMapSet){
                    diffuseMapSample = tex2D(_DiffuseMap, uv).rgb;
                }
                if(_DebugView == DEBUG_VIEW_DIFFUSE_MAP) return float4(diffuseMapSample, 1);

                float3 normalMapSample = 1;
                if(_NormalMapSet){
                    normalMapSample = tex2D(_NormalMap, uv).rgb;
                }
                if(_DebugView == DEBUG_VIEW_NORMAL_MAP) return float4(normalMapSample, 1);

                float displacementMapSample = 1;
                if(_DisplacementMapSet){
                    displacementMapSample = tex2D(_DisplacementMap, uv).r;
                }
                if(_DebugView == DEBUG_VIEW_DISPLACEMENT_MAP) return float4(displacementMapSample, displacementMapSample, displacementMapSample, 1);

                float roughnessMapSample = 1;
                if(_RoughnessMapSet){
                    roughnessMapSample = tex2D(_RoughnessMap, uv).r;
                }
                if(_DebugView == DEBUG_VIEW_ROUGHNESS_MAP) return float4(roughnessMapSample, roughnessMapSample, roughnessMapSample, 1);



                

                float ndotl = clampedDot(l, normalize(n));

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
