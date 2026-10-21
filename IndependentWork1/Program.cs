using System;
using System.Text;

class BankAccount
{
    private string _owner;
    private decimal _balance;

    public string Owner
    {
        get { return _owner; }
        set { _owner = value; }
    }

    public decimal Balance
    {
        get { return _balance; }
    }

    public BankAccount(string owner, decimal balance)
    {
        _owner = owner;
        _balance = balance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Сума поповнення має бути більшою за 0");
            return;
        }
        _balance += amount;
        Console.WriteLine($"Поповнено на {amount} грн. Баланс: {_balance} грн");
    }

    public bool Withdraw(decimal amount)
    {
        if (amount > _balance)
        {
            Console.WriteLine($"Недостатньо коштів для зняття {amount} грн. Баланс: {_balance} грн");
            return false;
        }
        _balance -= amount;
        Console.WriteLine($"Знято {amount} грн. Баланс: {_balance} грн");
        return true;
    }
}

class Movie
{
    private string _title;
    private int _duration;
    private double _rating;

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int Duration
    {
        get { return _duration; }
    }

    public double Rating
    {
        get { return _rating; }
        set { _rating = value; }
    }

    public Movie(string title, int duration, double rating)
    {
        _title = title;
        _duration = duration;
        _rating = rating;
    }

    public bool IsLong()
    {
        return _duration > 150;
    }

    public void PrintInfo()
    {
        int hours = _duration / 60;
        int minutes = _duration % 60;
        Console.WriteLine($"Фільм: {_title}, тривалість: {hours} год {minutes} хв, рейтинг: {_rating}");
    }
}

class Pizza
{
    private string _name;
    private int _size;
    private int _toppings;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int Size
    {
        get { return _size; }
    }

    public Pizza(string name, int size, int toppings)
    {
        _name = name;
        _size = size;
        _toppings = toppings;
    }

    public double CalculatePrice()
    {
        double basePrice = _size * 6.5;
        return basePrice + _toppings * 25;
    }

    public void AddTopping()
    {
        _toppings++;
        Console.WriteLine($"До піци '{_name}' додано топінг. Тепер топінгів: {_toppings}");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== BankAccount ===");
        BankAccount account = new BankAccount("Олександр", 1500);
        Console.WriteLine($"Власник: {account.Owner}, початковий баланс: {account.Balance} грн");
        account.Deposit(500);
        account.Withdraw(3000);
        account.Withdraw(700);
        Console.WriteLine();

        Console.WriteLine("=== Movie ===");
        Movie movie = new Movie("Інтерстеллар", 169, 8.7);
        movie.PrintInfo();
        Console.WriteLine($"Довгий фільм (більше 150 хв): {(movie.IsLong() ? "так" : "ні")}");
        movie.Rating = 9.0;
        Console.WriteLine($"Новий рейтинг: {movie.Rating}");
        Console.WriteLine();

        Console.WriteLine("=== Pizza ===");
        Pizza pizza = new Pizza("Пепероні", 30, 2);
        Console.WriteLine($"Піца '{pizza.Name}', {pizza.Size} см, ціна: {pizza.CalculatePrice():F2} грн");
        pizza.AddTopping();
        Console.WriteLine($"Нова ціна: {pizza.CalculatePrice():F2} грн");
    }
}
