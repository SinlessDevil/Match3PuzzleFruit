Shader "Custom/SpriteOutlineGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        _OutlineAlpha ("Outline Alpha", Range(0, 1)) = 1
        
        [Header(Glow Settings)]
        _GlowColor ("Glow Color", Color) = (1,0,0,1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _GlowPower ("Glow Power", Range(0.1, 10)) = 2
        _GlowAlpha ("Glow Alpha", Range(0, 1)) = 0.5
        
        [Header(Animation Settings)]
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.3
        _OutlinePulse ("Outline Pulse", Range(0, 1)) = 0
        _GlowPulse ("Glow Pulse", Range(0, 1)) = 0
        
        [Header(Features)]
        [Toggle] _EnableOutline ("Enable Outline", Float) = 1
        [Toggle] _EnableGlow ("Enable Glow", Float) = 1
        [Toggle] _EnablePulse ("Enable Pulse", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineAlpha;
            
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowPower;
            float _GlowAlpha;
            
            float _PulseSpeed;
            float _PulseIntensity;
            float _OutlinePulse;
            float _GlowPulse;
            
            float _EnableOutline;
            float _EnableGlow;
            float _EnablePulse;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Базовый цвет спрайта
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Пульсация
                float pulse = 1.0;
                if (_EnablePulse > 0.5)
                {
                    pulse = 1.0 + _PulseIntensity * sin(_Time.y * _PulseSpeed);
                }
                
                // Обводка
                if (_EnableOutline > 0.5)
                {
                    float2 texelSize = _MainTex_TexelSize.xy;
                    float2 uv = IN.texcoord;
                    
                    // Проверяем соседние пиксели
                    float4 outline = 0;
                    float outlineAlpha = 0;
                    
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            
                            float2 offset = float2(x, y) * texelSize * _OutlineWidth;
                            float4 neighbor = tex2D(_MainTex, uv + offset);
                            
                            // Если соседний пиксель прозрачный, а текущий нет
                            if (neighbor.a < 0.1 && c.a > 0.1)
                            {
                                outlineAlpha = max(outlineAlpha, _OutlineAlpha);
                            }
                        }
                    }
                    
                    if (outlineAlpha > 0)
                    {
                        float outlinePulse = _OutlinePulse > 0.5 ? pulse : 1.0;
                        c.rgb = lerp(c.rgb, _OutlineColor.rgb * outlinePulse, outlineAlpha);
                    }
                }
                
                // Глоу эффект
                if (_EnableGlow > 0.5)
                {
                    float2 texelSize = _MainTex_TexelSize.xy;
                    float2 uv = IN.texcoord;
                    float glowSum = 0;
                    
                    // Собираем глоу от соседних пикселей
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            
                            float2 offset = float2(x, y) * texelSize * 2;
                            float4 neighbor = tex2D(_MainTex, uv + offset);
                            
                            if (neighbor.a > 0.1)
                            {
                                float distance = length(float2(x, y));
                                float glow = neighbor.a * _GlowIntensity / pow(distance, _GlowPower);
                                glowSum += glow;
                            }
                        }
                    }
                    
                    if (glowSum > 0)
                    {
                        float glowPulse = _GlowPulse > 0.5 ? pulse : 1.0;
                        float glowAlpha = min(glowSum * _GlowAlpha * glowPulse, 1.0);
                        c.rgb = lerp(c.rgb, _GlowColor.rgb, glowAlpha * _GlowColor.a);
                    }
                }
                
                return c;
            }
            ENDCG
        }
    }
}

