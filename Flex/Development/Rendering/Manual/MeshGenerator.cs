using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Rendering.Manual
{
    public static class MeshGenerator
    {
        private static readonly String GENERATION_DEFAULT_TEXTURE = "Part/Placeholder";

        public static MeshPtr GenerateCube(float x, float y, float z, String name, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace)
        {
            x /= 2;
            y /= 2;
            z /= 2;

            ManualObject cube = new ManualObject(name);

            //START GENERATION 

            cube.Begin(bottomFace.Name);

            cube.Position(x, -y, z);
            cube.Normal(0.408248f, -0.816497f, 0.408248f);
            cube.TextureCoord(1, 0);

            //1
            cube.Position(-x, -y, -z);
            cube.Normal(-0.408248f, -0.816497f, -0.408248f);
            cube.TextureCoord(0, 1);

            //2
            cube.Position(x, -y, -z);
            cube.Normal(0.666667f, -0.333333f, -0.666667f);
            cube.TextureCoord(1, 1);

            //3
            cube.Position(-x, -y, z);
            cube.Normal(-0.666667f, -0.333333f, 0.666667f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(3, 1, 0);

            cube.End();
            cube.Begin(backFace.Name);

            //4
            cube.Position(x, y, z);
            cube.Normal(0.666667f, 0.333333f, 0.666667f);
            cube.TextureCoord(1, 0);

            //5
            cube.Position(-x, -y, z);
            cube.Normal(-0.666667f, -0.333333f, 0.666667f);
            cube.TextureCoord(0, 1);

            //6
            cube.Position(x, -y, z);
            cube.Normal(0.408248f, -0.816497f, 0.408248f);
            cube.TextureCoord(1, 1);

            //7
            cube.Position(-x, y, z);
            cube.Normal(-0.408248f, 0.816497f, 0.408248f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(0, 3, 1);

            cube.End();
            cube.Begin(leftFace.Name);

            //7
            cube.Position(-x, y, z);
            cube.Normal(-0.408248f, 0.816497f, 0.408248f);
            cube.TextureCoord(0, 0);

            //8
            cube.Position(-x, y, -z);
            cube.Normal(-0.666667f, 0.333333f, -0.666667f);
            cube.TextureCoord(0, 1);

            //9
            cube.Position(-x, -y, -z);
            cube.Normal(-0.408248f, -0.816497f, -0.408248f);
            cube.TextureCoord(1, 1);

            //10
            cube.Position(-x, -y, z);
            cube.Normal(-0.666667f, -0.333333f, 0.666667f);
            cube.TextureCoord(1, 0);

            cube.Triangle(1, 2, 3);
            cube.Triangle(3, 0, 1);

            cube.End();
            cube.Begin(rightFace.Name);

            //4
            cube.Position(x, y, z);
            cube.Normal(0.666667f, 0.333333f, 0.666667f);
            cube.TextureCoord(1, 0);

            //11
            cube.Position(x, -y, -z);
            cube.Normal(0.666667f, -0.333333f, -0.666667f);
            cube.TextureCoord(0, 1);

            //12
            cube.Position(x, y, -z);
            cube.Normal(0.408248f, 0.816497f, -0.408248f);
            cube.TextureCoord(1, 1);

            //13
            cube.Position(x, -y, z);
            cube.Normal(0.408248f, -0.816497f, 0.408248f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(0, 3, 1);

            cube.End();
            cube.Begin(frontFace.Name);

            //8
            cube.Position(-x, y, -z);
            cube.Normal(-0.666667f, 0.333333f, -0.666667f);
            cube.TextureCoord(0, 1);

            //12
            cube.Position(x, y, -z);
            cube.Normal(0.408248f, 0.816497f, -0.408248f);
            cube.TextureCoord(1, 1);

            //14
            cube.Position(x, -y, -z);
            cube.Normal(0.666667f, -0.333333f, -0.666667f);
            cube.TextureCoord(1, 0);

            //15
            cube.Position(-x, -y, -z);
            cube.Normal(-0.408248f, -0.816497f, -0.408248f);
            cube.TextureCoord(0, 0);

            cube.Triangle(2, 0, 1);
            cube.Triangle(2, 3, 0);

            cube.End();
            cube.Begin(topFace.Name);

            //16
            cube.Position(-x, y, z);
            cube.Normal(-0.408248f, 0.816497f, 0.408248f);
            cube.TextureCoord(1, 0);

            //17
            cube.Position(x, y, -z);
            cube.Normal(0.408248f, 0.816497f, -0.408248f);
            cube.TextureCoord(0, 1);

            //18
            cube.Position(-x, y, -z);
            cube.Normal(-0.666667f, 0.333333f, -0.666667f);
            cube.TextureCoord(1, 1);

            //19
            cube.Position(x, y, z);
            cube.Normal(0.666667f, 0.333333f, 0.666667f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(0, 3, 1);

            cube.End();

            return cube.ConvertToMesh(name + "/DynamicMesh/" + Guid.NewGuid().ToString());
        }
    }
}
