Shader "Unlit/TestTextures"
{
    Properties
    {
        _DiffuseTex ("Diffuse Texture", 2D) = "white" {}
        _NormalTex ("Normal Texture", 2D) = "white" {}
        _DisplacementTex ("Displacement Texture", 2D) = "white" {}
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos: TEXCOORD1;
                float3 normal: TEXCOORD02;
                float4 tangent: TEXCOORD3;
            };

            struct brdf
            {
                float3 diffuse;
                float3 specular;
            };

            sampler2D _DisplacementTex;
            float4 _DisplacementTex_ST;

            sampler2D _NormalTex;
            float4 _NormalTex_ST;

            sampler2D _DiffuseTex;
            float4 _DiffuseTex_ST;

            float3 _LightDirection;

            float _SpecularHardness;
            float _SpecularStrength;

            float _NormalStrength;

            int _UseNormalMap;

            float clampedDot(float3 a, float3 b) {
                return max(dot(a, b), 0.0001);
            }

            brdf BlinnPhongBRDF(float3 l, float3 v, float3 n){
                brdf result;
                float3 h = normalize(l + v);
                float ndoth = clampedDot(n, h);
                result.specular = pow(ndoth, _SpecularHardness) * _SpecularStrength;
                result.diffuse = 1.0;
                return result;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _DiffuseTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


                
                float3 normal = normalize(i.normal);
                float3 tangent = normalize(i.tangent);

                float3 tangentSpaceNormal = 0;
                tangentSpaceNormal.xy = tex2D(_NormalTex, i.uv).wy * 2 - 1;
                tangentSpaceNormal.xy *= _NormalStrength;
                tangentSpaceNormal.z = sqrt(1 - saturate(dot(tangentSpaceNormal.xy, tangentSpaceNormal.xy)));

                float3 bitangent = cross(normal, tangent) * i.tangent.w * unity_WorldTransformParams.w;

                float3 n = 0;
                if(_UseNormalMap){
                    n = normalize(tangentSpaceNormal.x * tangent + tangentSpaceNormal.y * bitangent + tangentSpaceNormal.z * normal);
                }
                else{
                    n = normal;
                }


                float3 l = normalize(_LightDirection);
                float3 v = normalize(_WorldSpaceCameraPos - i.worldPos);

                brdf result = BlinnPhongBRDF(l, v, n);
                float ndotl = max(dot(n,l), 0.00001);
                float3 col = (result.specular + tex2D(_DiffuseTex, i.uv) * result.diffuse);
                return float4(col * ndotl + col * 0.0, 1.0);
            }
            ENDCG
        }
    }
}
