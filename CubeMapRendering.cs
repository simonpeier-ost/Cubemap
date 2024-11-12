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
    const float Scale = 5f;

    public CubeMapRendering(Graphic graphic) : base(graphic, new Color3(0, 0, 0))
    {
        //var cubeAsFaces = GetCubeAsFaces(graphic);
        var cubePosition = new Point3(0, 0, 0);
        var sphere = GetSphere(graphic);
        var spherePosition = new Point3(0, 0, 0);
        using var imageFront = Image.Load<Rgba32>("assets/FieldCubeMap/FieldFront.png");
        imageFront.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageBack = Image.Load<Rgba32>("assets/FieldCubeMap/FieldBack.png");
        imageBack.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageLeft = Image.Load<Rgba32>("assets/FieldCubeMap/FieldLeft.png");
        imageLeft.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageRight = Image.Load<Rgba32>("assets/FieldCubeMap/FieldRight.png");
        imageRight.Mutate(context => context.Flip(FlipMode.Vertical));
        using var imageTop = Image.Load<Rgba32>("assets/FieldCubeMap/FieldTop.png");
        imageTop.Mutate(context => context.Flip(FlipMode.Horizontal));
        using var imageBottom = Image.Load<Rgba32>("assets/FieldCubeMap/FieldBottom.png");
        imageBottom.Mutate(context => context.Flip(FlipMode.Vertical));

        Face[] faces = getFacesOfCube(Cube.Positions, Cube.Triangles);
        var faceFront = GetFace(graphic, imageLeft, faces[0], "left");
        var faceBack = GetFace(graphic, imageFront, faces[1], "front");
        var faceLeft = GetFace(graphic, imageRight, faces[2], "right");
        var faceRight = GetFace(graphic, imageBack, faces[3], "back");
        var faceTop = GetFace(graphic, imageTop, faces[4], "top");
        var faceBottom =  GetFace(graphic, imageBottom, faces[5], "bottom");
            
            Console.WriteLine("CUBE TEXTUREUV");
            Console.WriteLine(string.Join(", ", Cube.TextureUv));

        Scene.AddRange(
        [
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
                .Scale(Scale/4)
                .Translate(spherePosition.Vector)
        ]);
    }

    private VisualPart GetFace(Graphic graphic, Image<Rgba32> image, Face face, String name){
            // Create Texture
            var texture = Graphic.CreateTexture(image);
            var material = new ColorTextureMaterial(1f, 1f, texture);
            // Create face shading
            var shading = graphic.CreateShading("emissive", material, [new AmbientLight(new Color3(1f, 1f, 1f))]);
        
            // Set up face geometry
            var positions = face.Positions;
            var triangles = face.Triangles;
            var textureUvs = face.TextureUv;
            if(name == "top"){
                textureUvs = new float[]
                {
                    1.0f, 0.0f,
                    1.0f, 1.0f,
                    0.0f, 1.0f,
                    0.0f, 0.0f
                };
            } else if(name == "bottom"){
                textureUvs = new float[]
                {
                    0.0f, 1.0f,
                    0.0f, 0.0f,
                    1.0f, 0.0f,
                    1.0f, 1.0f
                };
            }
            var geometry = Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
            
            var surface = graphic.CreateSurface(shading, geometry);
            return graphic.CreateVisual(name, surface);

            
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

    private Face[] getFacesOfCube(float[] positions,ushort[] triangles)
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
            TextureUv = new float[]
            {
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f
            };
        }
    }
}