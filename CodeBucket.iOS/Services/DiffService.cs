using System.Collections.Generic;
using System.Linq;
using CodeBucket.Core.Services;
using Newtonsoft.Json;

namespace CodeBucket.Services
{
    public class DiffService : IDiffService
    {
        private readonly JavaScriptCore.JSContext _context;

        public DiffService()
        {
            _context = new JavaScriptCore.JSContext();

            var script = System.IO.File.ReadAllText("WebResources/jsdiff.js");
            _context.EvaluateScript(script);
        }

        public IEnumerable<Hunk> CreateDiff(string oldContent, string newContent, int context)
        {
            if (oldContent == null && newContent == null)
                return Enumerable.Empty<Hunk>();

            if (oldContent == null)
                return new[] { new Hunk(0, 1, newContent.Split('\n')) };
            if (newContent == null)
                return new[] { new Hunk(1, 0, oldContent.Split('\n')) };

            var items = new { oldContent, newContent, context };
            var serialized = JsonConvert.SerializeObject(items);
            _context.EvaluateScript("item = " + serialized + ";");
            var ret = _context.EvaluateScript("JSON.stringify(JsDiff.structuredPatch('', '', item.oldContent, item.newContent, '', '', { context: item.context }).hunks);").ToString();
            var hunks = JsonConvert.DeserializeObject<List<JSHunk>>(ret);
            return hunks.Select(x => new Hunk(x.oldStart, x.newStart, x.lines));
        }

        private class JSHunk
        {
            public int oldStart { get; set; }
            public int newStart { get; set; }
            public List<string> lines { get; set; }
        }
    }
}

