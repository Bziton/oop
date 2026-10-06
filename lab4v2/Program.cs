using System.Text;

namespace lab4v2;

public class Shape
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

public class Circle : Shape
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
        Console.WriteLine($"Малюємо коло: колір = {Color}, радіус = {Radius}");
    }
}

public class Rectangle : Shape
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
        Console.WriteLine($"Малюємо прямокутник: колір = {Color}, ширина = {Width}, висота = {Height}");
    }

    public new string GetShapeType()
    {
        return "Прямокутник";
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Shape shape = new Shape("Сірий");
        Circle circle = new Circle("Червоний", 5);
        Rectangle rectangle = new Rectangle("Синій", 4, 6);

        Console.WriteLine("=== Об'єкти базового та похідних класів ===");
        Console.WriteLine($"Shape: площа = {shape.GetArea():F2}");
        Console.WriteLine($"Circle: площа = {circle.GetArea():F2}");
        Console.WriteLine($"Rectangle: площа = {rectangle.GetArea():F2}");
        Console.WriteLine();

        Console.WriteLine("=== Власні методи похідних класів ===");
        circle.Draw();
        rectangle.Draw();
        Console.WriteLine();

        Console.WriteLine("=== Поліморфізм (virtual / override) ===");
        Shape[] shapes = { shape, circle, rectangle };
        foreach (Shape item in shapes)
        {
            Console.WriteLine($"{item.GetType().Name,-10} колір = {item.Color,-9} площа = {item.GetArea():F2}");
        }
        Console.WriteLine();

        Console.WriteLine("=== Приховування (new) ===");
        Shape shapeRef = rectangle;
        Rectangle rectangleRef = rectangle;
        Console.WriteLine($"Через посилання типу Shape:     {shapeRef.GetShapeType()}");
        Console.WriteLine($"Через посилання типу Rectangle: {rectangleRef.GetShapeType()}");
        Console.WriteLine();

        Console.WriteLine("=== Порівняння override та new ===");
        Console.WriteLine($"GetArea (override) через Shape:     {shapeRef.GetArea():F2}");
        Console.WriteLine($"GetArea (override) через Rectangle: {rectangleRef.GetArea():F2}");
        Console.WriteLine($"GetShapeType (new) через Shape:     {shapeRef.GetShapeType()}");
        Console.WriteLine($"GetShapeType (new) через Rectangle: {rectangleRef.GetShapeType()}");
    }
}