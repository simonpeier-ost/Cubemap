using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.OpenGL;
using EduGraf.Shapes;
using EduGraf.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Cubemap;

public class CubeMapRendering : Rendering
{
    const float Scale = 5f;
    private Camera Camera;
    private string imageName = "Field";

    public CubeMapRendering(Graphic graphic, Camera camera) : base(graphic, new Color3(0, 0, 0))
    {
        Camera = camera;
        var cubePosition = new Point3(0, 0, 0);
        using var imageFront = Image.Load<Rgba32>("assets/" + imageName + "CubeMap/" + imageName + "Front.png");
        imageFront.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageBack = Image.Load<Rgba32>("assets/" + imageName + "CubeMap/" + imageName + "Back.png");
        imageBack.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageLeft = Image.Load<Rgba32>("assets/" + imageName + "CubeMap/" + imageName + "Left.png");
        imageLeft.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageRight = Image.Load<Rgba32>("assets/" + imageName + "CubeMap/" + imageName + "Right.png");
        imageRight.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageTop = Image.Load<Rgba32>("assets/" + imageName + "CubeMap/" + imageName + "Top.png");
        imageTop.Mutate(context => context.Flip(FlipMode.Horizontal));
        using var imageBottom = Image.Load<Rgba32>("assets/" + imageName + "CubeMap/" + imageName + "Bottom.png");
        imageBottom.Mutate(context => context.Flip(FlipMode.Vertical));

        /*The images and textures are manipulated to allow any CubeMap image like Field/CanyonCubeMap.jpeg to be used.*/
        Face[] faces = GetFacesOfCube(Cube.Positions, Cube.Triangles);
        var faceFront = GetFace(graphic, imageLeft, faces[0], "left", out var frontTexture);
        var faceBack = GetFace(graphic, imageFront, faces[1], "front", out var backTexture);
        var faceLeft = GetFace(graphic, imageRight, faces[2], "right", out var leftTexture);
        var faceRight = GetFace(graphic, imageBack, faces[3], "back", out var rightTexture);
        var faceTop = GetFace(graphic, imageTop, faces[4], "top", out var topTexture);
        var faceBottom =  GetFace(graphic, imageBottom, faces[5], "bottom", out var bottomTexture);

        GlTextureHandle[] textures = [backTexture, rightTexture, topTexture, bottomTexture, frontTexture, leftTexture];
        var sphere = GetSphere(graphic, textures, Scale);
            
        Console.WriteLine("CUBE TEXTUREUV");
        Console.WriteLine(string.Join(", ", Cube.TextureUv));

        foreach (var face in new[] {
                     faceFront
                         .Scale(Scale)
                         .Translate(cubePosition.Vector),
                     faceBack
                         .Scale(Scale)
                         .Translate(cubePosition.Vector),
                     faceLeft
                         .Scale(Scale)
                         .Translate(cubePosition.Vector),
                     faceRight
                         .Scale(Scale)
                         .Translate(cubePosition.Vector),
                     faceTop
                         .Scale(Scale)
                         .Translate(cubePosition.Vector),
                     faceBottom
                         .Scale(Scale)
                         .Translate(cubePosition.Vector),
                     sphere
                         .Scale(Scale / 4)
                 })
        {
            Scene.Add(face);
        }
    }

    private VisualPart GetFace(Graphic graphic, Image<Rgba32> image, Face face, String name, out GlTextureHandle texture){
            // Create Texture
            texture = (GlTextureHandle) Graphic.CreateTexture(image);
            var material = new ColorTextureMaterial(1f, 1f, texture);
            // Create face shading
            var shading = graphic.CreateShading("emissive", material, [new AmbientLight(new Color3(1f, 1f, 1f))]);
        
            // Set up face geometry
            var positions = face.Positions;
            var triangles = face.Triangles;
            var textureUvs = face.TextureUv;
            if(name == "top")
            {
                textureUvs =
                [
                    1.0f, 0.0f,
                    1.0f, 1.0f,
                    0.0f, 1.0f,
                    0.0f, 0.0f
                ];
            } else if(name == "bottom"){
                textureUvs =
                [
                    0.0f, 1.0f,
                    0.0f, 0.0f,
                    1.0f, 0.0f,
                    1.0f, 1.0f
                ];
            }
            var geometry = Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
            
            var surface = graphic.CreateSurface(shading, geometry);
            var visual = graphic.CreateVisual(name, surface);
            return visual;
    }
    
    private VisualPart GetSphere(Graphic graphic, GlTextureHandle[] textures, float z)
    {
        // Create sphere shading
        var shading = new ReflectionShading((GlGraphic)graphic, textures, z);
        
        // Set up sphere geometry
        var positions = Sphere.GetPositions(20, 20);
        var triangles = Sphere.GetTriangles(20, 20);
        var textureUvs = Sphere.GetTextureUvs(20, 20);
        var geometry = Geometry.CreateWithUv(positions, textureUvs, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        return graphic.CreateVisual("sphere", surface);
    } 

    private Face[] GetFacesOfCube(float[] positions,ushort[] triangles)
    {
        Face[] faces = new Face[6];
        for (int faceIndex = 0; faceIndex < 6; faceIndex++) {
            
            float[] facePositions = new float[12];
            int currentPositionsModifier = faceIndex * 12;
            for (int i = 0; i < 12; i++)
            {
                facePositions[i] = positions[i + currentPositionsModifier];
            }

            ushort[] faceTriangles = new ushort[6];
            for (int i = 0; i < 6; i++)
            {
                faceTriangles[i] = triangles[i];
            }
            if(faceIndex == 4){
                faces[faceIndex] = new Face(facePositions, faceTriangles);
            } else {
                faces[faceIndex] = new Face(facePositions, faceTriangles);
            }
        }
        return faces;
    }

    public class Face
    {
        public float[] Positions { get; }
        public ushort[] Triangles { get; }
        public float[] TextureUv { get; }

        public Face(float[] positions, ushort[] triangles)
        {
            Positions = positions;
            Triangles = triangles;
            TextureUv =
            [
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f
            ];
        }
    }
}