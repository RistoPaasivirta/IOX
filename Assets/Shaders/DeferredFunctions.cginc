struct DeferredData
{
	half3   color;
	half    shine;

	half3   transcolor;
	half    glossiness;

	float3  normal;
	half rim;
};

DeferredData DeferredDataFromGbuffer(half4 inGBuffer0, half4 inGBuffer1, half4 inGBuffer2)
{
	DeferredData data;

	data.color = inGBuffer0.rgb;
	data.shine = inGBuffer0.a;

	data.transcolor = inGBuffer1.rgb;
	data.glossiness = inGBuffer1.a;

	data.normal = normalize((float3)inGBuffer2.rgb * 2 - 1);
	data.rim = inGBuffer2.a;

	return data;
}

half4 DefferedLighting(DeferredData data, half3 viewDir, UnityLight light)
{
	half3 h = normalize(light.dir + viewDir);
	float nh = max(0, dot(data.normal, h));
	float spec = pow(nh, 256 * data.glossiness + 1) * (64 * data.shine);

	half l = dot(data.normal, light.dir);
	half diff = max(0, l);
	half trans = max(0, 1 - l);
	half glance = pow(1 - abs(l), 8) * .1;

	half3 direct = light.color * data.color * (diff + spec);
	half3 back = light.color * data.transcolor * trans;
	half3 side = light.color * data.rim * glance;

	return half4(direct + back + side, 1);
}