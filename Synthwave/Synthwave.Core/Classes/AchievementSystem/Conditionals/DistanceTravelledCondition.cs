using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.AchievementSystem.Conditionals;

public class DistanceTravelledCondition(float target) : IAchievementCondition
{
    #region Properties
    private float _target = target;
    private float _distance;

    private Vector3 _lastPosition;

    public bool IsCompleted => _distance >= _target;

    public float Progress => _distance / _target;
    #endregion

    #region Methods
    public void Initialize(AchievementContext context) => _lastPosition = context.Player.Position;
    
    public void Evaluate(AchievementContext context)
    {
        var current = context.Player.Position;

        _distance += Vector3.Distance(current, _lastPosition);

        _lastPosition = current;
    }
    #endregion
}
