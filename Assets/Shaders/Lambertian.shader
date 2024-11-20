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

            #define NDF_BLINNPHONG 0
            #define NDF_BECKMAN 1
            #define NDF_GGX 2

            #define GEO_BECKMAN 0
            #define GEO_GGX 1
            #define GEO_GGXSCHLICK 2

            #define DEBUG_VIEW_NDF 1
            #define DEBUG_VIEW_GEO_ATTEN 2
            #define DEBUG_VIEW_FRESNEL 3

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

            int _NDF;
            int _GEO;
            int _DebugView;

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

            float3 SchlickFresnel(float3 v, float3 n){
                //TODO: Figure out how to determine f0
                float3 F0 = 0.06;
                return F0 + (1 - F0) * pow((1 - (clampedDot(v,n))),5);
            }

            float BeckmanNDF(float3 n, float3 m, float alpha){
                float ndotm = clampedDot(n,m);
                float ndotm2 = pow(ndotm, 2);
                float ndotm4 = pow(ndotm, 4);
                float alpha2 = pow(alpha, 2);
                return (ndotm / (PI * alpha2 * ndotm4)) * exp((ndotm2 - 1) / (alpha2 * ndotm2));
            }

            float BlinnPhongNDF(float3 n, float3 m, float alpha){
                float alpha2 = pow(alpha, 2);
                float alphaMinus2 = pow(alpha, -2);
                float ndotm = clampedDot(n,m);
                return (1 / (PI * alpha2)) * pow(ndotm, 2 * alphaMinus2 - 2);

            }

            float GgxNDF(float3 n, float3 m, float alpha){
                float ndotm = clampedDot(n, m);
                float ndotm2 = pow(ndotm,2);
                float alpha2 = pow(alpha, 2);
                return (ndotm * alpha2) / pow((PI * (1 + ndotm2 * (alpha2 - 1))),2);
            }

            float BeckmanGeometry(float3 n, float3 v, float alpha){
                float ndotv = clampedDot(n, v);
                float ndotv2 = pow(ndotv,2);
                float c = ndotv / (alpha * sqrt(1 - ndotv2));
                float c2 = pow(c, 2);
                float result = 1;
                if(c < 1.6){
                    result = (3.535 * c + 2.181 * c2) / (1 + 2.276 * c + 2.577 * c2);
                }                
                return result;
            }

            float GgxGeometry(float3 n, float3 v, float alpha){
                float ndotv = clampedDot(n,v);
                float ndotv2 = pow(ndotv,2);
                float alpha2 = pow(alpha, 2);
                
                return (2 * ndotv) / (ndotv + sqrt(alpha2 + (1-alpha2)*ndotv2));
            }


            brdf PBR_BRDF(float3 n, float3 l, float3 v){
                brdf result;
                result.specular = 0;
                result.diffuse = 0;

                float3 h = normalize(l + v);
                float alpha = pow(_Roughness, 2);

                //Fresnel Term
                float3 fresnel = SchlickFresnel(h,v);

                //NDF Term
                float3 normalDistribution = 0;
                if(_NDF == NDF_BLINNPHONG){
                    normalDistribution = BlinnPhongNDF(n,h,alpha);
                }
                else if(_NDF == NDF_BECKMAN){
                    normalDistribution = BeckmanNDF(n,h,alpha);
                }
                else if(_NDF == NDF_GGX){
                    normalDistribution = GgxNDF(n,h,alpha);
                }

                //Gemoetry Term
                float3 geometryAttenuation = 0;
                if(_GEO == GEO_BECKMAN){
                    geometryAttenuation = BeckmanGeometry(n, v, alpha) * BeckmanGeometry(n, l, alpha);
                }
                else if(_GEO == GEO_GGX){
                    geometryAttenuation = GgxGeometry(n, v, alpha) * GgxGeometry(n, l, alpha);
                }
                else if(_GEO == GEO_GGXSCHLICK){

                }

                 if(_DebugView == DEBUG_VIEW_FRESNEL){
                    result.specular = fresnel;
                 }
                 else if(_DebugView == DEBUG_VIEW_NDF){
                    result.specular = normalDistribution;
                 }
                 else if(_DebugView == DEBUG_VIEW_GEO_ATTEN){
                    result.specular = geometryAttenuation;
                 }
                 else{
                     result.specular = (fresnel * normalDistribution * geometryAttenuation) / (4 * clampedDot(n,l) * clampedDot(n,v));
                 }

                return result;

            }



            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.worldNormal);
                float3 l = normalize(_LightDirection.xyz);
                float3 v = normalize(_WorldSpaceCameraPos - i.worldPos);
                brdf f = PBR_BRDF(n,l,v);
                float ndotl = clampedDot(l, normalize(i.worldNormal));
                float3 Li =  _LightColor.rgb * _LightIntensity; 
                float3 Lo = Li * (f.specular + f.diffuse * _DiffuseColor) * ndotl;
                return float4(Lo, 1.0);
            }
            ENDCG
        }
    }
}
