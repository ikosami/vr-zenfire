[System.Flags]
public enum HandPartType
{
    None = 0,
    Thumb = 1,
    Index = 2,
    Middle = 4,
    Ring = 8,
    Little = 16,
    Palm = 32,
    Wrist = 64,
    All = Thumb | Index | Middle | Ring | Little | Palm | Wrist
}