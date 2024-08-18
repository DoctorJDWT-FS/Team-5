Shader "Custom/TransparentEdgeShader"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)
        _EdgeThickness ("Edge Thickness", Range(0,1)) = 0.1
        _Alpha ("Alpha", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _EdgeColor;
            float _EdgeThickness;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 edgeDist = abs(i.uv - 0.5) * 2.0;
                float edgeFactor = smoothstep(1.0 - _EdgeThickness, 1.0, max(edgeDist.x, edgeDist.y));
                half4 edgeColor = _EdgeColor;
                edgeColor.a *= edgeFactor * _Alpha; // Multiply alpha by edge factor and shader alpha
                return edgeColor;
            }
            ENDCG
        }
    }
}
