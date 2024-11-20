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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _LightColor;
            float3 _LightDirection;
            float _LightIntensity;

            float4 _DiffuseColor;
            float _SpecularHardness;

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

            float3 LambertianBRDF(float3 l, float3 v, float3 n){
                return (1.0/PI) * _DiffuseColor.rgb;
            }
            float3 BlinnPhongBRDF(float3 l, float3 v, float3 n){
                float3 h = normalize(l + v);
                float ndoth = clampedDot(n, h);
                return pow(ndoth, _SpecularHardness) * _LightColor;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float3 l = normalize(_LightDirection.xyz);
                float3 v = normalize(_WorldSpaceCameraPos - i.worldPos);
                float ndotl = clampedDot(l, i.worldNormal);
                float3 f = BlinnPhongBRDF(l, v, i.worldNormal);
                float3 Li =  _LightColor.rgb * _LightIntensity; 
                float3 Lo = f * Li * ndotl;
                return float4(Lo, 1.0);
            }
            ENDCG
        }
    }
}
