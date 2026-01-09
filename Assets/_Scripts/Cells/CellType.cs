namespace _Scripts
{
    public enum CellType : byte
    {
        Default = 0,
        LowerLeftCorner = 1,
        LowerRightCorner = 2,
        UpperLeftCorner = 3,
        UpperRightCorner = 4,
        BottomCorner = 5,
        UpperCorner = 6,
        LeftCorner = 7,
        RightCorner = 8,
        LeftCornerWall = 9,
        RightCornerWall = 10,
        BottomCornerWall = 11,
    }
}