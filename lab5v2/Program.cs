using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Vehicle
{
    public double Speed { get; set; }

    public Vehicle(double speed)
    {
        Speed = speed;
    }

    public virtual void Move()
    {
        Console.WriteLine($"Транспортний засіб рухається зі швидкістю {Speed} км/год");
    }
}

class Car : Vehicle
{
    public int NumWheels { get; set; }

    public Car(double speed, int numWheels) : base(speed)
    {
        NumWheels = numWheels;
    }

    public override void Move()
    {
        Console.WriteLine($"Автомобіль їде по дорозі на {NumWheels} колесах зі швидкістю {Speed} км/год");
    }
}

class Bicycle : Vehicle
{
    public bool HasGears { get; set; }

    public Bicycle(double speed, bool hasGears) : base(speed)
    {
        HasGears = hasGears;
    }

    public override void Move()
    {
        string gears = HasGears ? "з перемиканням передач" : "без перемикання передач";
        Console.WriteLine($"Велосипед {gears} їде зі швидкістю {Speed} км/год");
    }
}

class Boat : Vehicle
{
    public string EngineType { get; set; }

    public Boat(double speed, string engineType) : base(speed)
    {
        EngineType = engineType;
    }

    public override void Move()
    {
        Console.WriteLine($"Човен з двигуном \"{EngineType}\" пливе по воді зі швидкістю {Speed} км/год");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<Vehicle> vehicles = new List<Vehicle>
        {
            new Car(120, 4),
            new Bicycle(25, true),
            new Boat(40, "підвісний мотор")
        };

        Console.WriteLine(" Поліморфний виклик Move()");
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.Move();
        }

        double averageSpeed = vehicles.Average(v => v.Speed);

        Console.WriteLine();
        Console.WriteLine("Агрегація");
        Console.WriteLine($"Кількість транспортних засобів: {vehicles.Count}");
        Console.WriteLine($"Середня швидкість: {averageSpeed:F2} км/год");
    }
}