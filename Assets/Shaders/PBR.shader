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

            #define DEBUG_VIEW_DIFFUSE_MAP 60
            #define DEBUG_VIEW_NORMAL_MAP 70
            #define DEBUG_VIEW_DISPLACEMENT_MAP 80
            #define DEBUG_VIEW_ROUGHNESS_MAP 90
            #define DEBUG_VIEW_NORMAL 100
            #define DEBUG_VIEW_ROUGHNESS 110

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

            sampler2D _IntegratedBRDF;

            sampler2D _FilteredDiffuseMap;

            UNITY_DECLARE_TEX2DARRAY(_FilteredSpecularMap);
            int _SpecularMipLevels;

            float _NormalStrength;
            float _DisplacementStrength;

            float4 _LightColor;
            float3 _LightDirection;
            float _LightIntensity;
            float4 _AmbientColor;

            float4 _DiffuseColor;
            float _Roughness;
            float _Reflectance;
            float _Subsurface;
            float _Sheen;
            float _SheenTint;
            float _Clearcoat;
            float _ClearcoatGloss;
            float _Metallic;
            float _Anisotropic;
            float _TextureTiling;

            int _UseDisplacementMap;
            int _UseNormalMap;
            int _UseDiffuseMap;
            int _UseRoughnessMap;
            int _UseEnvironmentLighting;

            int _NDF;
            int _Geometry;
            int _Diffuse;
            int _DebugView;

            float3 specularImageBasedLighting(float reflectance, float roughness, float3 diffuseColor, float metallic, float3 n, float3 v){
                float3 F0 =  0.16 * (reflectance * reflectance);
                F0 = lerp(F0, diffuseColor, metallic);
                float ndotv = clamp(dot(n,v), 0.001, 0.999);
                roughness = clamp(roughness, 0.001, 0.999);
                float3 r = reflect(-v, n);
                float2 uv = directionToSphericalTexture(r);

                float sampleLevel = roughness * float(_SpecularMipLevels);
                float lowSample = floor(sampleLevel);
                float highSample = ceil(sampleLevel);
                float diff = frac(sampleLevel);

                float3 T1_low =  UNITY_SAMPLE_TEX2DARRAY(_FilteredSpecularMap, float3(uv, lowSample)).rgb;
                float3 T1_high =  UNITY_SAMPLE_TEX2DARRAY(_FilteredSpecularMap, float3(uv, highSample)).rgb;
                float3 T1 = T1_low * (1.0 - diff) + T1_high * diff;
                T1 = linearToGamma(T1);

                float4 brdfIntegration = tex2D(_IntegratedBRDF, float2(ndotv, roughness));
                float3 T2 = (F0 * brdfIntegration.x + brdfIntegration.y);

                return T1 * T2;
            }

            float3 diffuseImageBasedLighting(float3 n){
                float2 envUV = directionToSphericalTexture(n);
                return tex2D(_FilteredDiffuseMap, envUV).rgb;
            }

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

                float2 uv = i.uv * _TextureTiling;
                if(_DisplacementMapSet && _UseDisplacementMap){
                    uv = parallaxMap(uv, transformToTangentSpace(v, normal, tangent, bitangent), _DisplacementMap, _DisplacementStrength);
                }

                if(_DebugView == DEBUG_VIEW_DIFFUSE_MAP) return (_DiffuseMapSet && _UseDiffuseMap) ? tex2D(_DiffuseMap, uv) : float4(1,1,1,1);
                if(_DebugView == DEBUG_VIEW_NORMAL_MAP) return (_NormalMapSet && _UseNormalMap) ? tex2D(_NormalMap, uv): float4(1,1,1,1);
                if(_DebugView == DEBUG_VIEW_DISPLACEMENT_MAP) return (_DisplacementMapSet && _UseDisplacementMap) ? tex2D(_DisplacementMap, uv) : float4(1,1,1,1);
                if(_DebugView == DEBUG_VIEW_ROUGHNESS_MAP) return (_RoughnessMapSet && _UseRoughnessMap) ? tex2D(_RoughnessMap, uv) : float4(1,1,1,1);


                float3 n = normal;
                float3 x = tangent;
                float3 y = bitangent;
                if(_NormalMapSet && _UseNormalMap){
                    n = normalMap(normal, tangent, bitangent, uv, _NormalMap, _NormalStrength);
                    float3x3 tbn = getTBNMatrix(n);
                    x = tbn[0];
                    y = tbn[1];
                }
                if(_DebugView == DEBUG_VIEW_NORMAL) return float4(n, 1);            

                float ndotl = clampedDot(l, normalize(n));

                float3 diffuseColor = _DiffuseColor * ((_DiffuseMapSet && _UseDiffuseMap) ? tex2D(_DiffuseMap, uv) : 1);
                float roughness = _Roughness * ((_RoughnessMapSet && _UseRoughnessMap) ? tex2D(_RoughnessMap, uv).r : 1);
                if(_DebugView == DEBUG_VIEW_ROUGHNESS) return float4(roughness, roughness, roughness, 1);

                brdfParameters params;
                params.roughness = roughness;
                params.diffuseColor = diffuseColor;
                params.reflectance = _Reflectance;
                params.subsurface = _Subsurface;
                params.sheen = _Sheen;
                params.sheenTint = _SheenTint;
                params.clearcoat = _Clearcoat;
                params.clearcoatGloss = _ClearcoatGloss;
                params.metallic = _Metallic;
                params.anisotropic = _Anisotropic;

                brdfSettings settings;
                settings.ndf = _NDF;
                settings.geometry = _Geometry;
                settings.diffuse = _Diffuse;
                settings.debug = _DebugView;


                float3 lightOut = 0;
                if(_UseEnvironmentLighting){
                    float3 diffuseIBL = diffuseColor * diffuseImageBasedLighting(n) * (1.0 - _Metallic);
                    float3 specularIBL = specularImageBasedLighting(_Reflectance, _Roughness, diffuseColor, _Metallic, n, v);
                    float3 clearcoatIBL = 0.25 * _Clearcoat * specularImageBasedLighting(0.5, 1.0 - _ClearcoatGloss, float3(1,1,1), 0.0, n, v);
                    lightOut =  diffuseIBL + specularIBL + clearcoatIBL; 
                }
                else{
                    brdfResult brdf = PBR_BRDF(n,l,v,x,y, params, settings);
                    float3 lightIn =  _LightColor.rgb * _LightIntensity;
                    lightOut = lightIn * (brdf.specular + brdf.diffuse) * ndotl;
                }


                return float4(lightOut, 1.0);
            }
            ENDCG
        }
    }
}
