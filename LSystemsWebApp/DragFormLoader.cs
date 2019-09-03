using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LSystemsWebApp
{
	/// <summary>
	/// Loads data from DragForm
	/// </summary>
	public static class DragFormHelper
	{
		/// <summary>
		/// Loads data from drag form and returns them.
		/// </summary>
		/// <param name="form">Form to read data from</param>
		/// <param name="prefix">Prefix of inputs corresponding to drag form</param>
		/// <param name="axiomInfos">List of all axiomInfos that could be read correctly</param>
		/// <param name="errorMsg">Message indicating what kind of error occured, empty if no error occurs</param>
		/// <returns>true if no errors are present in form, false otherwise</returns>
		public static bool TryGetFormData(IFormCollection form, string prefix, 
			out List<AxiomInfo> axiomInfos, out string errorMsg)
		{
			errorMsg = null;
			axiomInfos = new List<AxiomInfo>();
			var returnValue = true;
			
			
			string modulesCount = form[prefix + "-count"];
			if(string.IsNullOrWhiteSpace(modulesCount) || !int.TryParse(modulesCount, out var count))
			{
				errorMsg = "Internal Error";
				return false;
			}

			for (int currentModule = 0; currentModule < count; currentModule++)
			{
				string atomsCount = form[prefix + "-" + currentModule + "-0"];
				if (string.IsNullOrWhiteSpace(atomsCount) || !int.TryParse(atomsCount, out var paramCount))
				{
					returnValue = false;
					continue;
				}
				
				string moduleName = form[prefix + "-" + currentModule + "-1"];
				if (string.IsNullOrWhiteSpace(moduleName))
				{
					returnValue = false;
					continue;
				}
				
				var wasError = false;
				var paramsValues = new List<AxiomInfo.Param>();
				for (int currentParam = 2; currentParam <= paramCount; currentParam++)
				{
					string paramName = form[prefix + "-" + currentModule + "-" + currentParam + "-name"];
					if (string.IsNullOrWhiteSpace(paramName))
					{
						errorMsg = "Internal Error";
						wasError = true;
						break;
					}
					
					string paramValue = form[prefix + "-" + currentModule + "-" + currentParam];
					if (string.IsNullOrWhiteSpace(paramValue))
					{
						errorMsg = "All parameter values must be filled";
						wasError = true;
						break;
					}

					var param = new AxiomInfo.Param {Name = paramName, Value = paramValue};

					paramsValues.Add(param);
				}

				if (wasError)
				{
					returnValue = false;
					continue;
				}

				var info = new AxiomInfo {Name = moduleName, Params = paramsValues};
				axiomInfos.Add(info);
			}

			return returnValue;
		}
	}
}