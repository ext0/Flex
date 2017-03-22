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

        ///////////////////////////////////////////////////////
        ////                GENERATION CODE                ////
        ///////////////////////////////////////////////////////

        public static ManualObject GenerateCube(float x, float y, float z, String name, Material material, Vector3 positionOffset)
        {
            return GenerateCubeHelper(x, y, z, name, material, material, material, material, material, material, positionOffset);
        }

        public static ManualObject GenerateCube(float x, float y, float z, String name, Material material)
        {
            return GenerateCubeHelper(x, y, z, name, material, material, material, material, material, material, Vector3.ZERO);
        }

        public static ManualObject GenerateCube(float x, float y, float z, String name, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace, Vector3 positionOffset)
        {
            return GenerateCubeHelper(x, y, z, name, frontFace, backFace, leftFace, rightFace, topFace, bottomFace, positionOffset);
        }

        public static ManualObject GenerateCube(float x, float y, float z, String name, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace)
        {
            return GenerateCubeHelper(x, y, z, name, frontFace, backFace, leftFace, rightFace, topFace, bottomFace, Vector3.ZERO);
        }

        private static ManualObject GenerateCubeHelper(float x, float y, float z, String name, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace, Vector3 positionOffset)
        {
            float xOffset = positionOffset.x;
            float yOffset = positionOffset.y;
            float zOffset = positionOffset.z;

            x /= 2;
            y /= 2;
            z /= 2;

            ManualObject cube = Engine.Renderer.Scene.CreateManualObject(name);

            cube.Dynamic = true;

            //START GENERATION 

            cube.Begin(bottomFace.Name);

            cube.Position(x + xOffset, -y + yOffset, z + zOffset);
            cube.Normal(0.408248f, -0.816497f, 0.408248f);
            cube.TextureCoord(1, 0);

            //1
            cube.Position(-x + xOffset, -y + yOffset, -z + zOffset);
            cube.Normal(-0.408248f, -0.816497f, -0.408248f);
            cube.TextureCoord(0, 1);

            //2
            cube.Position(x + xOffset, -y + yOffset, -z + zOffset);
            cube.Normal(0.666667f, -0.333333f, -0.666667f);
            cube.TextureCoord(1, 1);

            //3
            cube.Position(-x + xOffset, -y + yOffset, z + zOffset);
            cube.Normal(-0.666667f, -0.333333f, 0.666667f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(3, 1, 0);

            cube.End();
            cube.Begin(backFace.Name);

            //4
            cube.Position(x + xOffset, y + yOffset, z + zOffset);
            cube.Normal(0.666667f, 0.333333f, 0.666667f);
            cube.TextureCoord(1, 0);

            //5
            cube.Position(-x + xOffset, -y + yOffset, z + zOffset);
            cube.Normal(-0.666667f, -0.333333f, 0.666667f);
            cube.TextureCoord(0, 1);

            //6
            cube.Position(x + xOffset, -y + yOffset, z + zOffset);
            cube.Normal(0.408248f, -0.816497f, 0.408248f);
            cube.TextureCoord(1, 1);

            //7
            cube.Position(-x + xOffset, y + yOffset, z + zOffset);
            cube.Normal(-0.408248f, 0.816497f, 0.408248f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(0, 3, 1);

            cube.End();
            cube.Begin(leftFace.Name);

            //7
            cube.Position(-x + xOffset, y + yOffset, z + zOffset);
            cube.Normal(-0.408248f, 0.816497f, 0.408248f);
            cube.TextureCoord(0, 0);

            //8
            cube.Position(-x + xOffset, y + yOffset, -z + zOffset);
            cube.Normal(-0.666667f, 0.333333f, -0.666667f);
            cube.TextureCoord(0, 1);

            //9
            cube.Position(-x + xOffset, -y + yOffset, -z + zOffset);
            cube.Normal(-0.408248f, -0.816497f, -0.408248f);
            cube.TextureCoord(1, 1);

            //10
            cube.Position(-x + xOffset, -y + yOffset, z + zOffset);
            cube.Normal(-0.666667f, -0.333333f, 0.666667f);
            cube.TextureCoord(1, 0);

            cube.Triangle(1, 2, 3);
            cube.Triangle(3, 0, 1);

            cube.End();
            cube.Begin(rightFace.Name);

            //4
            cube.Position(x + xOffset, y + yOffset, z + zOffset);
            cube.Normal(0.666667f, 0.333333f, 0.666667f);
            cube.TextureCoord(1, 0);

            //11
            cube.Position(x + xOffset, -y + yOffset, -z + zOffset);
            cube.Normal(0.666667f, -0.333333f, -0.666667f);
            cube.TextureCoord(0, 1);

            //12
            cube.Position(x + xOffset, y + yOffset, -z + zOffset);
            cube.Normal(0.408248f, 0.816497f, -0.408248f);
            cube.TextureCoord(1, 1);

            //13
            cube.Position(x + xOffset, -y + yOffset, z + zOffset);
            cube.Normal(0.408248f, -0.816497f, 0.408248f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(0, 3, 1);

            cube.End();
            cube.Begin(frontFace.Name);

            //8
            cube.Position(-x + xOffset, y + yOffset, -z + zOffset);
            cube.Normal(-0.666667f, 0.333333f, -0.666667f);
            cube.TextureCoord(0, 1);

            //12
            cube.Position(x + xOffset, y + yOffset, -z + zOffset);
            cube.Normal(0.408248f, 0.816497f, -0.408248f);
            cube.TextureCoord(1, 1);

            //14
            cube.Position(x + xOffset, -y + yOffset, -z + zOffset);
            cube.Normal(0.666667f, -0.333333f, -0.666667f);
            cube.TextureCoord(1, 0);

            //15
            cube.Position(-x + xOffset, -y + yOffset, -z + zOffset);
            cube.Normal(-0.408248f, -0.816497f, -0.408248f);
            cube.TextureCoord(0, 0);

            cube.Triangle(2, 0, 1);
            cube.Triangle(2, 3, 0);

            cube.End();
            cube.Begin(topFace.Name);

            //16
            cube.Position(-x + xOffset, y + yOffset, z + zOffset);
            cube.Normal(-0.408248f, 0.816497f, 0.408248f);
            cube.TextureCoord(1, 0);

            //17
            cube.Position(x + xOffset, y + yOffset, -z + zOffset);
            cube.Normal(0.408248f, 0.816497f, -0.408248f);
            cube.TextureCoord(0, 1);

            //18
            cube.Position(-x + xOffset, y + yOffset, -z + zOffset);
            cube.Normal(-0.666667f, 0.333333f, -0.666667f);
            cube.TextureCoord(1, 1);

            //19
            cube.Position(x + xOffset, y + yOffset, z + zOffset);
            cube.Normal(0.666667f, 0.333333f, 0.666667f);
            cube.TextureCoord(0, 0);

            cube.Triangle(0, 1, 2);
            cube.Triangle(0, 3, 1);

            cube.End();

            return cube;
        }

        ///////////////////////////////////////////////////////
        ////                UPDATING CODE                  ////
        ///////////////////////////////////////////////////////

        public static ManualObject UpdateCube(ManualObject updating, float x, float y, float z, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace, Face updatedFaces, Vector3 positionOffset)
        {
            return UpdateCubeHelper(updating, x, y, z, frontFace, backFace, leftFace, rightFace, topFace, bottomFace, updatedFaces, positionOffset);
        }

        public static ManualObject UpdateCube(ManualObject updating, float x, float y, float z, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace, Face updatedFaces)
        {
            return UpdateCubeHelper(updating, x, y, z, frontFace, backFace, leftFace, rightFace, topFace, bottomFace, updatedFaces, Vector3.ZERO);
        }

        private static ManualObject UpdateCubeHelper(ManualObject updating, float x, float y, float z, Material frontFace, Material backFace, Material leftFace, Material rightFace, Material topFace, Material bottomFace, Face updatedFaces, Vector3 positionOffset)
        {
            float xOffset = positionOffset.x;
            float yOffset = positionOffset.y;
            float zOffset = positionOffset.z;

            x /= 2;
            y /= 2;
            z /= 2;

            //START GENERATION 

            updating.BeginUpdate(0);

            updating.Position(x + xOffset, -y + yOffset, z + zOffset);
            updating.Normal(0.408248f, -0.816497f, 0.408248f);
            updating.TextureCoord(1, 0);

            //1
            updating.Position(-x + xOffset, -y + yOffset, -z + zOffset);
            updating.Normal(-0.408248f, -0.816497f, -0.408248f);
            updating.TextureCoord(0, 1);

            //2
            updating.Position(x + xOffset, -y + yOffset, -z + zOffset);
            updating.Normal(0.666667f, -0.333333f, -0.666667f);
            updating.TextureCoord(1, 1);

            //3
            updating.Position(-x + xOffset, -y + yOffset, z + zOffset);
            updating.Normal(-0.666667f, -0.333333f, 0.666667f);
            updating.TextureCoord(0, 0);

            updating.Triangle(0, 1, 2);
            updating.Triangle(3, 1, 0);

            updating.End();
            updating.BeginUpdate(1);

            //4
            updating.Position(x + xOffset, y + yOffset, z + zOffset);
            updating.Normal(0.666667f, 0.333333f, 0.666667f);
            updating.TextureCoord(1, 0);

            //5
            updating.Position(-x + xOffset, -y + yOffset, z + zOffset);
            updating.Normal(-0.666667f, -0.333333f, 0.666667f);
            updating.TextureCoord(0, 1);

            //6
            updating.Position(x + xOffset, -y + yOffset, z + zOffset);
            updating.Normal(0.408248f, -0.816497f, 0.408248f);
            updating.TextureCoord(1, 1);

            //7
            updating.Position(-x + xOffset, y + yOffset, z + zOffset);
            updating.Normal(-0.408248f, 0.816497f, 0.408248f);
            updating.TextureCoord(0, 0);

            updating.Triangle(0, 1, 2);
            updating.Triangle(0, 3, 1);

            updating.End();
            updating.BeginUpdate(2);

            //7
            updating.Position(-x + xOffset, y + yOffset, z + zOffset);
            updating.Normal(-0.408248f, 0.816497f, 0.408248f);
            updating.TextureCoord(0, 0);

            //8
            updating.Position(-x + xOffset, y + yOffset, -z + zOffset);
            updating.Normal(-0.666667f, 0.333333f, -0.666667f);
            updating.TextureCoord(0, 1);

            //9
            updating.Position(-x + xOffset, -y + yOffset, -z + zOffset);
            updating.Normal(-0.408248f, -0.816497f, -0.408248f);
            updating.TextureCoord(1, 1);

            //10
            updating.Position(-x + xOffset, -y + yOffset, z + zOffset);
            updating.Normal(-0.666667f, -0.333333f, 0.666667f);
            updating.TextureCoord(1, 0);

            updating.Triangle(1, 2, 3);
            updating.Triangle(3, 0, 1);

            updating.End();
            updating.BeginUpdate(3);

            //4
            updating.Position(x + xOffset, y + yOffset, z + zOffset);
            updating.Normal(0.666667f, 0.333333f, 0.666667f);
            updating.TextureCoord(1, 0);

            //11
            updating.Position(x + xOffset, -y + yOffset, -z + zOffset);
            updating.Normal(0.666667f, -0.333333f, -0.666667f);
            updating.TextureCoord(0, 1);

            //12
            updating.Position(x + xOffset, y + yOffset, -z + zOffset);
            updating.Normal(0.408248f, 0.816497f, -0.408248f);
            updating.TextureCoord(1, 1);

            //13
            updating.Position(x + xOffset, -y + yOffset, z + zOffset);
            updating.Normal(0.408248f, -0.816497f, 0.408248f);
            updating.TextureCoord(0, 0);

            updating.Triangle(0, 1, 2);
            updating.Triangle(0, 3, 1);

            updating.End();
            updating.BeginUpdate(4);

            //8
            updating.Position(-x + xOffset, y + yOffset, -z + zOffset);
            updating.Normal(-0.666667f, 0.333333f, -0.666667f);
            updating.TextureCoord(0, 1);

            //12
            updating.Position(x + xOffset, y + yOffset, -z + zOffset);
            updating.Normal(0.408248f, 0.816497f, -0.408248f);
            updating.TextureCoord(1, 1);

            //14
            updating.Position(x + xOffset, -y + yOffset, -z + zOffset);
            updating.Normal(0.666667f, -0.333333f, -0.666667f);
            updating.TextureCoord(1, 0);

            //15
            updating.Position(-x + xOffset, -y + yOffset, -z + zOffset);
            updating.Normal(-0.408248f, -0.816497f, -0.408248f);
            updating.TextureCoord(0, 0);

            updating.Triangle(2, 0, 1);
            updating.Triangle(2, 3, 0);

            updating.End();
            updating.BeginUpdate(5);

            //16
            updating.Position(-x + xOffset, y + yOffset, z + zOffset);
            updating.Normal(-0.408248f, 0.816497f, 0.408248f);
            updating.TextureCoord(1, 0);

            //17
            updating.Position(x + xOffset, y + yOffset, -z + zOffset);
            updating.Normal(0.408248f, 0.816497f, -0.408248f);
            updating.TextureCoord(0, 1);

            //18
            updating.Position(-x + xOffset, y + yOffset, -z + zOffset);
            updating.Normal(-0.666667f, 0.333333f, -0.666667f);
            updating.TextureCoord(1, 1);

            //19
            updating.Position(x + xOffset, y + yOffset, z + zOffset);
            updating.Normal(0.666667f, 0.333333f, 0.666667f);
            updating.TextureCoord(0, 0);

            updating.Triangle(0, 1, 2);
            updating.Triangle(0, 3, 1);

            updating.End();

            updating.BoundingBox = new AxisAlignedBox(
                new Vector3(
                    -x + xOffset,
                    -y + yOffset,
                    -z + zOffset),
                new Vector3(
                    x + xOffset,
                    y + yOffset,
                    z + zOffset));

            return updating;
        }
    }
}
