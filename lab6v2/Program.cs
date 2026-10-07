using System;
using System.Collections.Generic;
using System.Text;

class Shape
{
    private string _color;

    public string Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public Shape(string color)
    {
        _color = color;
    }

    public virtual double GetArea()
    {
        return 0;
    }

    public string GetShapeType()
    {
        return "Фігура";
    }
}

class Circle : Shape
{
    private double _radius;

    public double Radius
    {
        get { return _radius; }
        set { _radius = value; }
    }

    public Circle(string color, double radius) : base(color)
    {
        _radius = radius;
    }

    public override double GetArea()
    {
        return Math.PI * _radius * _radius;
    }

    public void Draw()
    {
        Console.WriteLine($"Малюємо коло кольору {Color} з радіусом {Radius}");
    }
}

class Rectangle : Shape
{
    private double _width;
    private double _height;

    public double Width
    {
        get { return _width; }
        set { _width = value; }
    }

    public double Height
    {
        get { return _height; }
        set { _height = value; }
    }

    public Rectangle(string color, double width, double height) : base(color)
    {
        _width = width;
        _height = height;
    }

    public override double GetArea()
    {
        return _width * _height;
    }

    public void Draw()
    {
        Console.WriteLine($"Малюємо прямокутник кольору {Color} зі сторонами {Width} x {Height}");
    }

    public new string GetShapeType()
    {
        return "Прямокутник";
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Shape shape = new Shape("сірий");
        Circle circle = new Circle("червоний", 5);
        Rectangle rectangle = new Rectangle("синій", 4, 6);

        Console.WriteLine("Власні методи похідних класів");
        circle.Draw();
        rectangle.Draw();

        Console.WriteLine();
        Console.WriteLine("=== Поліморфізм: override через посилання на базовий клас ===");
        List<Shape> shapes = new List<Shape> { shape, circle, rectangle };
        foreach (Shape s in shapes)
        {
            Console.WriteLine($"{s.GetType().Name}: площа = {s.GetArea():F2}");
        }

        Console.WriteLine();
        Console.WriteLine("Різниця між override та new");
        Shape shapeRef = rectangle;
        Rectangle rectangleRef = rectangle;

        Console.WriteLine($"Shape shapeRef = rectangle; shapeRef.GetArea() = {shapeRef.GetArea():F2}");
        Console.WriteLine($"Rectangle rectangleRef = rectangle; rectangleRef.GetArea() = {rectangleRef.GetArea():F2}");
        Console.WriteLine($"shapeRef.GetShapeType() = {shapeRef.GetShapeType()}");
        Console.WriteLine($"rectangleRef.GetShapeType() = {rectangleRef.GetShapeType()}");
    }
}