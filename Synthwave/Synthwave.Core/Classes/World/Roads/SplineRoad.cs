using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Roads;

public class SplineRoad
{
    #region Properties
    public List<Vector3> Points { get; } = [];
    #endregion

    #region Constructor
    public SplineRoad() => Generate();
    #endregion

    #region Methods
    private void Generate()
    {
        for (float z = -1000; z < 1000; z += 20f)
        {
            Points.Add(new Vector3(0,0,z));
        }
    }
    #endregion
}