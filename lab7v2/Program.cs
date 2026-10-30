using System;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        IRepository<User> repository = new InMemoryRepository<User>();

        repository.Add(new User("oleh", "oleh@example.com", true));
        repository.Add(new User("anna", "anna@example.com", true));
        repository.Add(new User("ivan", "ivan@example.com", false));

        Console.WriteLine(" Усі користувачі після додавання");
        foreach (User user in repository.GetAll())
        {
            Console.WriteLine(user);
        }

        Console.WriteLine();
        Console.WriteLine("Отримання за Id = 2");
        Console.WriteLine(repository.GetById(2));

        Console.WriteLine();
        Console.WriteLine(" Оновлення користувача з Id = 3");
        User updated = new User("ivan", "ivan.new@example.com", true) { Id = 3 };
        repository.Update(updated);
        Console.WriteLine(repository.GetById(3));

        Console.WriteLine();
        Console.WriteLine("Видалення користувача з Id  1");
        repository.Delete(1);
        foreach (User user in repository.GetAll())
        {
            Console.WriteLine(user);
        }

        Console.WriteLine();
        Console.WriteLine(" Пошук неіснуючого Id  99 ");
        Console.WriteLine(repository.GetById(99) == null ? "Користувача не знайдено" : "Знайдено");
    }
}