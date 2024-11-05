using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;

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

    private static VisualPart GetCube(Graphic graphic, Camera camera)
    {
        var color = new Color4(0.8f, 1, 0.8f, 1);
        var shading = graphic.CreateShading([], [new EmissiveUniformMaterial(color)], camera);
        var positions = Cube.Positions;
        var triangles = Cube.Triangles;
        var geometry = Geometry.Create(positions, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        return graphic.CreateVisual("cube", surface);
    }
}