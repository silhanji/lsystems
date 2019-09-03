using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace LSystems
{
	public class VectorDrawer
	{
		private Dictionary<int, Action<Canvas>> _elements;
		private Canvas _canvas;
		
		public VectorDrawer(DrawingAction[] elements)
		{
			if(elements == null)
				throw new ArgumentException("elements cannot be null");
			
			_elements = new Dictionary<int, Action<Canvas>>();
			foreach(var element in elements)
				_elements.Add(element.ID, element.Handler);
			
			_canvas = new Canvas();
		}

		public string Draw(Generation<double> modules, double randomDerivationLimit = 0)
		{
			_canvas.Clear();
			if(randomDerivationLimit > 0)
				_canvas.SetRandomDerivationLimit(randomDerivationLimit);
			else
				_canvas.ClearRandomDerivationLimit();
			
			for (int i = 0; i < modules.Count; i++)
			{
				bool actionExists = _elements.TryGetValue(modules[i].Id, out var action);
				if(!actionExists)
					continue; //TODO: Change to throw exception here?

				_canvas.SetModule(modules[i]);
				action(_canvas);
			}

			return _canvas.GetSvg();
		}
	}

	public struct Position
	{
		public double X;
		public double Y;
		public double Angle;

		public Position(double x, double y, double angle)
		{
			X = x;
			Y = y;
			Angle = angle;
		}
	}

	public class Canvas
	{
		private Random _random;
		private bool _isRandomized;
		private double _randomDerivationLimit;
		
		private Position _currentPosition;
		private StringBuilder _svgBuilder;
		private Stack<Position> _positionStack;

		private StringBuilder _pathBuilder;
		
		private StringBuilder _polygonBuilder;
		private Stack<StringBuilder> _polygonStack;

		private double _minX, _minY, _maxX, _maxY;
		
		private PathSegment _currentPathSegment;
		private bool _isPathBuilt;
		
		private Module<double> _currentModule;
		
		public Canvas()
		{
			_random = new Random(); //TODO: Add seed ability

			_isRandomized = false;
			_randomDerivationLimit = 0;
			
			_svgBuilder = new StringBuilder();
			
			_positionStack = new Stack<Position>();
			_polygonStack = new Stack<StringBuilder>();
			
			Clear();
		}
		
		public void Clear()
		{
			_currentPosition = new Position(0, 0, 270);
			_svgBuilder.Clear();
			_pathBuilder = null;
			_polygonBuilder = null;
			_positionStack.Clear();
			_polygonStack.Clear();
			_minX = 0;
			_minY = 0;
			_maxX = 0;
			_maxY = 0;
		}

		public void SetModule(Module<double> module)
		{
			_currentModule = module;
		}
		
		public void AddToPath(PathSegment pathSegment)
		{
			if (_isPathBuilt == false)
			{
				CreatePath();
				_currentPathSegment = pathSegment;
			}
			else if (pathSegment.Equals(_currentPathSegment) == false)
			{
				ClosePath();
				_currentPathSegment = pathSegment;
				CreatePath();
			}

			_currentPosition = CalcPosition(pathSegment.Length.GetCanvasProperty(_currentModule));
			_pathBuilder.Append("L ");
			_pathBuilder.Append(_currentPosition.X);
			_pathBuilder.Append(" ");
			_pathBuilder.Append(_currentPosition.Y);
			_pathBuilder.Append(" ");
		}

		public void Move(Move move)
		{
			double distance = move.Distance.GetCanvasProperty(_currentModule);
			_currentPosition = CalcPosition(distance);
			
			//TODO: Consider closing path here
			if (_isPathBuilt)
			{
				_pathBuilder.Append("M ");
				_pathBuilder.Append(_currentPosition.X);
				_pathBuilder.Append(" ");
				_pathBuilder.Append(_currentPosition.Y);
				_pathBuilder.Append(" ");
			}
		}

		public void Rotate(Rotation rotation)
		{
			var angleChange = rotation.Angle.GetCanvasProperty(_currentModule);
			if (_isRandomized)
				angleChange *= _random.NextDouble() % (2 * _randomDerivationLimit) - _randomDerivationLimit + 1;
			
			double angle = (_currentPosition.Angle + angleChange) % 360;
			_currentPosition = new Position(_currentPosition.X, _currentPosition.Y, angle);
		}

		public void PushToStack()
		{
			_positionStack.Push(_currentPosition);
		}

		public void PopFromStack()
		{
			if(_positionStack.Count == 0) throw new Exception(); //TODO: Create custom exception here
			_currentPosition = _positionStack.Pop();

			if (_isPathBuilt)
			{
				_pathBuilder.Append("M ");
				_pathBuilder.Append(_currentPosition.X);
				_pathBuilder.Append(" ");
				_pathBuilder.Append(_currentPosition.Y);
				_pathBuilder.Append(" ");
			}
		}

		public void CreatePolygon(Polygon polygon)
		{
			if (_polygonBuilder != null)
			{
				_polygonStack.Push(_polygonBuilder);
			}

			_polygonBuilder = new StringBuilder();
			_polygonBuilder.Append("<polygon ");
			_polygonBuilder.Append("fill=\"");
			_polygonBuilder.Append(polygon.FillColor.GetColor(_currentModule));
			_polygonBuilder.Append("\" stroke=\"");
			_polygonBuilder.Append(polygon.StrokeColor.GetColor(_currentModule));
			_polygonBuilder.Append("\" points=\"");
		}

		public void AddPositionToPolygon()
		{
			_polygonBuilder.Append(_currentPosition.X);
			_polygonBuilder.Append(",");
			_polygonBuilder.Append(_currentPosition.Y);
			_polygonBuilder.Append(" ");
		}
		
		public void ClosePolygon()
		{
			_polygonBuilder.Append("\" />");

			_svgBuilder.Append(_polygonBuilder.ToString());
			if (_polygonStack.Count > 0)
				_polygonBuilder = _polygonStack.Pop();
			else
				_polygonBuilder = null;
		}
		
		public string GetSvg()
		{
			if(_isPathBuilt) ClosePath();

			double padding = 20;
			
			var start = "<svg xmlns='http://www.w3.org/2000/svg'";
			var viewbox = " viewBox=\"" + (_minX - padding / 2) + " " + (_minY - padding / 2) + " " + 
			              (_maxX - _minX + padding) + " " + (_maxY - _minY + padding) + "\">";
			var end = "</svg>";

			return start + viewbox + _svgBuilder.ToString() + end;
		}

		public void SetRandomDerivationLimit(double randomDerivationLimit)
		{
			if(randomDerivationLimit <= 0)
				throw new ArgumentException("randomDerivationLimit must be positive");
			if(randomDerivationLimit > 1)
				throw new ArgumentException("randomDerivationLimit must be smaller than 1");
			
			_isRandomized = true;
			_randomDerivationLimit = randomDerivationLimit;
		}

		public void ClearRandomDerivationLimit()
		{
			_isRandomized = false;
			_randomDerivationLimit = 0;
		}
		
		private void CreatePath()
		{
			if(_pathBuilder == null)
				_pathBuilder = new StringBuilder();
			
			_pathBuilder.Append("<path d=\"M ");
			_pathBuilder.Append(_currentPosition.X);
			_pathBuilder.Append(" ");
			_pathBuilder.Append(_currentPosition.Y);
			_pathBuilder.Append(" ");

			_isPathBuilt = true;
		}
		
		private void ClosePath()
		{
			//TODO: Use @ strings
			_pathBuilder.Append("\" stroke=\"");
			_pathBuilder.Append(_currentPathSegment.FgColor.GetColor(_currentModule));
			_pathBuilder.Append("\" stroke-width=\"");
			_pathBuilder.Append(_currentPathSegment.StrokeWidth.GetCanvasProperty(_currentModule));
			_pathBuilder.Append("\" fill=\"");
			_pathBuilder.Append(_currentPathSegment.BgColor.GetColor(_currentModule));
			_pathBuilder.Append("\" />");

			_isPathBuilt = false;

			_svgBuilder.Append(_pathBuilder.ToString());
			_pathBuilder.Clear();
		}

		private Position CalcPosition(double distance)
		{
			if (_isRandomized)
				distance *= ((_random.NextDouble() % (2*_randomDerivationLimit)) - _randomDerivationLimit) + 1;
			
			double rad =_currentPosition.Angle / 180 * Math.PI;
			double dx = Math.Cos(rad) * distance;
			double dy = Math.Sin(rad) * distance;

			double newX = Math.Round(_currentPosition.X + dx, 2);
			double newY = Math.Round(_currentPosition.Y + dy, 2);

			if (newX < _minX) _minX = newX;
			if (newX > _maxX) _maxX = newX;
			if (newY < _minY) _minY = newY;
			if (newY > _maxY) _maxY = newY;
			return new Position(newX, newY, _currentPosition.Angle);
		}
	}

	public abstract class CanvasProperty<T>
	{
		public abstract T GetCanvasProperty(Module<T> module);

		public abstract bool Equals(CanvasProperty<T> other);
	}

	public sealed class ValueCanvasProperty<T> : CanvasProperty<T>, IEquatable<CanvasProperty<T>>
	{
		private readonly T _value;
		
		public ValueCanvasProperty(T value)
		{
			_value = value;
		}

		public override T GetCanvasProperty(Module<T> module) => _value;


		public override bool Equals(CanvasProperty<T> other)
		{
			if (other is ValueCanvasProperty<T>)
			{
				var otherValue = (ValueCanvasProperty<T>) other;
				return _value.Equals(otherValue._value);
			}

			return false;
		}
	}

	public sealed class IndexCanvasProperty<T> : CanvasProperty<T>, IEquatable<CanvasProperty<T>>
	{
		private readonly int _index;

		public IndexCanvasProperty(int index)
		{
			_index = index;
		}

		public override T GetCanvasProperty(Module<T> module) => module.Parameters[_index];
		public override bool Equals(CanvasProperty<T> other)
		{
			if (other is IndexCanvasProperty<T>)
			{
				var otherIndex = (IndexCanvasProperty<T>) other;
				return _index == otherIndex._index;
			}

			return false;
		}
	}

	public struct Color : IEquatable<Color>
	{
		private readonly CanvasProperty<double> _red;
		private readonly CanvasProperty<double> _green;
		private readonly CanvasProperty<double> _blue;

		private readonly bool _none;

		public Color(CanvasProperty<double> red, CanvasProperty<double> green, CanvasProperty<double> blue)
		{
			_red = red;
			_green = green;
			_blue = blue;
			_none = false;
		}

		public Color(bool tmp) //TODO: Think of some better constructors
		{
			_none = true;
			
			//Place dummy values into _red, _green and _blue
			_red = new ValueCanvasProperty<double>(0);
			_green = new ValueCanvasProperty<double>(0);
			_blue = new ValueCanvasProperty<double>(0);
		}

		public string GetColor(Module<double> module)
		{
			if (_none) return "none";

			int red = (int)_red.GetCanvasProperty(module);
			int green = (int)_green.GetCanvasProperty(module);
			int blue = (int)_blue.GetCanvasProperty(module);

			string redS = red.ToString("X");
			if (redS.Length == 1)
				redS = "0" + redS;
			string greenS = green.ToString("X");
			if (greenS.Length == 1)
				greenS = "0" + greenS;
			string blueS = blue.ToString("X");
			if (blueS.Length == 1)
				blueS = "0" + blueS;
			
			return "#" + redS + greenS + blueS;
		}

		public bool Equals(Color other)
		{
			if (_none && other._none)
				return true;

			if (_none != other._none)
				return false;

			return _red.Equals(other._red) && _green.Equals(other._green) && _blue.Equals(other._blue);
		}
		
		public static Color Black => new Color( new ValueCanvasProperty<double>(0), 
			new ValueCanvasProperty<double>(0), new ValueCanvasProperty<double>(0));
		
		public static Color Red => new Color( new ValueCanvasProperty<double>(255), 
			new ValueCanvasProperty<double>(0), new ValueCanvasProperty<double>(0));
		
		public static Color Green => new Color( new ValueCanvasProperty<double>(0), 
			new ValueCanvasProperty<double>(255), new ValueCanvasProperty<double>(0));
		
		public static Color Blue => new Color( new ValueCanvasProperty<double>(0), 
			new ValueCanvasProperty<double>(0), new ValueCanvasProperty<double>(255));
		
		public static Color White => new Color( new ValueCanvasProperty<double>(255), 
			new ValueCanvasProperty<double>(255), new ValueCanvasProperty<double>(255));
	}

	public class PathSegment : IEquatable<PathSegment>
	{
		public CanvasProperty<double> Length = new ValueCanvasProperty<double>(10);
		public CanvasProperty<double> StrokeWidth = new ValueCanvasProperty<double>(1);
		public Color FgColor = Color.Black;
		public Color BgColor = new Color(true);

		public PathSegment()
		{ }
		
		public PathSegment(CanvasProperty<double> length)
		{
			Length = length;
			StrokeWidth = new ValueCanvasProperty<double>(1);
			FgColor = Color.Black;
			BgColor = new Color(true);
		}

		public override bool Equals(object obj)
		{
			if (obj is PathSegment)
			{
				var otherLine = (PathSegment) obj;
				return Equals(otherLine);
			}

			return false;
		}

		public bool Equals(PathSegment other)
		{
			return Length.Equals(other.Length) && 
			       StrokeWidth.Equals(other.StrokeWidth) &&
			       FgColor.Equals(other.FgColor) && 
			       BgColor.Equals(other.BgColor);
		}

		public static bool operator ==(PathSegment line1, PathSegment line2)
		{
			return line1.Equals(line2);
		}

		public static bool operator !=(PathSegment line1, PathSegment line2)
		{
			return !line1.Equals(line2);
		}
	}

	public class Polygon
	{
		public Color FillColor = new Color(true);
		public Color StrokeColor = Color.Black;
	}

	public class Move
	{
		public CanvasProperty<double> Distance = new ValueCanvasProperty<double>(10);

		public Move() {}
		
		public Move(CanvasProperty<double> distance)
		{
			Distance = distance;
		}
	}

	public class Rotation
	{
		public CanvasProperty<double> Angle = new ValueCanvasProperty<double>(30);

		public Rotation() {}
		
		public Rotation(CanvasProperty<double> angle)
		{
			Angle = angle;
		}
	}

	public struct DrawingAction
	{
		public readonly int ID;
		public readonly Action<Canvas> Handler;

		public DrawingAction(int id, Action<Canvas> handler)
		{
			ID = id;
			Handler = handler ?? throw new ArgumentException("handler cannot be null");
		}
	}
}