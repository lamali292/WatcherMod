shader_type canvas_item;

uniform vec4 base_color  = vec4(1.0, 0.0, 0.0, 1.0); // Can change in inspector
uniform float time_speed : hint_range(0.0, 5.0) = 0.2;
uniform float cloud_scale : hint_range(0.1, 10.0) = 3.0;
uniform float alpha_strength : hint_range(0.0, 1.0) = 0.8;

void fragment() {
    // Scroll the noise texture
    vec2 uv = UV * cloud_scale;
    uv += vec2(TIME * time_speed, TIME * time_speed * 0.5);

    // Sample the noise texture
    float n = texture(TEXTURE, uv).r;

    // Radial mask: center stronger alpha
    vec2 center = vec2(0.5, 0.5);
    float dist = distance(UV, center);
    float radial = smoothstep(0.5, 0.0, dist);

    // Final alpha combines radial and noise
    float alpha = n * radial * alpha_strength;

    // Output color using uniform
    COLOR = vec4(base_color.rgb, alpha);
}