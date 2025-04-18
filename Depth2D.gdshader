shader_type canvas_item;
render_mode blend_mix;

uniform sampler2D ViewportTexture;

void fragment() {
    COLOR.rgb = textureLod(ViewportTexture, UV, 0).rgb;
}