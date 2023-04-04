shader_type canvas_item;

uniform float offset = 1.0f;

void fragment() {
  vec4 green = texture(SCREEN_TEXTURE, SCREEN_UV);
  vec4 red = texture(SCREEN_TEXTURE, vec2(SCREEN_UV.x + offset * SCREEN_PIXEL_SIZE.x, SCREEN_UV.y));
  vec4 blue = texture(SCREEN_TEXTURE, vec2(SCREEN_UV.x - offset * SCREEN_PIXEL_SIZE.x, SCREEN_UV.y));
  COLOR = vec4(red.r, green.g, blue.b, 1.0);
}