using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Weather;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Core._3DModel;

public class ModelLoader(ContentManager content, GraphicsDevice device, MaterialFactory materials)
{
    private readonly ContentManager _content = content;
    private readonly GraphicsDevice _device = device;
    private readonly MaterialFactory _materials = materials;

    private Model _carModel;
    private Model _lampModel;
    private Model _yukkaModel;
    private Model _buildingModel;

    private readonly List<LampInstance> _lamps = [];
    private readonly List<YukkaInstance> _yukkas = [];
    private readonly List<BuildingInstance> _buildings = [];

    private const float LampDrawDist = 600f;
    private const float YukkaDrawDist = 500f;
    private const float BuildingDrawDist = 2000f;

    private record LampInstance(Vector3 Position, float Yaw);
    private record YukkaInstance(Vector3 Position, float Scale, float Yaw);
    private record BuildingInstance(Vector3 Position, Vector3 Scale, Color Tint);

    public void LoadModels()
    {
        try
        {
            // _carModel = _content.Load<Model>("Models/Cars/R8");
            _lampModel = _content.Load<Model>("Models/StreetLight");
            _yukkaModel = _content.Load<Model>("Models/PalmTree");
            _buildingModel = _content.Load<Model>("Models/Building");
        }catch(Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }

    public void Populate(RoadSplineSystem roads, TerrainSystem terrain, int seed = 1337)
    {
        _lamps.Clear();
        _yukkas.Clear();
        _buildings.Clear();

        var rng = new Random(seed);

        foreach (var spline in roads.Splines)
        {
            float stepT = 0.02f;

            for (float t = 0f; t < 1f - stepT; t += stepT)
            {
                Vector3 p0 = spline.Evaluate(t);
                Vector3 p1 = spline.Evaluate(t + stepT);

                Vector3 tangent = Vector3.Normalize(p1 - p0);
                Vector3 right = Vector3.Normalize(Vector3.Cross(tangent, Vector3.Up));

                float roadHalf = 10f;

                float worldDist = Vector3.Distance(p0, p1) / stepT;
                bool placeLamp = (int)(t / stepT) % (int)MathF.Max(1f, 100f / worldDist) == 0;

                if (placeLamp)
                {
                    int lampIdx = (int)(t / stepT);
                    float side = (lampIdx % 2 == 0) ? 1f : -1f;

                    Vector3 lampPos = p0 + right * (roadHalf + 3f) * side;
                    lampPos.Y = terrain.GetHeight(lampPos.X, lampPos.Z);

                    float lampYaw = MathF.Atan2(tangent.X, tangent.Z);
                    _lamps.Add(new LampInstance(lampPos, lampYaw));

                    Vector3 yukkaOffset = -tangent * 2f + right * (roadHalf + 6f) * side;
                    Vector3 yukkaPos = p0 + yukkaOffset;
                    yukkaPos.Y = terrain.GetHeight(yukkaPos.X, yukkaPos.Z);

                    float yukkaScale = 0.8f + (float)rng.NextDouble() * 0.6f;
                    float yukkaYaw = (float)rng.NextDouble() * MathF.Tau;

                    _yukkas.Add(new YukkaInstance(yukkaPos, yukkaScale, yukkaYaw));
                }

                if (rng.NextDouble() < 0.04f)
                {
                    float side = rng.Next(2) == 0 ? 1f : -1f;
                    float setback = roadHalf + 15f + (float)rng.NextDouble() * 40f;

                    Vector3 bPos = p0 + right * setback * side;
                    bPos.Y = terrain.GetHeight(bPos.X, bPos.Z);

                    float bWidth = 10f + (float)rng.NextDouble() * 30f;
                    float bHeight = 15f + (float)rng.NextDouble() * 60f;
                    float bDepth = 10f + (float)rng.NextDouble() * 30f;

                    Color[] palette =
                    [
                            new Color(1f, 0.1f, 0.6f),
                            new Color(0.5f, 0f, 1f),
                            new Color(0f, 0.8f, 1f),
                            new Color(0.9f, 0.2f, 0.9f),
                            new Color(0.1f, 1f, 0.7f),
                        ];

                    _buildings.Add(new BuildingInstance(
                        bPos,
                        new Vector3(bWidth, bHeight, bDepth),
                        palette[rng.Next(palette.Length)]
                    ));
                }
            }
        }
    }

    public void Draw(Camera3D camera, WeatherSystem weather, EnvironmentLightingState lighting, GameTime gameTime)
    {
        float time = (float)gameTime.TotalGameTime.TotalSeconds;

      //  DrawCar(camera, lighting);
        DrawLamps(camera, lighting, time);
        DrawYukkas(camera, lighting);
        DrawBuildings(camera, lighting, time);
    }

    private void DrawCar(Camera3D camera, EnvironmentLightingState lighting)
    {
        var material = _materials.CreateCarMaterial();

        Matrix world =
            Matrix.CreateScale(0.012f) *
            Matrix.CreateRotationY(camera.Vehicle.Yaw + MathF.PI) *
            Matrix.CreateTranslation(camera.Vehicle.Position);

        DrawModel(_carModel, world, camera, material, lighting);
    }

    private void DrawLamps(Camera3D camera, EnvironmentLightingState lighting, float time)
    {
        var baseMaterial = _materials.CreateLampMaterial();

        foreach (var lamp in _lamps)
        {
            if (Vector3.Distance(camera.Position, lamp.Position) > LampDrawDist)
                continue;

            float flicker = 0.6f + 0.4f * MathF.Sin(time * 5f + lamp.Position.X);

            baseMaterial.EmissiveStrength = lighting.NightFactor * 3f * flicker;

            Matrix world =
                Matrix.CreateScale(0.5f) *
                Matrix.CreateRotationY(lamp.Yaw) *
                Matrix.CreateTranslation(lamp.Position);

            DrawModel(_lampModel, world, camera, baseMaterial, lighting);
        }
    }

    private void DrawYukkas(Camera3D camera, EnvironmentLightingState lighting)
    {
        var material = _materials.CreateYukkaMaterial();

        foreach (var yukka in _yukkas)
        {
            if (Vector3.Distance(camera.Position, yukka.Position) > YukkaDrawDist)
                continue;

            Matrix world =
                Matrix.CreateScale(yukka.Scale) *
                Matrix.CreateRotationY(yukka.Yaw) *
                Matrix.CreateTranslation(yukka.Position);

            DrawModel(_yukkaModel, world, camera, material, lighting);
        }
    }

    private void DrawBuildings(Camera3D camera, EnvironmentLightingState lighting, float time)
    {
        var material = _materials.CreateBuildingMaterial();

        foreach (var b in _buildings)
        {
            if (Vector3.Distance(camera.Position, b.Position) > BuildingDrawDist)
                continue;

            float flicker = 0.3f + 0.7f * MathF.Sin(time * 2f + b.Position.X);

            material.EmissiveStrength = lighting.NightFactor * flicker;

            Matrix world =
                Matrix.CreateScale(b.Scale) *
                Matrix.CreateTranslation(b.Position);

            DrawModel(_buildingModel, world, camera, material, lighting);
        }
    }

    private static void DrawModel(Model model, Matrix world, Camera3D camera, Material material, EnvironmentLightingState lighting)
    {
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                var effect = material.Effect;

                effect.Parameters["World"]?.SetValue(world);
                effect.Parameters["View"]?.SetValue(camera.View);
                effect.Parameters["Projection"]?.SetValue(camera.Projection);

                effect.Parameters["DiffuseMap"]?.SetValue(material.Diffuse);

                float emissive = material.UseEmissive
                    ? lighting.NightFactor * material.EmissiveStrength
                    : 0f;

                effect.Parameters["EmissiveStrength"]?.SetValue(emissive);

                if (material.Emissive != null)
                    effect.Parameters["EmissiveMap"]?.SetValue(material.Emissive);

                foreach (var pass in effect.CurrentTechnique.Passes)
                    pass.Apply();
            }

            mesh.Draw();
        }
    }
}