// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TransparentWireframe" {
	Properties
	{
		_WireColor("WireColor", Color) = (1,0,0,1)
		_Color("Color", Color) = (1,1,1,1)
		_WireThickness("Wire thickness", Range(0, 800)) = 100
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
	{
		
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#include "UnityCG.cginc"
#pragma target 5.0
#pragma only_renderers d3d11
#pragma vertex vert
#pragma geometry geom
#pragma fragment frag


		half4 _WireColor, _Color;
	float _WireThickness;

	struct v2g
	{
		float4  pos : SV_POSITION;
		float2  uv : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct g2f
	{
		float4  pos : SV_POSITION;
		float2  uv : TEXCOORD0;
		float3 dist : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2g vert(appdata_base v)
	{
		UNITY_SETUP_INSTANCE_ID(v);
		v2g OUT;
		OUT.pos = UnityObjectToClipPos(v.vertex);
		OUT.uv = v.texcoord; //the uv's arent used in this shader but are included in case you want to use them
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		return OUT;
	}

	[maxvertexcount(3)]
	void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
	{

		//frag position
		float2 p0 = IN[0].pos.xy / IN[0].pos.w;
		float2 p1 = IN[1].pos.xy / IN[1].pos.w;
		float2 p2 = IN[2].pos.xy / IN[2].pos.w;

		//barycentric position
		float2 v0 = p2 - p1;
		float2 v1 = p2 - p0;
		float2 v2 = p1 - p0;
		//triangles area

		float area = abs(v1.x*v2.y - v1.y * v2.x);

		float3 distScale[3];
		distScale[0] = float3(area / length(v0), 0, 0);
		distScale[1] = float3(0, area / length(v1), 0);
		distScale[2] = float3(0, 0, area / length(v2));

		float wireScale = 800 - _WireThickness;

		g2f OUT;

		[unroll]
		for (uint idx = 0; idx < 3; ++idx)
		{
			OUT.pos = IN[idx].pos;
			OUT.uv = 1.0 / OUT.pos.w;
			OUT.dist = distScale[idx] * OUT.pos.w * wireScale;
			UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(IN[idx], OUT);
			triStream.Append(OUT);
		}

	}

	half4 frag(g2f IN) : COLOR
	{
		//distance of frag from triangles center
		float d = min(IN.dist.x, min(IN.dist.y, IN.dist.z)) * IN.uv;
	//fade based on dist from center
	float I = exp2(-2.0*d*d);

	return lerp(_Color, _WireColor, I);
	}

		ENDCG

	}
	}
}
