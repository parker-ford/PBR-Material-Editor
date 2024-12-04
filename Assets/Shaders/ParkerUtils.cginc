#pragma once

#include "UnityCG.cginc"

#define PI 3.141592653589793238462

//TODO: Differentiate between clamped dot and epsilon dot
float clampedDot(float3 a, float3 b) {
    return max(dot(a, b), 0.0001);
}

float epsilonDot(float3 a, float3 b){
    return max(clampedDot(a,b), 0.0001);
}

int isPositive(float x){
    return x > 0;
}

float3 gammaToLinear(float3 col){
    return pow(col, 2.2);
}

float3 linearToGamma(float3 col){
    return pow(col, 1.0/2.2);
}

// TODO: Blue noise offset?
// TODO: Try these: https://github.com/panthuncia/webgl_test/blob/main/index.html
// https://developer.nvidia.com/gpugems/gpugems2/part-i-geometric-complexity/chapter-8-pixel-displacement-mapping-distance-functions
// view Direction should be passed in as tangent space
float2 parallaxMap(float2 uv, float3 viewDirection, sampler2D displacementTex, float displacementStrength){
    const int minSteps = 128;
    const int maxSteps = 128;
    viewDirection = normalize(viewDirection);
    int numSteps = lerp(maxSteps, minSteps, clampedDot(float3(0,0,1), viewDirection));
    float depthPerStep = 1.0 / (float)numSteps;
    float2 uvDelta = (viewDirection.xy * displacementStrength) / (float)numSteps;

    float2 currUV = uv;
    float currDepth = 1.0 - tex2D(displacementTex, currUV).r; //Inversed for depth rather than height
    float currStep = 0.0;

    [unroll(maxSteps)]
    while(currStep < currDepth){
        currUV -= uvDelta;
        currDepth = 1.0 - tex2D(displacementTex, currUV).r;
        currStep += depthPerStep;
    }

    float2 prevUV = currUV + uvDelta;

    float afterStep = currDepth - currStep;
    float beforeStep = 1.0 - tex2D(displacementTex, prevUV).r - currStep + depthPerStep;

    return lerp(currUV, prevUV, afterStep / (afterStep - beforeStep));
}

float3 normalMap(float3 normal, float3 tangent, float3 bitangent, float2 uv, sampler2D normalTex, float normalStrength){
    float3 tangentSpaceNormal = 0;
    tangentSpaceNormal.xy = tex2D(normalTex, uv).wy * 2 - 1;
    tangentSpaceNormal.xy *= normalStrength;
    tangentSpaceNormal.z = sqrt(1 - saturate(dot(tangentSpaceNormal.xy, tangentSpaceNormal.xy)));
    return normalize(tangentSpaceNormal.x * tangent + tangentSpaceNormal.y * bitangent + tangentSpaceNormal.z * normal);
}

float3 getBitangent(float3 normal, float3 tangent, float handedness){
    return cross(normal, tangent) * handedness * unity_WorldTransformParams.w;
}

float textureToPixels(float t, float size){
    return t * size - 0.5;
}

float pixelToTexture(float p, float size){
    return (p + 0.5) / size;
}