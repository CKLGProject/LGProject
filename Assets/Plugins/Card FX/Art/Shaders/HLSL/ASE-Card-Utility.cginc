// inline float4 MixColorsWithAlphaLimit(float4 baseColor, float4 overlayColor, float alphaLimit)
// {
//     float4 mixedColor = baseColor * overlayColor;
//     mixedColor.a = min(mixedColor.a, alphaLimit);
//     return mixedColor;
// }
static const float PI = 3.14159265f;
float CalculateRectangleDistance(float2 position, float2 halfDimensions)
{
    float2 distanceToEdges = abs(position) - halfDimensions;
    float distanceOutside = length(max(distanceToEdges, 0));
    float distanceInside = min(max(distanceToEdges.x, distanceToEdges.y), 0);
    return distanceOutside + distanceInside;
}

float CalculateRoundedRectangleDistance(float2 position, float roundness, float2 halfDimensions)
{
    return CalculateRectangleDistance(position, halfDimensions - roundness) - roundness;
}

float AntialiasedCutoff(float distance)
{
    float distanceChange = fwidth(distance) * 0.5;
    return smoothstep(distanceChange, -distanceChange, distance);
}

float CalculateAlpha(float2 position, float width, float height, float radius)
{
    float2 translatedPosition = (position - 0.5) * float2(width, height);
    float distToRect = CalculateRoundedRectangleDistance(translatedPosition, radius * 0.5, float2(width, height) * 0.5);
    return AntialiasedCutoff(distToRect);
}

inline float2 TranslatePosition(float2 position, float2 offset)
{
    return position - offset;
}

float IntersectShapes(float shape1, float shape2)
{
    return max(shape1, shape2);
}

float2 RotatePosition(float2 position, float rotation)
{
    float angle = rotation * PI * 2 * -1;
    float sine, cosine;
    sincos(angle, sine, cosine);
    return float2(cosine * position.x + sine * position.y,
                  cosine * position.y - sine * position.x);
}

float CalculateCircleDistance(float2 position, float radius)
{
    return length(position) - radius;
}

// float CalculateAlphaForIndependentCorners(float2 position, float2 halfDimensions, float4 rectangleProperties, float4 cornerRadii)
// {
//     position = (position - 0.5) * halfDimensions * 2;
//
//     float rectangle1Distance = CalculateRectangleDistance(position, halfDimensions);
//     float2 rectangle2Position = RotatePosition(TranslatePosition(position, rectangleProperties.xy), 0.125);
//     float rectangle2Distance = CalculateRectangleDistance(rectangle2Position, rectangleProperties.zw);
//     float2 circle0Position = TranslatePosition(position, float2(-halfDimensions.x + cornerRadii.x, halfDimensions.y - cornerRadii.x));
//     float circle0Distance = CalculateCircleDistance(circle0Position, cornerRadii.x);
//     float2 circle1Position = TranslatePosition(position, float2(halfDimensions.x - cornerRadii.y, halfDimensions.y + cornerRadii.x));
//     float circle1Distance = CalculateCircleDistance(circle1Position, cornerRadii.y);
//     float2 circle2Position = TranslatePosition(position, float2(halfDimensions.x - cornerRadii.z, -halfDimensions.y + cornerRadii.z));
//     float circle2Distance = CalculateCircleDistance(circle2Position, cornerRadii.z);
//     float2 circle3Position = TranslatePosition(position, -halfDimensions + cornerRadii.w);
//     float circle3Distance = CalculateCircleDistance(circle3Position, cornerRadii.w);
//
//     float maxDistance = max(rectangle1Distance, min(min(min(min(rectangle2Distance, circle0Distance), circle1Distance), circle2Distance), circle3Distance));
//     return AntialiasedCutoff(maxDistance);
// }

float ResultAlpha_float(float width, float height, float radius, float2 uv)
{
    float alpha = CalculateAlpha(uv, width, height, radius);
    return alpha;
}

half ResultAlpha_half(half width, half height, half radius, float2 uv)
{
    float alpha = CalculateAlpha(uv, width, height, radius);
    return alpha;
}
