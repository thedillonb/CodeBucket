using System;
using MonoTouch.JavaScriptCore;
using MonoTouch.Foundation;
using CodeBucket.Core.Services;

namespace CodeBucket.iOS.Services
{
	public class MarkdownService : IMarkdownService
    {
		public string ConvertMarkdown(string c)
		{
			if (string.IsNullOrEmpty(c))
				return string.Empty;

			using (var vm = new JSVirtualMachine())
			{
				var ctx = new JSContext(vm);
				var script = System.IO.File.ReadAllText("Markdown/marked.js", System.Text.Encoding.UTF8);
				ctx.EvaluateScript(script);
				var val = ctx[new NSString("marked")];
				return val.Call(JSValue.From(c, ctx)).ToString();
			}
		}

		public string ConvertTextile(string c)
		{
			if (string.IsNullOrEmpty(c))
				return string.Empty;

			using (var vm = new JSVirtualMachine())
			{
				var ctx = new JSContext(vm);
				var script = System.IO.File.ReadAllText("Markdown/textile.js", System.Text.Encoding.UTF8);
				ctx.EvaluateScript(script);
				var val = ctx[new NSString("convert")];
				return val.Call(JSValue.From(c, ctx)).ToString();
			}
		}

		public string ConvertCreole(string c)
		{
			if (string.IsNullOrEmpty(c))
				return string.Empty;

			using (var vm = new JSVirtualMachine())
			{
				var ctx = new JSContext(vm);
				var script = System.IO.File.ReadAllText("Markdown/creole.js", System.Text.Encoding.UTF8);
				ctx.EvaluateScript(script);
				var val = ctx[new NSString("convert")];
				return val.Call(JSValue.From(c, ctx)).ToString();
			}
		}
    }
}

