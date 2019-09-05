using System;
using System.IO;
using LSystems;
using LSystems.Utils;
using LSystems.Utils.Parsers;
using NUnit.Framework;

namespace LSystemsTests
{
	[TestFixture]
	public class GeneratorTests
	{
		[Test]
		public static void TEST()
		{
			var expressionParserFactory = new IntExpressionParserFactory();
			var generatorParser = new GeneratorParser<int>(expressionParserFactory);

			var input =
				"A\n" +
				"B->BB\n" +
				"A->BCACA";

			var reader = new StringReader(input);
			var generator = generatorParser.Parse(reader);

			generator.AdvanceNGenerations(3);
			var output = generator.CurrentGeneration;
		}

		[Test]
		public static void DRAWING_TEST_TREE()
		{
			var expressionParserFactory = new DoubleExpressionParserFactory();
			var generatorParser = new GeneratorParser<double>(expressionParserFactory);

			var input =
				"Mlada\n" +
				"Mlada->StaraUlozDopravaMladaNactiUlozDolevaMladaNactiStaraMlada\n" +
				"Stara->StaraStara";

			var reader = new StringReader(input);
			var generator = generatorParser.Parse(reader);

			generator.AdvanceNGenerations(4);
			var output = generator.CurrentGeneration;

			var blackLine = new PathSegment(new ValueCanvasProperty<double>(10));
			blackLine.FgColor = Color.Black;
			var greenLine = new PathSegment(new ValueCanvasProperty<double>(10));
			greenLine.FgColor = Color.Green;
			
			DrawingAction[] elements = new DrawingAction[]
			{
				new DrawingAction(0, canvas => canvas.AddToPath(greenLine)), 
				new DrawingAction(1, canvas => canvas.AddToPath(blackLine)), 
				new DrawingAction(2, canvas => canvas.PushToStack()),
				new DrawingAction(3, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(45)))),
				new DrawingAction(4, canvas => canvas.PopFromStack()),
				new DrawingAction(5, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(-45))))
			};
			
			var creator = new VectorDrawer(elements);

			string svg = creator.Draw(output);
			
			Console.WriteLine(svg);
		}
		
		[Test]
		public static void DRAWING_TEST_GOSPER()
		{
			var expressionParserFactory = new DoubleExpressionParserFactory();
			var generatorParser = new GeneratorParser<double>(expressionParserFactory);

			var input =
				"L\n" +
				"L->L Doprava R Doprava Doprava R Doleva L Doleva Doleva L L Doleva R Doprava\n" +
				"R->Doleva L Doprava R R Doprava Doprava R Doprava L Doleva Doleva L Doleva R";

			var reader = new StringReader(input);
			var generator = generatorParser.Parse(reader);

			generator.AdvanceNGenerations(5);
			var output = generator.CurrentGeneration;

			DrawingAction[] elements = new DrawingAction[]
			{
				new DrawingAction(0, canvas => canvas.AddToPath(new PathSegment(new ValueCanvasProperty<double>(10)))), 
				new DrawingAction(1, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(60)))), 
				new DrawingAction(2, canvas => canvas.AddToPath(new PathSegment(new ValueCanvasProperty<double>(10)))), 
				new DrawingAction(3, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(-60))))
			};
			
			var creator = new VectorDrawer(elements);

			string svg = creator.Draw(output);
			
			Console.WriteLine(svg);
		}

		[Test]
		public static void DRAWING_TEST_HTREE()
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

			generator.AdvanceNGenerations(10);
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

			string svg = creator.Draw(output);
			
			Console.WriteLine(svg);
		}
		
		[Test]
		public static void DRAWING_TEST_LEAF()
		{
			var expressionParserFactory = new DoubleExpressionParserFactory();
			var generatorParser = new GeneratorParser<double>(expressionParserFactory);

			var input =
				"Uloz Otoc S(50) Nacti Uloz X Nacti Uloz Y Nacti\n" +
				"X->Uloz Doprava X Vytvor_polygon Polygon Nacti Polygon Z Polygon Ukonci_polygon \n" + 
				"Y->Uloz Doleva Y Vytvor_polygon Polygon Nacti Polygon Z Polygon Ukonci_polygon\n" + 
				"Z->Z F(100)\n" + 
				"S(len)->F(len) Rotace(1) S(len)";

			var reader = new StringReader(input);
			var generator = generatorParser.Parse(reader);

			generator.AdvanceNGenerations(20);
			var output = generator.CurrentGeneration;

			var greenLine = new PathSegment(new IndexCanvasProperty<double>(0));
			var polygon = new Polygon();
			polygon.FillColor = Color.Green;
			polygon.StrokeColor = Color.Black;

			DrawingAction[] elements = new DrawingAction[]
			{
				new DrawingAction(0, canvas => canvas.PushToStack()),
				new DrawingAction(1, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(180)))),
				new DrawingAction(2, canvas => canvas.AddToPath(greenLine)),
				new DrawingAction(3, canvas => canvas.PopFromStack()),
				new DrawingAction(4, canvas => { }),
				new DrawingAction(5, canvas => { }),
				new DrawingAction(6, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(10)))), 
				new DrawingAction(7, canvas => canvas.CreatePolygon(polygon)),
				new DrawingAction(8, canvas => canvas.AddPositionToPolygon()), 
				new DrawingAction(9, canvas => { }),
				new DrawingAction(10, canvas => canvas.ClosePolygon()), 
				new DrawingAction(11, canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(-10)))),
				new DrawingAction(12, canvas => canvas.AddToPath(greenLine)),
				new DrawingAction(13, canvas => canvas.Rotate(new Rotation(new IndexCanvasProperty<double>(0))))
			};
			
			var creator = new VectorDrawer(elements);

			string svg = creator.Draw(output);
			
			Console.WriteLine(svg);
		}
		
		[Test]
		public static void DRAWING_TEST_ROSE()
		{
			var expressionParserFactory = new DoubleExpressionParserFactory();
			var generatorParser = new GeneratorParser<double>(expressionParserFactory);

			var input =
				"Uloz Vytvor_polygon X(0,0) Polygon Ukonci_polygon Nacti Uloz Vytvor_polygon X(0,1) Polygon Ukonci_polygon\n" +
				"X(t,d):d=0->Polygon G(5, 1.15, -1) Polygon Uloz Doprava Y(t) G(3, 1.19, t) Polygon Ukonci_polygon Nacti Uloz Doprava Y(t) Vytvor_polygon Polygon Nacti X(t+1,d)\n" +
				"X(t,d):d=1->Polygon G(5, 1.15, -1) Polygon Uloz Doleva Y(t) G(3, 1.19, t) Polygon Ukonci_polygon Nacti Uloz Doleva Y(t) Vytvor_polygon Polygon Nacti X(t+1, d)\n" +
				"Y(t):t>0->G(1.3, 1.25, -1) Y(t-1)\n" +
				"G(s,r,t):t>1->G(s*r, r, t-1)\n" +
				"G(s,r,t):t=-1->G(s*r, r, -1)";

			var reader = new StringReader(input);
			var generator = generatorParser.Parse(reader);

			generator.AdvanceNGenerations(25);
			var output = generator.CurrentGeneration;

			var polygon = new Polygon();
			polygon.FillColor = Color.Green;
			polygon.StrokeColor = Color.Black;

			var moduleParser = generatorParser.ModuleParser;

			DrawingAction[] elements = new DrawingAction[]
			{
				new DrawingAction(moduleParser.GetModuleId("Uloz"), canvas => canvas.PushToStack()), 
				new DrawingAction(moduleParser.GetModuleId("Nacti"), canvas => canvas.PopFromStack()), 
				new DrawingAction(moduleParser.GetModuleId("Doprava"), canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(60)))), 
				new DrawingAction(moduleParser.GetModuleId("Doleva"), canvas => canvas.Rotate(new Rotation(new ValueCanvasProperty<double>(-60)))), 
				new DrawingAction(moduleParser.GetModuleId("Vytvor_polygon"), canvas => canvas.CreatePolygon(polygon)),
				new DrawingAction(moduleParser.GetModuleId("Ukonci_polygon"), canvas => canvas.ClosePolygon()),
				new DrawingAction(moduleParser.GetModuleId("Polygon"), canvas => canvas.AddPositionToPolygon()),
				new DrawingAction(moduleParser.GetModuleId("X"), canvas => canvas.Move(new Move(new IndexCanvasProperty<double>(0)))),
				new DrawingAction(moduleParser.GetModuleId("Y"), canvas => canvas.Move(new Move(new IndexCanvasProperty<double>(0)))),
				new DrawingAction(moduleParser.GetModuleId("G"), canvas => canvas.Move(new Move(new IndexCanvasProperty<double>(0)))),
			};
			
			var creator = new VectorDrawer(elements);

			string svg = creator.Draw(output);
			
			Console.WriteLine(svg);
		}
		
		[Test]
		public static void DRAWING_TEST_PYTHAGORAS()
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

			generator.AdvanceNGenerations(6);
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
			
			Console.WriteLine(svg);
		}
	}
}