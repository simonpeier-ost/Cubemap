using EduGraf.OpenGL;

namespace Cubemap;

public class ReflectionShading : GlShading
{
    private const string VERTEX_SHADER = @"
#version 410

in vec3 Position;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
out vec3 surfacePosition;

void main(void)
{
    gl_Position = vec4(Position, 1.0) * Model * View * Projection;
    surfacePosition = vec3(vec4(Position, 1.0) * Model);
}";
    private const string FRAGMENT_SHADER = @"
#version 330

in vec3 surfacePosition;

uniform vec3 CameraPosition;
uniform float scale;
uniform sampler2D right;
uniform sampler2D left;
uniform sampler2D top;
uniform sampler2D bottom;
uniform sampler2D front;
uniform sampler2D back;
out vec4 FragColor;

void main()
{         
    vec3 incident = normalize(surfacePosition - CameraPosition);
    vec3 reflected = reflect(incident, normalize(surfacePosition));
    float t = (scale - surfacePosition.z) / reflected.z;
    float f = (scale - surfacePosition.x) / reflected.x;
    float g = (scale - surfacePosition.y) / reflected.y;
    FragColor = vec4(0, 0, 0, 0);
    if (t > 0) {
      vec2 uv = 0.5 * (surfacePosition.xy + t * reflected.xy) / scale + 0.5;
      if (0 <= uv.x && uv.x < 1 && 0 <= uv.y && uv.y < 1) FragColor = texture(front, uv);
    } 
    if (t < 0) {
      t = (scale + surfacePosition.z) / reflected.z;
      vec2 uv = 0.5 * (surfacePosition.xy - t * reflected.xy) / scale + 0.5;
      uv = vec2(1- uv.x, uv.y);
      if (0 <= uv.x && uv.x < 1 && 0 <= uv.y && uv.y < 1) FragColor = texture(back, uv);
    }
    if (f > 0) {
      vec2 uv = -0.5 * (surfacePosition.yz + f * reflected.yz) / scale + 0.5;
      uv = vec2(uv.y, 1.0 - uv.x);
      if (0 <= uv.x && uv.x < 1 && 0 <= uv.y && uv.y < 1) FragColor = texture(left, uv);
    }
    if (f < 0) {
      f = (scale + surfacePosition.x) / reflected.x;
      vec2 uv = 0.5 * (surfacePosition.yz - f * reflected.yz) / scale + 0.5;
      uv = vec2(uv.y, uv.x);
      if (0 <= uv.x && uv.x < 1 && 0 <= uv.y && uv.y < 1) FragColor = texture(right, uv);
    }
    if (g > 0) {
      vec2 uv = -0.5 * (surfacePosition.xz + g * reflected.xz) / scale + 0.5;
      uv = vec2(1- uv.x, uv.y);
      if (0 <= uv.x && uv.x < 1 && 0 <= uv.y && uv.y < 1) FragColor = texture(top, uv);
    } 
    if (g < 0) {
      g = (scale + surfacePosition.y) / reflected.y;
      vec2 uv = -0.5 * (surfacePosition.xz - g * reflected.xz) / scale + 0.5;
      uv = vec2(uv.x, uv.y);
      if (0 <= uv.x && uv.x < 1 && 0 <= uv.y && uv.y < 1) FragColor = texture(bottom, uv);
    }
}";

    public ReflectionShading(GlGraphic graphic, GlTextureHandle[] textures, float scale) : base(
            "uniform", 
            graphic, 
            VERTEX_SHADER, 
            FRAGMENT_SHADER, 
            new GlNamedTextureShadingAspect("left", textures[0]), 
            new GlNamedTextureShadingAspect("right", textures[1]), 
            new GlNamedTextureShadingAspect("top", textures[2]), 
            new GlNamedTextureShadingAspect("bottom", textures[3]), 
            new GlNamedTextureShadingAspect("front", textures[4]), 
            new GlNamedTextureShadingAspect("back", textures[5]))
    {
        DoInContext(() => Set("scale", scale));
    }
}