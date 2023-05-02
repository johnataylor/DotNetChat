namespace DotNetChat
{
    public class ToolAttribute : Attribute
    {
        public string? Description { get; set; }

        public string? Name { get; set; }
    }
}
