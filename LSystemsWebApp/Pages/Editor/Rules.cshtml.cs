using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSystemsWebApp.Pages.Editor
{
	public class Rules : PageModel
	{
		public string ErrorMsg = null;
		public bool DisplayEditor = false;
		
		public List<ModuleInfo> ModuleInfos;
		public List<RuleInfo> RuleInfos;
		
		public void OnGet()
		{
			GetInfoFromSession();
		}

		public void OnPostCreateRule()
		{
			GetInfoFromSession();
			DisplayEditor = true;
		}

		public void OnPostRemoveRule()
		{
			GetInfoFromSession();

			var ruleIndexString = Request.Form["rule-index"];
			if (string.IsNullOrWhiteSpace(ruleIndexString) || !int.TryParse(ruleIndexString, out var index))
			{
				ErrorMsg = "Internal Error";
				return;
			}
			
			RuleInfos.RemoveAt(index);
			
			UpdateSession();
		}
		
		public void OnPostNewRule()
		{
			GetInfoFromSession();
			
			var source = Request.Form["source-module"];
			if (string.IsNullOrWhiteSpace(source))
			{
				ErrorMsg = "InternalError";
				return;
			}

			DragFormHelper.TryGetFormData(Request.Form, "next", out var nextGenInfo, out var error);
			if (!string.IsNullOrWhiteSpace(error))
				ErrorMsg = error;

			DragFormHelper.TryGetFormData(Request.Form, "left", out var leftContextInfo, out error);
			if (!string.IsNullOrWhiteSpace(error))
				ErrorMsg = error;

			var leftContextNames = ExtractModuleNames(leftContextInfo);

			DragFormHelper.TryGetFormData(Request.Form, "right", out var rightContextInfo, out error);
			if (!string.IsNullOrWhiteSpace(error))
				ErrorMsg = error;

			var rightContextNames = ExtractModuleNames(rightContextInfo);
			
			RuleInfos.Add(new RuleInfo
			{
				SourceModule = source, 
				NextGeneration = nextGenInfo,
				LeftContext = leftContextNames, 
				RightContext = rightContextNames, 
			});
			
			UpdateSession();
		}

		private List<string> ExtractModuleNames(List<AxiomInfo> axiomInfos)
		{
			var result = new List<string>();
			foreach(var info in axiomInfos)
				result.Add(info.Name);
			return result;
		}

		public string BuildContextDescription(List<string> context)
		{
			var builder = new StringBuilder();
			if (context.Count > 0)
			{
				for (int i = 0; i < context.Count - 1; i++)
				{
					builder.Append(context[i]);
					builder.Append(' ');
				}
				builder.Append(context[context.Count - 1]);
			}

			return builder.ToString();
		}

		public string BuildNextGenDescription(List<AxiomInfo> nextGen)
		{
			var builder = new StringBuilder();
			foreach (var info in nextGen)
			{
				builder.Append(info.Name);
				if (info.Params.Count != 0)
				{
					builder.Append('(');	
					for(int i = 0; i < info.Params.Count -1; i++)
					{
						builder.Append(info.Params[i].Value);
						builder.Append(", ");
					}

					builder.Append(info.Params[info.Params.Count - 1].Value);
					builder.Append(") ");
				}

				builder.Append(" ");
			}

			return builder.ToString();
		}
		
		/// <summary>
		/// Loads information from session and sets them. If session doesn't contain required information, sets default
		/// values instead.
		/// </summary>
		private void GetInfoFromSession()
		{
			var serialized = HttpContext.Session.GetString(SessionConstants.MODULE_INFOS_SESSION_KEY);

			XmlSerializer serializer;
			StringReader reader;
			
			if (string.IsNullOrWhiteSpace(serialized))
			{
				ModuleInfos = new List<ModuleInfo>();
			}
			else
			{
				serializer = new XmlSerializer(typeof(List<ModuleInfo>));
				reader = new StringReader(serialized);
				ModuleInfos = (List<ModuleInfo>)serializer.Deserialize(reader);
			}

			serialized = HttpContext.Session.GetString(SessionConstants.RULE_INFOS_SESSION_KEY);

			if (string.IsNullOrWhiteSpace(serialized))
			{
				RuleInfos = new List<RuleInfo>();
			}
			else
			{
				serializer = new XmlSerializer(typeof(List<RuleInfo>));
				reader = new StringReader(serialized);
				RuleInfos = (List<RuleInfo>) serializer.Deserialize(reader);
			}
		}

		/// <summary>
		/// Updates data stored in HTTPContext.Session
		/// </summary>
		private void UpdateSession()
		{
			var serializer = new XmlSerializer(typeof(List<RuleInfo>));
			var writer = new StringWriter();
			serializer.Serialize(writer, RuleInfos);
			HttpContext.Session.SetString(SessionConstants.RULE_INFOS_SESSION_KEY, writer.ToString());
		}
	}
}