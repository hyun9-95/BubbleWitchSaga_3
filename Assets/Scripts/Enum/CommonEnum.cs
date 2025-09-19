public enum UICanvasType
{
    View,
    Popup,
    Front,
}

public enum TransitionType
{
    Default,
}

[System.Flags]
public enum LayerFlag
{
    None = 0,
    Default = 1 << 0,
    TransparentFX = 1 << 1,
    IgnoreRaycast = 1 << 2,
    Character = 1 << 3,
    Water = 1 << 4,
    UI = 1 << 5,
}

public enum LoadDataType
{
    Editor,
    Addressable,
}

public enum SoundType
{
    Bgm,
    Amb,
    Sfx,
    None = 99,
}

public enum ButtonSoundType
{
    Click,
    Confirm,
    Cancel,
    None = 99,
}