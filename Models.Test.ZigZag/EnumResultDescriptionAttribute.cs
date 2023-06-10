


namespace ZigZag.Test;

public class EnumResultDescriptionAttribute : Attribute
{
    public readonly string Description;

    public EnumResultDescriptionAttribute(string description)
    {
        Description = description;
    }
}
