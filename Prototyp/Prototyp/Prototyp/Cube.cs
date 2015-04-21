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

        public Cube(Vector3 position, Model model, float scaling)
        {
            this.model = model;
            this.position = position;
            this.scaling = scaling;
            _boneTransforms = new Matrix[model.Bones.Count];
        }

        public void Draw(GameTime gametime, Matrix projektion, Matrix view)
        {
            
            model.CopyAbsoluteBoneTransformsTo(_boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effects in mesh.Effects)
                {
                    effects.World = _boneTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(2.0f, 0.0f, 3.0f);
                    effects.View = view;
                    effects.Projection = projektion;
                    effects.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
