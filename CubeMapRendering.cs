using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cubemap;

public class CubeMapRendering : Rendering
{
    const float Scale = 5f;

    public CubeMapRendering(Graphic graphic) : base(graphic, new Color3(0, 0, 0))
    {
        var cube = GetCube(graphic);
        var cubePosition = new Point3(0, 0, 0);
        var sphere = GetSphere(graphic);
        var spherePosition = new Point3(0, 0, 0);
        
        Scene.AddRange(
        [
            cube
                .Scale(Scale)
                .Translate(cubePosition.Vector),
            sphere
                .Scale(Scale/4)
                .Translate(spherePosition.Vector)
        ]);
    }

    private VisualPart GetCube(Graphic graphic)
    {
        // Create Texture from image
        using var image = Image.Load<Rgba32>("assets/CanyonCubeMap.jpeg");
        var texture = Graphic.CreateTexture(image);
        var material = new ColorTextureMaterial(1f, 1f, texture);
        
        // Create cube shading
        var shading = graphic.CreateShading("emissive", material, [new AmbientLight(new Color3(1f, 1f, 1f))]);
        
        // Set up cube geometry
        var positions = Cube.Positions;
        var triangles = Cube.Triangles;
        var textureUvs = Cube.TextureUv;
        var geometry = Geometry.CreateWithUv(positions, textureUvs, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        return graphic.CreateVisual("cube", surface);
    }
    
    private VisualPart GetSphere(Graphic graphic)
    {
        // Create Texture from image
        using var image = Image.Load<Rgba32>("assets/CanyonCubeMap.jpeg");
        var texture = Graphic.CreateTexture(image);
        var material = new ColorTextureMaterial(1f, 1f, texture);
        
        // Create sphere shading
        var shading = graphic.CreateShading("emissive", material, [new AmbientLight(new Color3(1f, 1f, 1f))]);
        
        // Set up sphere geometry
        var positions = Sphere.GetPositions(10, 10);
        var triangles = Sphere.GetTriangles(10, 10);
        var textureUvs = Sphere.GetTextureUvs(10, 10);
        var geometry = Geometry.CreateWithUv(positions, textureUvs, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        return graphic.CreateVisual("sphere", surface);
    } 
}