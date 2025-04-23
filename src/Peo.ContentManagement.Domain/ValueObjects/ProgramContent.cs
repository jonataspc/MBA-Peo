namespace Peo.ContentManagement.Domain.ValueObjects
{
    public class ProgramContent
    {
        public string? Content { get; private set; }

        public ProgramContent(string? content)
        {
            Content = content;
        }
    }
}