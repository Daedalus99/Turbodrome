shader_type spatial;
render_mode unshaded, depth_draw_always, cull_disabled;

uniform float TwixelAngle : hint_range(0.0, 45.0) = 45.0;
uniform float PixelSize : hint_range(1.0, 32.0) = 16.0;
uniform sampler2D ScreenTexture : hint_screen_texture;
uniform sampler2D PaletteTexture : source_color;
uniform ivec2 palette_size = ivec2(10,1);


void vertex(){
	//put this shader on a simple quad mesh instance
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
	//POSITION = vec4(UV*2.0-1.0, 0.0, 1.0);
}

void fragment() {
    // Reconstruct screen-space coordinates
    vec2 frag_coord = FRAGCOORD.xy;
    vec2 screen_pixel_size = 1.0 / VIEWPORT_SIZE;
    
    // Center rotation at fragment center
    float aRad = radians(TwixelAngle);
    mat2 rotation = mat2(
        vec2(cos(aRad), -sin(aRad)),
        vec2(sin(aRad),  cos(aRad))
    );
    
    // Apply rotation and pixelation
    vec2 rotated_coord = rotation * frag_coord;
    vec2 pixelated_coord = floor(rotated_coord / PixelSize) * PixelSize;
    vec2 final_coord = transpose(rotation) * pixelated_coord;
    
    // Sample screen texture with NEAREST filtering for crisp pixels
    vec3 original_color = textureLod(ScreenTexture, final_coord * screen_pixel_size, 0.0).rgb;

    // Palette quantization
    //ivec2 palette_size = textureSize(PaletteTexture, 0);
    vec3 quantized_color = vec3(0.0, 1.0, 0.0); //bright debug-green color
    float min_dist = 9999.0; // rgb vectors have a relatively small max-distance since they can go from -1 to 1.

        // Search through palette texture
    for (int y = 0; y < palette_size.y; y++) {
        for (int x = 0; x < palette_size.x; x++) {
			//get the color of the pixel at the center of each pixel rather than the corner.
            vec2 palette_uv = (vec2(float(x), float(y)) +0.5) / vec2(palette_size);
            vec3 palette_color = textureLod(PaletteTexture, palette_uv, 0.0).rgb;
            float dist = distance(original_color, palette_color);
	      	if (dist < min_dist) {
                min_dist = dist;
            	quantized_color = palette_color;
            }
        }
    }
    ALBEDO = quantized_color;
}