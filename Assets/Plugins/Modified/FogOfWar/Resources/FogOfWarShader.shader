// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FogOfWar"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FogTex ("Fog", 2D) = "white" {}
		_FogColorTex ("Fog Color", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"
	
	uniform sampler2D _MainTex;
	uniform sampler2D_float _FogTex;
	uniform sampler2D_float _CameraDepthTexture;
	uniform float4 _MainTex_TexelSize;
	uniform sampler2D _FogColorTex;
	uniform float2 _FogColorTexScale;
	
	// for fast world space reconstruction
	uniform float4x4 _FrustumCornersWS;
	uniform float3 _CameraWS; // camera's world space position
	uniform float4 _CameraDir; // xyz = camera direction, w = near plane distance

	uniform float _FogTextureSize;
	uniform float _MapSize;
	uniform float4 _MapOffset;
	uniform float4 _FogColor;
	uniform float _OutsideFogStrength;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv_depth : TEXCOORD1;
		float4 interpolatedRay : TEXCOORD2;
	};
	
	v2f vert (appdata_img v)
	{
		v2f o;
		half index = v.vertex.z;
		v.vertex.z = 0.1;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif				
		
		o.interpolatedRay = _FrustumCornersWS[(int)index];
		o.interpolatedRay.w = index;
		
		return o;
	}

	ENDCG

	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile CAMERA_ORTHOGRAPHIC CAMERA_PERSPECTIVE
			#pragma multi_compile PLANE_XY PLANE_YZ PLANE_XZ
			#pragma multi_compile _ TEXTUREFOG
			#pragma multi_compile _ FOGFARPLANE
			#pragma multi_compile _ FOGOUTSIDEAREA
			#pragma multi_compile _ CLEARFOG

			half4 frag (v2f i) : SV_Target
			{
				// Reconstruct world space position and direction
				float rawdpth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);
				#ifdef CAMERA_ORTHOGRAPHIC
					float3 nearPlaneDist = _CameraDir.w; // from camera
					float3 camDir = _CameraDir.xyz;
					float3 nearPlaneOffset = camDir * nearPlaneDist; // relative to camera pos
					float3 camPos = _CameraWS + nearPlaneOffset; // pretend the camera pos is at the near plane
					float3 rayFar = i.interpolatedRay - nearPlaneOffset; // relative to camera near plane
				
					float3 rayVec = camDir * dot(rayFar, camDir); // relative to rayOrigin
					float3 rayOrigin = rayFar - rayVec; // relative to camera
					// apparently unity inverted the Z buffer when not using Linear01Depth: https://docs.unity3d.com/Manual/UpgradeGuide55.html
					// this could be avoided if the orthographic camera used Linear01Depth!!
					// also, the depth buffer seems to be reversed on mobile devices (ie GL ES)
					#if UNITY_VERSION >= 550 && !SHADER_API_GLES && !SHADER_API_GLES3 && !SHADER_API_D3D9
						rawdpth = 1 - rawdpth;
					#endif
					float3 rayCast = (rayFar - rayOrigin) * rawdpth; // just use the raw depth texture for ortho
					float3 wsPos = camPos + rayOrigin + rayCast;
				
				#else
					float3 wsDir = i.interpolatedRay * Linear01Depth(rawdpth); // for PERSPECTIVE
					float3 wsPos = _CameraWS + wsDir;
				#endif
				
				#ifdef PLANE_XY
					float2 modepos = wsPos.xy;
				#elif PLANE_YZ
					float2 modepos = wsPos.yz;
				#elif PLANE_XZ
					float2 modepos = wsPos.xz;
				#endif

				float2 mapPos = (modepos - _MapOffset.xy) / _MapSize + float2(0.5f, 0.5f);
				
				// if it is beyond the map
				float isoutsidemap = min(1, step(mapPos.x, 0) + step(1, mapPos.x) + step(mapPos.y, 0) + step(1, mapPos.y));

				// if outside map, use the outside fog color
				float fog = lerp(tex2D(_FogTex, mapPos).a, _OutsideFogStrength, isoutsidemap);

				#ifdef TEXTUREFOG
					// raycast plane
					float3 rayorigin = _CameraWS;
					float3 raydir = normalize(i.interpolatedRay);
					float3 planeorigin = float3(0, _FogColorTexScale.y, 0);
					float3 planenormal = float3(0, 1, 0);
					float t = dot(planeorigin - rayorigin, planenormal) / dot(planenormal, raydir);
					float4 fogcolor = tex2D(_FogColorTex, (rayorigin + raydir * t).xz * _FogColorTexScale.x);
				#else
					float4 fogcolor = _FogColor;
				#endif
				
				#ifndef FOGFARPLANE
					fog *= step(rawdpth, 0.999); // don't show fog on the far plane
				#endif

				half4 sceneColor = tex2D(_MainTex, i.uv);

				#ifdef CLEARFOG
					return lerp(sceneColor, fogcolor, fog);
				#else
					return lerp(sceneColor, fogcolor, fog * fogcolor.a);
				#endif
			}

			ENDCG
		}
	}

	Fallback off
}
