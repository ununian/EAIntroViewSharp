using ObjCRuntime;

namespace EAIntroView
{
    public enum EAIntroViewTags : uint
    {
        TitleLabelTag = 1,
        DescLabelTag,
        TitleImageViewTag
    }

    [Native]
    public enum EAViewAlignment : ulong
    {
        Left,
        Center,
        Right
    }
}
