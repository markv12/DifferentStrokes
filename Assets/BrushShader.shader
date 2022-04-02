// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Brush" {
    Properties{
        _Color("Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
    }

        Category{
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            Cull Off Lighting Off ZWrite Off

            SubShader {
                Pass {

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma target 2.0
                    #pragma multi_compile_particles
                    #pragma multi_compile_fog

                    #include "UnityCG.cginc"

                    sampler2D _MainTex;
                    fixed4 _Color;

                    struct appdata_t {
                        float4 vertex : POSITION;
                        fixed4 color : COLOR;
                        float2 texcoord : TEXCOORD0;
                        UNITY_VERTEX_INPUT_INSTANCE_ID
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        fixed4 color : COLOR;
                        float2 texcoord : TEXCOORD0;
                        UNITY_FOG_COORDS(1)
                        UNITY_VERTEX_OUTPUT_STEREO
                    };

                    float4 _MainTex_ST;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        UNITY_SETUP_INSTANCE_ID(v);
                        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                        o.vertex = UnityObjectToClipPos(v.vertex);
    
                        o.color = v.color * _Color;
                        o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                        UNITY_TRANSFER_FOG(o,o.vertex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                        fixed4 col = i.color * tex2D(_MainTex, i.texcoord);
                        col.a = saturate(col.a); // alpha should not have double-brightness applied to it, but we can't fix that legacy behavior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)

                        UNITY_APPLY_FOG(i.fogCoord, col);
                        return col;
                    }
                    ENDCG
                }
            }
        }
}
