shader_type canvas_item;
render_mode blend_mix;

uniform float TwixelAngle : hint_range(0.0, 45.0) = 45.0;
uniform float PixelSize : hint_range(1.0, 32.0) = 16.0;
uniform sampler2D ViewportTexture;
uniform sampler2D PaletteTexture : source_color; // Small texture with your color palette (e.g. 8x1 or 16x1)
uniform ivec2 palette_size = ivec2(10,1);

void fragment() {
    // Get rotated/pixelated texture sample (your existing code)
    float aRad = radians(TwixelAngle);
    mat2 rotation = mat2(
        vec2(cos(aRad), -sin(aRad)),
        vec2(sin(aRad),  cos(aRad))
    );
    vec2 rotated_coord = rotation * FRAGCOORD.xy;
    vec2 pixelated_coord = mod(rotated_coord, PixelSize);
    vec2 final_coord = FRAGCOORD.xy - (transpose(rotation) * pixelated_coord);
    vec2 uv = final_coord * SCREEN_PIXEL_SIZE;
    vec3 original_color = textureLod(ViewportTexture, uv, 0).rgb;

    // Palette quantization
    //ivec2 palette_size = textureSize(PaletteTexture, 0);
    vec3 quantized_color = vec3(0.0);
    float min_dist = 9999.0;

    // Search through palette texture
    for (int y = 0; y < palette_size.y; y++) {
        for (int x = 0; x < palette_size.x; x++) {
			//get the color of the pixel at the center of each pixel rather than the corner.
            vec2 palette_uv = (vec2(float(x), float(y)) +0.5) / vec2(palette_size);
            vec3 palette_color = textureLod(PaletteTexture, palette_uv, 0).rgb;
            float dist = distance(original_color, palette_color);
            if (dist < min_dist) {
                min_dist = dist;
                quantized_color = palette_color;
            }
        }
    }
    COLOR.rgb = quantized_color;
}