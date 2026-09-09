using System;

namespace Lab1
{
    class Car
    {
        private string _brand;
        private string _model;
        private int _year;

        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                if (value >= 1886 && value <= 2026)
                {
                    _year = value;
                }
            }
        }

        public Car(string brand, string model, int year)
        {
            _brand = brand;
            _model = model;
            _year = year;
        }

        public void Drive()
        {
            Console.WriteLine($"Автомобіль {_brand} {_model} ({_year} року) вирушив у дорогу.");
        }
    }

    class Cars
    {
        static void Main(string[] args)
        {
            Car car1 = new Car("BMW", "M5", 2021);
            car1.Drive();

            Car car2 = new Car("Audi", "RS6", 2022);
            car2.Drive();
        }
    }
}