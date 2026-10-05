namespace LivingSim.Core;

/// <summary>Opt-in diagnostics. Counts observations without changing simulation decisions.</summary>
public sealed class HuntingTelemetry
{
    private readonly Dictionary<int, PredatorHuntStats> _predators = [];

    public IReadOnlyDictionary<int, PredatorHuntStats> Predators => _predators;
    public long CarcassNutritionCreated { get; private set; }
    public long KillCarcassNutritionCreated { get; private set; }
    public long CarcassNutritionConsumed { get; private set; }
    public long CarcassNutritionDecayed { get; private set; }

    internal void RecordCarcassCreated(int nutrition, bool killedByPredator)
    {
        CarcassNutritionCreated += nutrition;
        if (killedByPredator) KillCarcassNutritionCreated += nutrition;
    }
    internal void RecordCarcassConsumed(int nutrition) => CarcassNutritionConsumed += nutrition;
    internal void RecordCarcassDecayed() => CarcassNutritionDecayed++;

    internal void RecordTarget(int predatorId, int previousTargetId, int currentTargetId, bool previousTargetSurvived, long tick)
    {
        var stats = For(predatorId);
        if (currentTargetId != 0) stats.PreyDetections++;
        if (previousTargetId != 0 && previousTargetId != currentTargetId && previousTargetSurvived)
        {
            stats.AbandonedPursuits++;
            stats.CompletePursuit(tick);
        }
        if (currentTargetId != 0 && previousTargetId != currentTargetId)
        {
            stats.PursuitsStarted++;
            stats.StartPursuit(tick);
        }
    }

    internal void RecordMetabolism(int predatorId, int cost, bool pursuing)
    {
        var stats = For(predatorId);
        stats.MetabolicEnergySpent += cost;
        if (pursuing) stats.PursuitEnergySpent += cost;
    }
    internal void RecordMovement(int predatorId, int distance) => For(predatorId).DistanceTravelled += distance;
    internal void RecordAttack(int predatorId) => For(predatorId).Attacks++;
    internal void RecordKill(int predatorId, long tick)
    {
        var stats = For(predatorId);
        stats.Kills++;
        stats.CompletePursuit(tick);
    }
    internal void RecordMeal(int predatorId, int energy, long tick)
    {
        var stats = For(predatorId);
        stats.CarcassEnergyGained += energy;
        stats.Meals++;
        if (stats.LastMealTick >= 0) stats.TotalTicksBetweenMeals += tick - stats.LastMealTick;
        stats.LastMealTick = tick;
    }
    internal void RecordStarvation(int predatorId) => For(predatorId).StarvationDeaths++;

    private PredatorHuntStats For(int predatorId)
    {
        if (!_predators.TryGetValue(predatorId, out var stats)) _predators[predatorId] = stats = new PredatorHuntStats();
        return stats;
    }
}

public sealed class PredatorHuntStats
{
    public int PreyDetections { get; internal set; }
    public int PursuitsStarted { get; internal set; }
    public int AbandonedPursuits { get; internal set; }
    public int Attacks { get; internal set; }
    public int Kills { get; internal set; }
    public int Meals { get; internal set; }
    public int StarvationDeaths { get; internal set; }
    public long MetabolicEnergySpent { get; internal set; }
    public long PursuitEnergySpent { get; internal set; }
    public long DistanceTravelled { get; internal set; }
    public long CarcassEnergyGained { get; internal set; }
    public long TotalTicksBetweenMeals { get; internal set; }
    public long TotalPursuitTicks { get; private set; }
    public int CompletedPursuits { get; private set; }
    public long LastMealTick { get; internal set; } = -1;
    private long _pursuitStartTick = -1;

    internal void StartPursuit(long tick) => _pursuitStartTick = tick;

    internal void CompletePursuit(long tick)
    {
        if (_pursuitStartTick < 0) return;
        TotalPursuitTicks += tick - _pursuitStartTick + 1;
        CompletedPursuits++;
        _pursuitStartTick = -1;
    }
}
