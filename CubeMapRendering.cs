using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Cubemap;

public class CubeMapRendering : Rendering
{
    const int Scale = 5;

    public CubeMapRendering(Graphic graphic, Camera camera) : base(graphic, new Color3(0, 0, 0))
    {
        var cube = GetCube(graphic, camera);
        var cubePosition = new Point3(0, Scale, 0);
        Scene.AddRange(
        [
            cube
                .Scale(Scale)
                .Translate(cubePosition.Vector),
        ]);
    }

    private VisualPart GetCube(Graphic graphic, Camera camera)
    {
        // Create Texture from image
        using var image = Image.Load<Rgba32>("assets/CanyonCubeMap.jpeg");
        var texture = Graphic.CreateTexture(image);
        var material = new ColorTextureMaterial(1f, 1f, texture);
        
        
        // Create cube object
        Console.WriteLine("BING BONG");
        var shading = graphic.CreateShading("crazymofo", material, [new AmbientLight(new Color3(1f, 1f, 1f))]);
        Console.WriteLine("MAMA");
        var positions = Cube.Positions;
        var triangles = Cube.Triangles;
        
        var textureUvs = Sphere.GetTextureUvs(600, 800);
        var geometry = Geometry.CreateWithUv(positions, textureUvs, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        Console.WriteLine("BABABA");
        return graphic.CreateVisual("cube", surface);
    }
}