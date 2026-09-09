using System;

namespace Lab2
{
    public class Car
    {
        private string _brand;
        private string _model;
        private int _year;

        public string Brand
        {
            get => _brand;
            set => _brand = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public string Model
        {
            get => _model;
            set => _model = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public int Year
        {
            get => _year;
            set
            {
                if (value > DateTime.Now.Year)
                {
                    _year = 2000;
                }
                else
                {
                    _year = value;
                }
            }
        }

        public Car() : this("Unknown", "Unknown", 2000)
        {
        }

        public Car(string brand, string model, int year)
        {
            Brand = brand;
            Model = model;
            Year = year;
        }

        public void StartEngine()
        {
            Console.WriteLine($"Двигун автомобіля {Brand} {Model} ({Year} р.) успішно запущено!");
        }

        ~Car()
        {
            Console.WriteLine($"Об'єкт {Brand} {Model} знищено.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Car car1 = new Car();
            car1.StartEngine();

            Car car2 = new Car("Toyota", "Camry", 2021);
            car2.StartEngine();

            Car car3 = new Car("Tesla", "Cybertruck", 2035);
            car3.StartEngine();

            car1 = null;
            car2 = null;
            car3 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}