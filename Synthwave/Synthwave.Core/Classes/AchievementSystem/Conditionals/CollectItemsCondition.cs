namespace Synthwave.Core.Classes.AchievementSystem.Conditionals;

public class CollectItemsCondition(int target) : IAchievementCondition
{
    #region Properties
    private int _target = target;
    private int _current;

    public bool IsCompleted => _current >= _target;

    public float Progress => (float)_current / _target;
    #endregion

    #region Methods
    public void Initialize(AchievementContext context) => _current = 0;
    
    public void Evaluate(AchievementContext context) => _current = context.Player.Inventory.GetTotalItems();
    #endregion
}
