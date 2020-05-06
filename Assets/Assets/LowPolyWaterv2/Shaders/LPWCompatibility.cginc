// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

#ifndef LPW_COMPAT_INCLUDED
#define LPW_COMPAT_INCLUDED

#define COMPUTESCREENPOS ComputeScreenPos
#if UNITY_VERSION < 560

	#define UNITY_SHADOW_COORDS(a) SHADOW_COORDS(a)
	#undef UNITY_SHADOW_ATTENUATION
	#define UNITY_SHADOW_ATTENUATION(a, worldPos) SHADOW_ATTENUATION(a)

	#if UNITY_VERSION < 540
		#define UNITY_VERTEX_INPUT_INSTANCE_ID
		#define UNITY_VERTEX_OUTPUT_STEREO
		#define UNITY_SETUP_INSTANCE_ID(v)
		#define UNITY_TRANSFER_INSTANCE_ID(v,o)
		#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
	#endif
#endif

#endif // LPW_COMPAT_INCLUDED