
namespace CodeBucket.Core.Services
{
    public interface IMarkdownService
    {
		string ConvertMarkdown(string c);
		string ConvertTextile(string c);
		string ConvertCreole(string c);
    }
}

