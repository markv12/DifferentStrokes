Shader "Unlit/Brush" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma target 2.0
                    #pragma multi_compile_fog

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                        UNITY_VERTEX_INPUT_INSTANCE_ID
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        float2 texcoord : TEXCOORD0;
                        UNITY_FOG_COORDS(1)
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    float4 _Color;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        UNITY_SETUP_INSTANCE_ID(v);
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                        UNITY_TRANSFER_FOG(o,o.vertex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
                        UNITY_APPLY_FOG(i.fogCoord, col);
                        return col;
                    }
                ENDCG
            }
    }

}
