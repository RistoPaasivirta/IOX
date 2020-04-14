half3 Dissolve(half3 color, half noise, half level, half width, half3 color1, half3 color2)
{
	if (noise < level)
		discard;
	
	if (noise < level + width)
		color = lerp(color2, color1, max((noise - level), 0) / width);

	return color;
}