using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Prototyp
{
    class Cube
    {
        Vector3 position;
        Model model;
        Matrix[] _boneTransforms;
        float scaling;
        Vector3 translate;
        float rotationX;
        float rotationY;
        float rotationZ;

        public Cube( Model model, float scaling, Vector3 translate, float rotationX, float rotationY, float rotationZ)
        {
            this.model = model;
            this.scaling = scaling;
            _boneTransforms = new Matrix[model.Bones.Count];
            this.translate = translate;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
        }

        public void Draw(GameTime gametime, Matrix projektion, Matrix view)
        {
            
            model.CopyAbsoluteBoneTransformsTo(_boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effects in mesh.Effects)
                {
                    effects.World = _boneTransforms[mesh.ParentBone.Index] *Matrix.CreateRotationX(rotationX)*Matrix.CreateRotationY(rotationY)*Matrix.CreateRotationZ(rotationZ)* Matrix.CreateScale(scaling) * Matrix.CreateTranslation(translate);
                    effects.View = view;
                    effects.Projection = projektion;
                    effects.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
