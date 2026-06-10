using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Synthwave.Core.Classes.Core;

public class ModelLoader
{
    #region Properties
    private readonly Model carModel;
    private readonly ContentManager Content;
    #endregion

    #region Constructor
    public ModelLoader(ContentManager content)
    {
        Content = content;
       // carModel = Content.Load<Model>("Models/Cars/R8");
    }
    #endregion

    #region Methods
    private void DrawModel(Model model, Matrix world)
    {
        if (model == null) return;
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects.Cast<BasicEffect>())
            {
                effect.World = world;

                effect.EnableDefaultLighting();
            }

            mesh.Draw();
        }
    }

    public void Draw()
    {
        DrawModel(carModel, Matrix.CreateScale(1f) * Matrix.CreateTranslation(0, 0, 0));
    }
    #endregion
}
