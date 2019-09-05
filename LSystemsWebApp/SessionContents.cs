using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LSystems;
using LSystems.Utils;
using Microsoft.AspNetCore.Http;

namespace LSystemsWebApp
{
	/*
	 * This file contains classes which store information about content which is inserted by user.
	 * Main purpose of these classes is to serialize them and store them in HTMLContext.Session
	 */
	public static class SessionConstants
	{
		public const string MODULE_INFOS_SESSION_KEY = "module_infos";
		public const string AXIOM_INFOS_SESSION_KEY = "axiom_infos";
		public const string RULE_INFOS_SESSION_KEY = "rule_infos";
	}

	public class SessionContents
	{
		private readonly ISession _session;
		
		public SessionContents(ISession session)
		{
			_session = session ?? throw new ArgumentException("session cannot be null");
		}
		
		public List<T> GetSessionContents<T>(string key)
		{
			var serialized = _session.GetString(key);
			if(string.IsNullOrWhiteSpace(serialized))
				return new List<T>();
			
			var serializer = new XmlSerializer(typeof(List<T>));
			var reader = new StringReader(serialized);
			return (List<T>) serializer.Deserialize(reader);
		}

		public void UpdateSessionContents<T>(string key, List<T> collection)
		{
			var serializer = new XmlSerializer(typeof(List<T>));
			var writer = new StringWriter();
			serializer.Serialize(writer, collection);
			_session.SetString(key, writer.ToString());
		}
	}

	[Serializable]
	public class ModuleInfo
	{
		public string ModuleName;
		public List<string> ParamNames;
		public RepresentationAction Representation;
		public RepresentationActionInfo RepresentationInfo;

		/// <summary>
		/// Returns human readable version of RepresentationInfo variable
		/// </summary>
		/// <returns>string with representation of RepresentationInfo</returns>
		public string GetRepresentationSummary()
		{
			if (RepresentationInfo == null)
				return string.Empty;
			else
				return RepresentationInfo.GetDescription();
		}

		public Action<Canvas> GetAction()
		{
			if (RepresentationInfo == null)
				return null;
			else
				return RepresentationInfo.GetAction(this);
		}

		[Serializable]
		public enum RepresentationAction
		{
			None,
			Line,
			Move,
			Rotation,
			Polygon,
			SavePosition,
			LoadPosition
		}
		
		[XmlInclude(typeof(NoneInfo))]
		[XmlInclude(typeof(LineInfo))]
		[XmlInclude(typeof(MoveInfo))]
		[XmlInclude(typeof(RotationInfo))]
		[XmlInclude(typeof(PolygonInfo))]
		[XmlInclude(typeof(PushToStackInfo))]
		[XmlInclude(typeof(PopFromStackInfo))]
		[Serializable]
		public abstract class RepresentationActionInfo
		{
			public abstract string GetDescription();
			
			public abstract Action<Canvas> GetAction(ModuleInfo info);

			protected CanvasProperty<double> CreateCanvasProperty(string representation, List<string> parameters)
			{
				if (double.TryParse(representation, out var value))
				{
					return new ValueCanvasProperty<double>(value);
				}
				else
				{
					var index = parameters.IndexOf(representation);
					if(index < 0)
						throw new ArgumentException("representation is not value nor variable");
					
					return new IndexCanvasProperty<double>(index);
				}
			}

		}

		[Serializable]
		public class NoneInfo : RepresentationActionInfo
		{
			public override string GetDescription()
			{
				return "None";
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				return canvas => { };
			}
		}
		
		[Serializable]
		public class LineInfo : RepresentationActionInfo
		{
			public string Length;
			public ColorInfo Color;
			
			public override string GetDescription()
			{
				return $"Line with length: {Length} and color {Color}";
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				return canvas =>
				{
					var lengthProperty = CreateCanvasProperty(Length, info.ParamNames);
					var redProperty = CreateCanvasProperty(Color.Red, info.ParamNames);
					var greenProperty = CreateCanvasProperty(Color.Green, info.ParamNames);
					var blueProperty = CreateCanvasProperty(Color.Blue, info.ParamNames);
					var segment = new PathSegment
					{
						Length =  lengthProperty, 
						FgColor = new Color(redProperty, greenProperty, blueProperty)
						//TODO: Consider adding support for stroke width
					};
					canvas.AddToPath(segment);
				};
			}
		}

		[Serializable]
		public class MoveInfo : RepresentationActionInfo
		{
			public string Length;
			
			public override string GetDescription()
			{
				return $"Move forward by: {Length}";
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				return canvas =>
				{
					var lengthProperty = CreateCanvasProperty(Length, info.ParamNames);
					var move = new Move(lengthProperty);
					canvas.Move(move);
				};
			}
		}

		[Serializable]
		public class RotationInfo : RepresentationActionInfo
		{
			public string Angle;
			
			public override string GetDescription()
			{
				return $"Rotation by angle: {Angle}";
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				return canvas =>
				{
					var angleProperty = CreateCanvasProperty(Angle, info.ParamNames);
					var rotation = new Rotation(angleProperty);
					canvas.Rotate(rotation);
				};
				
			}
		}

		[Serializable]
		public class PolygonInfo : RepresentationActionInfo
		{
			[Serializable]
			public enum PolygonAction
			{
				Create,
				AddPoint,
				Close
			}

			public PolygonAction Action;
			public ColorInfo StrokeColor;
			public ColorInfo FillColor;
			
			public override string GetDescription()
			{
				switch (Action)
				{
					case PolygonAction.Create:
						return $"Creates new polygon with stroke color {StrokeColor} and fill color {FillColor}";
					case PolygonAction.AddPoint:
						return "Adds current point to currently created polygon";
					case PolygonAction.Close:
						return "Closes current polygon and draws it";
					default:
						return string.Empty;
				}
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				switch (Action)
				{
					case PolygonAction.Create:
						return canvas =>
						{
							var strokeRedProperty = CreateCanvasProperty(StrokeColor.Red, info.ParamNames);
							var strokeGreenProperty = CreateCanvasProperty(StrokeColor.Green, info.ParamNames);
							var strokeBlueProperty = CreateCanvasProperty(StrokeColor.Blue, info.ParamNames);
							var strokeColor = new Color(strokeRedProperty, strokeGreenProperty, strokeBlueProperty);
							
							
							var fillRedProperty = CreateCanvasProperty(FillColor.Red, info.ParamNames);
							var fillGreenProperty = CreateCanvasProperty(FillColor.Green, info.ParamNames);
							var fillBlueProperty = CreateCanvasProperty(FillColor.Blue, info.ParamNames);
							var fillColor = new Color(fillRedProperty, fillGreenProperty, fillBlueProperty);

							var polygon = new Polygon
							{
								StrokeColor = strokeColor,
								FillColor = fillColor
							};
							canvas.CreatePolygon(polygon);
						};
					case PolygonAction.AddPoint:
						return canvas => canvas.AddPositionToPolygon();
					case PolygonAction.Close:
						return canvas => canvas.ClosePolygon();
					default:
						return canvas => { };
				}
			}
		}

		[Serializable]
		public class PushToStackInfo : RepresentationActionInfo
		{
			public override string GetDescription()
			{
				return "Saves current position to stack";
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				return canvas => canvas.PushToStack();
			}
		}

		[Serializable]
		public class PopFromStackInfo : RepresentationActionInfo
		{
			public override string GetDescription()
			{
				return "Loads last saved position from stack";
			}

			public override Action<Canvas> GetAction(ModuleInfo info)
			{
				return canvas => canvas.PopFromStack();
			}
		}

		[Serializable]
		public class ColorInfo
		{
			public string Red;
			public string Green;
			public string Blue;

			public ColorInfo()
			{
				Red = "0";
				Green = "0";
				Blue = "0";
			}
			
			public ColorInfo(string red, string green, string blue)
			{
				Red = red;
				Green = green;
				Blue = blue;
			}

			public override string ToString()
			{
				return $"R: {Red} G: {Green} B: {Blue}";
			}
		}
	}

	[Serializable]
	public class AxiomInfo
	{
		public string Name;
		public List<Param> Params;

		public struct Param
		{
			public string Name;
			public string Value;
		}
	}

	[Serializable]
	public class RuleInfo
	{
		public string SourceModule;
		public List<string> LeftContext;
		public List<string> RightContext;
		public List<string> ParametricConditions;
		public List<AxiomInfo> NextGeneration;
	}
}