#pragma once

#include "./ParkerUtils.cginc"

#define NDF_BLINNPHONG 0
#define NDF_BECKMAN 1
#define NDF_GGX 2

#define GEO_BECKMAN 0
#define GEO_GGX 1
#define GEO_GGXSCHLICK 2

#define DIFF_LAMBERT 0
#define DIFF_HAMMON 1
#define DIFF_DISNEY 2

#define DEBUG_VIEW_NDF 1
#define DEBUG_VIEW_GEO_ATTEN 2
#define DEBUG_VIEW_FRESNEL 3
#define DEBUG_VIEW_DIFFUSE 4
#define DEBUG_VIEW_SPECULAR 5
#define DEBUG_VIEW_SHEEN 6
#define DEBUG_VIEW_CLEARCOAT 7
#define DEBUG_VIEW_CLEARCOAT_GLOSS 8

struct brdfResult
{
    float3 diffuse;
    float3 specular;
};

struct brdfParameters
{
    float roughness;
    float3 diffuseColor;
    float reflectance;
    float subsurface;
    float sheen;
    float sheenTint;
    float clearcoat;
    float clearcoatGloss;
};

struct brdfSettings
{
    int ndf;
    int geometry;
    int diffuse;
    int debug;
};

float3 SchlickFresnelReflectance(float3 v, float3 n, float reflectance, float3 diffuseColor){
    //TODO: Figure out how to do metallic
    float3 F0 = (0.16 * (reflectance * reflectance));
    F0 = lerp(F0, diffuseColor, 0);
    return F0 + (1 - F0) * pow((1 - (clampedDot(v,n))),5);
}

float SchlickFresnel2(float3 v, float3 n){
    float vdotn = 1.0 - clampedDot(v,n);
    float vdotn2 = vdotn * vdotn;
    return vdotn2 * vdotn2 * vdotn;
}

/*
*   Normal Distriubtion Functions
*/
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


/*
*   Geometry Attenuation Functions
*/
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

// TODO: look into alpha_g = (0.5 + roughness/2)^2 . Section 5.6 in disney paper.
float GgxGeometry(float3 n, float3 v, float alpha){
    float ndotv = clampedDot(n,v);
    float ndotv2 = pow(ndotv,2);
    float alpha2 = pow(alpha, 2);
    
    return (2 * ndotv) / (ndotv + sqrt(alpha2 + (1-alpha2)*ndotv2));
}

float GgxSchlickGeometry(float3 n, float3 v, float alpha){
    float k = alpha / 2.0;
    float ndotv = clampedDot(n,v);
    return ndotv / (ndotv * (1 - k) + k);
}

float GgxSchlickGeometry(float ndotv, float alpha){
    float k = alpha / 2.0;
    return ndotv / (ndotv * (1 - k) + k);
}


/*
*   Diffuse Models
*/
float3 LambertianDiffuse(){
    return 1.0 / PI;
}

float3 HammonDiffuse(float3 n, float3 l, float3 v, float3 h, float alpha, brdfParameters params){
    float F0 = (0.16 * (params.reflectance * params.reflectance));
    float ndotl = clampedDot(n,l);
    float invNdotl5 = pow(1.0 - ndotl, 5.0);
    float ndotv = clampedDot(n,v);
    float invNdotv5 = pow(1.0 - ndotv, 5);
    float smooth = (21.0/20.0) * (1.0 - F0) * (1.0 - invNdotl5) * (1.0 - invNdotv5);

    float ldotv = clampedDot(l, v);
    float facing = 0.5f + 0.5f * ldotv;

    float multi = 0.3641 * alpha;

    float ndoth = clampedDot(n,h);
    float ndothE = epsilonDot(n,h);
    float rough = facing * (0.9 - 0.4 * facing) * ((0.5 + ndoth) / ndothE);

    return isPositive(ndotl) * isPositive(ndotv) * (1.0 / PI) * ((1.0 - alpha) * smooth + alpha * rough + params.diffuseColor * multi);
}

float3 DisneyDiffuse(float3 n, float3 l, float3 v, float alpha, brdfParameters params){
    float3 h = normalize(l + v);

    float hdotl = clampedDot(h,l); //theta d
    float ndotl = clampedDot(n,l); //theta l
    float ndotv = clampedDot(n,v); //theta v

    ndotl = max(ndotl, 0.001);
    ndotv = max(ndotv, 0.001);

    float invNdotL = 1.0 - ndotl;
    float invNdotV = 1.0 - ndotv;

    float invNdotL5 = invNdotL * invNdotL * invNdotL * invNdotL * invNdotL;
    float invNdotV5 = invNdotV * invNdotV * invNdotV * invNdotV * invNdotV;

    float hdotl2 = hdotl * hdotl;

    float FSS90 = sqrt(alpha) * hdotl2;
    float FD90 = 0.5 + 2.0 * FSS90;

    float fd = (1.0 + (FD90 - 1.0) * invNdotL5) * (1.0 + (FD90 - 1.0) * invNdotV5);
    
    float FSS = (1.0 + (FSS90 - 1.0) * invNdotL5) * (1.0 + (FSS90 - 1.0) * invNdotV5);
    float fss = (1.0 / (ndotl + ndotv) - 0.5f) * FSS + 0.5;

    // TODO: Double check that  sub surface is correct
    return (1.0 / PI) * (1.0 - params.subsurface) * fd + 1.25 * params.subsurface * fss;
}


/*
*   BRDF
*/
brdfResult PBR_BRDF(float3 n, float3 l, float3 v, brdfParameters params, brdfSettings settings){
    brdfResult result;
    result.specular = 0;
    result.diffuse = 0;

    float3 h = normalize(l + v);
    float alpha = pow(params.roughness, 2);

    //Fresnel Term
    float3 fresnel = SchlickFresnelReflectance(h, v, params.reflectance, params.diffuseColor);

    //NDF Term
    float3 normalDistribution = 0;
    if(settings.ndf == NDF_BLINNPHONG){
        normalDistribution = BlinnPhongNDF(n,h,alpha);
    }
    else if(settings.ndf == NDF_BECKMAN){
        normalDistribution = BeckmanNDF(n,h,alpha);
    }
    else if(settings.ndf == NDF_GGX){
        normalDistribution = GgxNDF(n,h,alpha);
    }

    //Gemoetry Term
    float3 geometryAttenuation = 0;
    if(settings.geometry == GEO_BECKMAN){
        geometryAttenuation = BeckmanGeometry(n, v, alpha) * BeckmanGeometry(n, l, alpha);
    }
    else if(settings.geometry == GEO_GGX){
        geometryAttenuation = GgxGeometry(n, v, alpha) * GgxGeometry(n, l, alpha);
    }
    else if(settings.geometry == GEO_GGXSCHLICK){
        geometryAttenuation = GgxSchlickGeometry(n, v, alpha) * GgxSchlickGeometry(n, l, alpha);
    }

    result.specular = (fresnel * normalDistribution * geometryAttenuation) / (4 * clampedDot(n,l) * clampedDot(n,v));

     //Diffuse
    if(settings.diffuse == DIFF_LAMBERT){
        result.diffuse = (1.0 - fresnel) * LambertianDiffuse();
    }
    else if(settings.diffuse == DIFF_HAMMON){
        result.diffuse = HammonDiffuse(n, l, v, h, alpha, params);
    }
    else if(settings.diffuse == DIFF_DISNEY){
        result.diffuse = DisneyDiffuse(n, l, v, alpha, params);
    }
    result.diffuse *= params.diffuseColor;

    // Sheen
    // float3 fSheen = SchlickFresnel2(n, v) * params.sheen* lerp(float3(1,1,1), params.diffuseColor, params.sheenTint);
    float3 fSheen = SchlickFresnel2(h, l) * params.sheen * lerp(float3(1,1,1), params.diffuseColor, params.sheenTint); // From Disney
    result.diffuse += fSheen;

    // Clear Coat
    //Clear coat reflectance 0.5 (F0 = 0.04)
    float clearcoatNDF = GgxNDF(n,h,lerp(.1, .001, params.clearcoatGloss));
    float clearcoatFresnel = SchlickFresnelReflectance(h, l, 0.5, float3(1,1,1)); // Half and light or half and view. White color?
    float clearcoatGeometry =  GgxSchlickGeometry(n, v, 0.25) * GgxSchlickGeometry(n, l, 0.25);
    float clearSpecular = 0.25 * params.clearcoat * clearcoatGeometry * clearcoatFresnel * clearcoatNDF;
    result.specular += clearSpecular;

    // Debug
    if(settings.debug == DEBUG_VIEW_DIFFUSE){
        result.specular = 0;
    }
    else if(settings.debug == DEBUG_VIEW_SPECULAR){
        result.diffuse = 0;
    }
    else if(settings.debug == DEBUG_VIEW_SHEEN){
        result.specular = 0;
        result.diffuse = fSheen;
    }
    else if(settings.debug == DEBUG_VIEW_FRESNEL){
        result.specular = fresnel;
        result.diffuse = 0;
    }
    else if(settings.debug == DEBUG_VIEW_NDF){
        result.specular = normalDistribution;
        result.diffuse = 0;
    }
    else if(settings.debug == DEBUG_VIEW_GEO_ATTEN){
        result.specular = geometryAttenuation;
        result.diffuse = 0;
    }
    else if(settings.debug == DEBUG_VIEW_CLEARCOAT){
        result.specular = clearSpecular;
        result.diffuse = 0;
    }
    else if(settings.debug == DEBUG_VIEW_CLEARCOAT_GLOSS){
        result.specular = clearcoatNDF;
        result.diffuse = 0;
    }

    return result;

}
