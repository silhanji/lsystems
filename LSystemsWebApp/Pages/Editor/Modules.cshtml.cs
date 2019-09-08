using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSystemsWebApp.Pages.Editor
{
	public class Modules : PageModel
	{
		public List<ModuleInfo> ModuleInfos;
		public List<AxiomInfo> AxiomInfos;		
		public List<RuleInfo> RuleInfos;
		public bool DisplayEditor = false;
		
		public string ErrorMsg = null;
		
		public void OnGet()
		{
			GetInfoFromSession();
		}

		/// <summary>
		/// Handles POST requests which creates new ModuleInfo
		/// </summary>
		public void OnPostNewModule()
		{
			GetInfoFromSession();
			
			var moduleInfo = new ModuleInfo();
			
			var moduleName = Request.Form["module-name"];
			if (string.IsNullOrEmpty(moduleName))
			{
				ErrorMsg = "Module name must be set!";
				return;
			}

			moduleInfo.ModuleName = moduleName;
			
			var parameters = new List<string>();
			foreach (var parameter in Request.Form["param-name"])
				parameters.Add(parameter);

			moduleInfo.ParamNames = parameters;
			
			var representation = Request.Form["module-representation"];
			switch (representation)
			{
				case "none":
					moduleInfo.Representation = ModuleInfo.RepresentationAction.None;
					moduleInfo.RepresentationInfo = new ModuleInfo.NoneInfo();
					break;
				case "line":
					var lineInfo = new ModuleInfo.LineInfo();
					
					string length = Request.Form["length"];
					if (string.IsNullOrWhiteSpace(length))
					{
						ErrorMsg = "length must be defined";
						return;
					}
					lineInfo.Length = length;
					
					string colorRed = Request.Form["color-line-red"];
					if (string.IsNullOrWhiteSpace(colorRed))
					{
						ErrorMsg = "color red must be defined";
						return;
					}
					string colorGreen = Request.Form["color-line-green"];
					if (string.IsNullOrWhiteSpace(colorGreen))
					{
						ErrorMsg = "color green must be defined";
						return;
					}
					string colorBlue = Request.Form["color-line-blue"];
					if (string.IsNullOrWhiteSpace(colorBlue))
					{
						ErrorMsg = "color blue must be defined";
						return;
					}
					lineInfo.Color = new ModuleInfo.ColorInfo(colorRed, colorGreen, colorBlue);
					
					moduleInfo.Representation = ModuleInfo.RepresentationAction.Line;
					moduleInfo.RepresentationInfo = lineInfo;
					break;
				case "polygon":
					var polygonInfo = new ModuleInfo.PolygonInfo();
					
					string action = Request.Form["polygon-action"];
					if (string.IsNullOrWhiteSpace(action))
					{
						ErrorMsg = "Internal Error";
						return;
					}

					switch (action)
					{
						case "create-polygon":
							polygonInfo.Action = ModuleInfo.PolygonInfo.PolygonAction.Create;

							var strokeColorRed = Request.Form["color-polygon-stroke-red"];
							if (string.IsNullOrWhiteSpace(strokeColorRed))
							{
								ErrorMsg = "Stroke color red must be set";
								return;
							}
							var strokeColorGreen = Request.Form["color-polygon-stroke-green"];
							if (string.IsNullOrWhiteSpace(strokeColorGreen))
							{
								ErrorMsg = "Stroke color green must be set";
								return;
							}
							var strokeColorBlue = Request.Form["color-polygon-stroke-blue"];
							if (string.IsNullOrWhiteSpace(strokeColorBlue))
							{
								ErrorMsg = "Stroke color blue must be set";
								return;
							}
							polygonInfo.StrokeColor = 
								new ModuleInfo.ColorInfo(strokeColorRed, strokeColorGreen, strokeColorBlue);

							var fillColorRed = Request.Form["color-polygon-fill-red"];
							if (string.IsNullOrWhiteSpace(fillColorRed))
							{
								ErrorMsg = "Fill color red must be set";
								return;
							}
							var fillColorGreen = Request.Form["color-polygon-fill-green"];
							if (string.IsNullOrWhiteSpace(fillColorGreen))
							{
								ErrorMsg = "Fill color green must be set";
								return;
							}
							var fillColorBlue = Request.Form["color-polygon-fill-blue"];
							if (string.IsNullOrWhiteSpace(fillColorBlue))
							{
								ErrorMsg = "Fill color blue must be set";
								return;
							}
							polygonInfo.FillColor = 
								new ModuleInfo.ColorInfo(fillColorRed, fillColorGreen, fillColorBlue);
							break;
						case "add-to-polygon":
							polygonInfo.Action = ModuleInfo.PolygonInfo.PolygonAction.AddPoint;
							break;
						case "close-polygon":
							polygonInfo.Action = ModuleInfo.PolygonInfo.PolygonAction.Close;
							break;
						default:
							ErrorMsg = "Internal Error";
							return;
					}

					moduleInfo.Representation = ModuleInfo.RepresentationAction.Polygon;
					moduleInfo.RepresentationInfo = polygonInfo;
					break;
				case "move":
					var moveInfo = new ModuleInfo.MoveInfo();
					
					var moveLength = Request.Form["length"];
					if (string.IsNullOrWhiteSpace(moveLength))
					{
						ErrorMsg = "length must be set";
						return;
					}

					moveInfo.Length = moveLength;

					moduleInfo.Representation = ModuleInfo.RepresentationAction.Move;
					moduleInfo.RepresentationInfo = moveInfo;
					break;
				case "rotation":
					var rotationInfo = new ModuleInfo.RotationInfo();

					var angle = Request.Form["rotation-angle"];
					if (string.IsNullOrWhiteSpace(angle))
					{
						ErrorMsg = "Angle must be set";
						return;
					}
					rotationInfo.Angle = angle;

					moduleInfo.Representation = ModuleInfo.RepresentationAction.Rotation;
					moduleInfo.RepresentationInfo = rotationInfo;
					break;
				case "save":
					moduleInfo.Representation = ModuleInfo.RepresentationAction.SavePosition;
					moduleInfo.RepresentationInfo = new ModuleInfo.PushToStackInfo();
					break;
				case "load":
					moduleInfo.Representation = ModuleInfo.RepresentationAction.LoadPosition;
					moduleInfo.RepresentationInfo = new ModuleInfo.PopFromStackInfo();
					break;
				default:
					ErrorMsg = "Internal Error";
					return;
			}

			if (!CheckForConflict(moduleInfo))
			{
				ErrorMsg = "Module with this name already exists";
				return;
			}
			
			ModuleInfos.Add(moduleInfo);
			UpdateSessionModules();
		}

		/// <summary>
		/// Handles POST requests which shows dialog for new module creation
		/// </summary>
		public void OnPostCreateModule()
		{
			GetInfoFromSession();
			DisplayEditor = true;
		}

		/// <summary>
		/// Handles POST requests which removes module from ModuleInfos
		/// </summary>
		public void OnPostRemoveModule()
		{
			GetInfoFromSession();
			
			string formData = Request.Form["module-index"];
			if (!int.TryParse(formData, out int index) || index < 0 || index > ModuleInfos.Count)
			{
				ErrorMsg = "Internal Error";
				return;
			}

			var info = ModuleInfos[index];
			
			ModuleInfos.RemoveAt(index);
			RemoveObsoleteAxioms(info);
			RemoveObsoleteRules(info);
			UpdateSessionAll();
		}
		
		private bool CheckForConflict(ModuleInfo info)
		{
			string name = info.ModuleName.ToLower();
			foreach(var module in ModuleInfos)
				if (module.ModuleName.ToLower() == name)
					return false;

			return true;
		}
		private void RemoveObsoleteAxioms(ModuleInfo info)
		{

			AxiomInfos.RemoveAll(axiomInfo => axiomInfo.Name.ToLower() == info.ModuleName.ToLower());
		}

		private void RemoveObsoleteRules(ModuleInfo info)
		{
			var name = info.ModuleName.ToLower();
			
			RuleInfos.RemoveAll(ruleInfo => ruleInfo.SourceModule.ToLower() == name ||
			                                ruleInfo.LeftContext.Exists(s => s.ToLower() == name) ||
			                                ruleInfo.RightContext.Exists(s => s.ToLower() == name) ||
			                                ruleInfo.NextGeneration.Exists(
				                                axiomInfo => axiomInfo.Name.ToLower() == name));
		}
		
		/// <summary>
		/// Loads information from session and sets them. If session doesn't contain required information, sets default
		/// values instead.
		/// </summary>
		private void GetInfoFromSession()
		{
			var sessionContents = new SessionContents(HttpContext.Session);
			ModuleInfos = sessionContents.GetSessionContents<ModuleInfo>(SessionConstants.MODULE_INFOS_SESSION_KEY);
			AxiomInfos = sessionContents.GetSessionContents<AxiomInfo>(SessionConstants.AXIOM_INFOS_SESSION_KEY);
			RuleInfos = sessionContents.GetSessionContents<RuleInfo>(SessionConstants.RULE_INFOS_SESSION_KEY);
		}

		/// <summary>
		/// Updates data which relates to ModuleInfos stored in HTTPContext.Session
		/// </summary>
		private void UpdateSessionModules()
		{
			var sessionContents = new SessionContents(HttpContext.Session);
			sessionContents.UpdateSessionContents(SessionConstants.MODULE_INFOS_SESSION_KEY, ModuleInfos);
		}

		/// <summary>
		/// Updates all data stored in HTTPContext.Session
		/// </summary>
		private void UpdateSessionAll()
		{
			var sessionContents = new SessionContents(HttpContext.Session);
			sessionContents.UpdateSessionContents(SessionConstants.MODULE_INFOS_SESSION_KEY, ModuleInfos);
			sessionContents.UpdateSessionContents(SessionConstants.AXIOM_INFOS_SESSION_KEY, AxiomInfos);
			sessionContents.UpdateSessionContents(SessionConstants.RULE_INFOS_SESSION_KEY, RuleInfos);
		}
	}
}