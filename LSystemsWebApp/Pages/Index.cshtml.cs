using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LSystems;
using LSystems.Core;
using LSystems.Utils;
using LSystems.Utils.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSystemsWebApp.Pages
{
	public class IndexModel : PageModel
	{
		public string Svg;
		public string Text;
		
		
		public void OnGet()
		{
			Svg = GenerateTree();
			Text = "none";
		}

		public void OnPost()
		{
			Svg = GenerateTree();
			var form1 = Request.Form["form-id"];
			Text = form1;
			//Text = Request.Form["test-input"];
			Module<double> module = new Module<double>(10);
			XmlSerializer serializer = new XmlSerializer(module.GetType());
			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, module);
			HttpContext.Session.SetString("module",writer.ToString());
			StringReader reader = new StringReader(writer.ToString());
			serializer.Deserialize(reader);
		}

		public void OnPostSubmitOne()
		{
			Text = Request.Form["form-id"][1];
			

		}

		public void OnPostSubmitTwo()
		{
			Text = "SubmitTwo";
		}
		
		private string GenerateTree()
		{
			var expressionParserFactory = new DoubleExpressionParserFactory();
			var generatorParser = new GeneratorParser<double>(expressionParserFactory);

			double constR = 1/Math.Sqrt(2);
			double constU = 45;

			var input =
				"F(200)\n" +
				"F(l)->H(l) Uloz Doleva F(l*" + constR + ") Nacti Uloz Doprava F(l*" + constR + ") Nacti";

			var reader = new StringReader(input);
			var generator = generatorParser.Parse(reader);

			generator.AdvanceNGenerations(8);
			var output = generator.CurrentGeneration;

			DrawingAction[] elements = new DrawingAction[]
			{
				new DrawingAction(0, canvas => canvas.AddToPath(new PathSegment(new IndexCanvasProperty<double>(0)))), 
				new DrawingAction(1, canvas => canvas.AddToPath(new PathSegment(new IndexCanvasProperty<double>(0)))), 
				new DrawingAction(2, canvas => canvas.PushToStack()),
				new DrawingAction(3, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(constU)))), 
				new DrawingAction(4, canvas => canvas.PopFromStack()),
				new DrawingAction(5, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(-constU))))
			};
			
			var creator = new VectorDrawer(elements);

			string svg = creator.Draw(output, 0.3);

			return svg;
		}
	}
}