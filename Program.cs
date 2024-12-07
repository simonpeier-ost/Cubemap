using EduGraf;
using EduGraf.Cameras;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Tensors;

namespace Cubemap;

public static class Program
{
    public static void Main()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(3, 3, 1), Point3.Origin);
        using var window = new OpenTkWindow("CubeMapRendering", graphic, 1024, 768, camera.Handle);
        var rendering = new CubeMapRendering(graphic, camera);
        window.Show(rendering, camera);
    }
}