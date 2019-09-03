using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSystemsWebApp.Pages.Editor
{
	public class Axiom : PageModel
	{
		public List<ModuleInfo> ModuleInfos;
		public List<AxiomInfo> AxiomInfos; 
		
		public string ErrorMsg = null;
		
		public void OnGet()
		{
			GetInfoFromSession();
		}

		public void OnPostSetAxiom()
		{
			DragFormHelper.TryGetFormData(Request.Form, "axiom", out AxiomInfos, out ErrorMsg);
			
			UpdateSession();
			GetInfoFromSession();
		}
		
		/// <summary>
		/// Loads information from session and sets them. If session doesn't contain required information, sets default
		/// values instead.
		/// </summary>
		private void GetInfoFromSession()
		{
			var serializedModules = HttpContext.Session.GetString(SessionConstants.MODULE_INFOS_SESSION_KEY);
			var serializedAxiom = HttpContext.Session.GetString(SessionConstants.AXIOM_INFOS_SESSION_KEY);

			XmlSerializer serializer;
			StringReader reader;
			
			if (string.IsNullOrWhiteSpace(serializedModules))
			{
				ModuleInfos = new List<ModuleInfo>();
			}
			else
			{
				serializer = new XmlSerializer(typeof(List<ModuleInfo>));
				reader = new StringReader(serializedModules);
				ModuleInfos = (List<ModuleInfo>)serializer.Deserialize(reader);
			}

			if (string.IsNullOrWhiteSpace(serializedAxiom))
			{
				AxiomInfos = new List<AxiomInfo>();
			}
			else
			{
				serializer = new XmlSerializer(typeof(List<AxiomInfo>));
				reader = new StringReader(serializedAxiom);
				AxiomInfos = (List<AxiomInfo>) serializer.Deserialize(reader);
			}
		}

		/// <summary>
		/// Updates data stored in HTTPContext.Session
		/// </summary>
		private void UpdateSession()
		{
			var serializer = new XmlSerializer(typeof(List<AxiomInfo>));
			var writer = new StringWriter();
			serializer.Serialize(writer, AxiomInfos);
			HttpContext.Session.SetString(SessionConstants.AXIOM_INFOS_SESSION_KEY, writer.ToString());
		}
	}
}