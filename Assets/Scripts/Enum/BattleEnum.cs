public enum BubbleType
{
    // 상호작용
    Normal,
    Fairy,
    Magic,

    // 상호작용 X
    Empty,
    Spawn,
    None,
}

public enum  BubbleColor
{
    Red,
    Yellow,
    Blue,
}

public enum StageType
{
    None,
    Boss,
}

public enum BossSpawnType
{
    None,
    Snake_BothSide,
}

public enum BattleCellDirection
{
    Left,
    TopLeft,
    TopRight,
    Right,
    BottomLeft,
    BottomRight,
}

public enum BattlePhase
{
    None,
    Stage,
    Player,
    Interaction,
    Win,
    Defeat,
}
