Shader "Parker/Lambertian"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            #define PI 3.141592653589793238462

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

            struct brdf
            {
                float3 diffuse;
                float3 specular;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _LightColor;
            float3 _LightDirection;
            float _LightIntensity;

            float4 _DiffuseColor;
            float _SpecularHardness;
            float _SpecularStrength;
            float _Roughness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                return o;
            }

            float clampedDot(float3 a, float3 b){
                return max(dot(a, b), 0.00001);
            }

            brdf LambertianBRDF(float3 l, float3 v, float3 n){
                brdf result;
                result.specular = 0.0;
                result.diffuse = 1.0;
                return result;
            }

            brdf BlinnPhongBRDF(float3 l, float3 v, float3 n){
                brdf result;
                float3 h = normalize(l + v);
                float ndoth = clampedDot(n, h);
                result.specular = pow(ndoth, _SpecularHardness) * _SpecularStrength;
                result.diffuse = 1.0;
                return result;
            }

            float3 SchlickFresnel(float3 n, float3 l){
                float3 F0 = float3(0.2,0.2,0.2);
                return F0 + (1 - F0) * pow((1 - (clampedDot(n,l))),5);
            }

            float BeckmanNDF(float3 n, float3 m){
                float ndotm = clampedDot(n,m);
                float ndotm2 = pow(ndotm, 2);
                float ndotm4 = pow(ndotm, 4);
                float a2 = pow(_Roughness, 2);
                return (ndotm / (PI * a2 * ndotm4)) * exp((ndotm2 - 1) / (a2 * ndotm2));
                // return ndotm / (PI * pow(_Roughness, 2) * pow(ndotm, 4)) * exp((pow(ndotm,2)) - 1) / (pow(_Roughness,2) * pow(ndotm, 2));
            }

            float BlinnPhongNDF(float3 n, float3 m){
                float a = pow(8192, _Roughness);
                float ndotm = clampedDot(n,m);
                return ndotm * ((a + 2) / (2 * PI)) * pow(ndotm, a);
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.worldNormal);
                float3 l = normalize(_LightDirection.xyz);
                float3 v = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 h = normalize(l + v);
                float ndotl = clampedDot(l, normalize(i.worldNormal));
                brdf f = BlinnPhongBRDF(l, v, normalize(i.worldNormal));
                float3 Li =  _LightColor.rgb * _LightIntensity; 
                float3 Lo = Li * (f.specular + f.diffuse * _DiffuseColor) * ndotl;
                float3 fresnel = SchlickFresnel(n, l);
                float3 ndf = BeckmanNDF(n,h);
                // float3 ndf = BlinnPhongNDF(n,h);
                return float4(ndf, 1.0);
                // return float4(Lo, 1.0);
            }
            ENDCG
        }
    }
}
