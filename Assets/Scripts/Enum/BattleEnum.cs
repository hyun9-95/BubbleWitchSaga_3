public enum BubbleType
{
    // ��ȣ�ۿ�
    Normal,
    Fairy,
    Magic,

    // ��ȣ�ۿ� X
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
