using System.Diagnostics;

var courses = new List<Course>
{
    new(1, "Programming Basics", 5),
    new(2, "Databases", 4),
    new(3, "Web Development", 6),
    new(4, "Computer Networks", 3)
};

var students = new List<Student>
{
    new(1,  "Andrii Kovalenko", 19, 1),
    new(2,  "Olena Shevchenko", 18, 1),
    new(3,  "Dmytro Bondar",    21, 1),
    new(4,  "Maria Tkachenko",  20, 1),
    new(5,  "Ivan Melnyk",      17, 1),
    new(6,  "Sofiia Kravets",   22, 2),
    new(7,  "Bohdan Lysenko",   19, 2),
    new(8,  "Anna Polishchuk",  20, 2),
    new(9,  "Yurii Savchenko",  21, 2),
    new(10, "Kateryna Moroz",   18, 3),
    new(11, "Oleksandr Rudenko",19, 3),
    new(12, "Nazar Hrytsenko",  23, 3),
    new(13, "Daryna Petrenko",  20, 4),
    new(14, "Taras Yaroshenko", 19, 4),
    new(15, "Vira Honchar",     22, 4)
};

Console.WriteLine("=== Лабораторна №10, варіант 2: студенти та курси ===\n");

Console.WriteLine("--- Запит 1: студенти до 20 років на курсі \"Programming Basics\" ---");

var (q1Query, q1QueryFirst, q1QueryAvg) = Measure(() =>
    (from s in students
     join c in courses on s.CourseId equals c.Id
     where c.Title == "Programming Basics" && s.Age < 20
     select s).ToList());

var (q1Method, q1MethodFirst, q1MethodAvg) = Measure(() =>
    students
        .Join(courses.Where(c => c.Title == "Programming Basics"),
              s => s.CourseId,
              c => c.Id,
              (s, c) => s)
        .Where(s => s.Age < 20)
        .ToList());

Console.WriteLine("Результат (синтаксис запитів):");
foreach (var s in q1Query) Console.WriteLine($"  {s.Name}, {s.Age} р.");
Console.WriteLine($"Збігається з синтаксисом методів: {q1Query.SequenceEqual(q1Method)}");
PrintTime("Синтаксис запитів", q1QueryFirst, q1QueryAvg);
PrintTime("Синтаксис методів", q1MethodFirst, q1MethodAvg);

Console.WriteLine("\n--- Запит 2: студенти разом з курсами (join) ---");

var (q2Query, q2QueryFirst, q2QueryAvg) = Measure(() =>
    (from s in students
     join c in courses on s.CourseId equals c.Id
     select new { StudentName = s.Name, CourseTitle = c.Title }).ToList());

var (q2Method, q2MethodFirst, q2MethodAvg) = Measure(() =>
    students
        .Join(courses,
              s => s.CourseId,
              c => c.Id,
              (s, c) => new { StudentName = s.Name, CourseTitle = c.Title })
        .ToList());

Console.WriteLine("Результат (синтаксис запитів):");
foreach (var item in q2Query) Console.WriteLine($"  {item.StudentName} -> {item.CourseTitle}");
Console.WriteLine($"Кількість рядків: запитів = {q2Query.Count}, методів = {q2Method.Count}");
PrintTime("Синтаксис запитів", q2QueryFirst, q2QueryAvg);
PrintTime("Синтаксис методів", q2MethodFirst, q2MethodAvg);

Console.WriteLine("\n--- Запит 3: кількість студентів на кожному курсі ---");

var (q3Query, q3QueryFirst, q3QueryAvg) = Measure(() =>
    (from c in courses
     join s in students on c.Id equals s.CourseId into enrolled
     select new { c.Title, Count = enrolled.Count() }).ToList());

var (q3Method, q3MethodFirst, q3MethodAvg) = Measure(() =>
    courses
        .GroupJoin(students,
                   c => c.Id,
                   s => s.CourseId,
                   (c, enrolled) => new { c.Title, Count = enrolled.Count() })
        .ToList());

Console.WriteLine("Результат (синтаксис запитів):");
foreach (var item in q3Query) Console.WriteLine($"  {item.Title}: {item.Count}");
Console.WriteLine($"Кількість груп: запитів = {q3Query.Count}, методів = {q3Method.Count}");
PrintTime("Синтаксис запитів", q3QueryFirst, q3QueryAvg);
PrintTime("Синтаксис методів", q3MethodFirst, q3MethodAvg);

Console.WriteLine("\n--- Запит 4: наймолодший студент ---");

var (q4Query, q4QueryFirst, q4QueryAvg) = Measure(() =>
    (from s in students
     orderby s.Age
     select s).First());

var (q4Method, q4MethodFirst, q4MethodAvg) = Measure(() =>
    students.MinBy(s => s.Age)!);

Console.WriteLine($"Результат (синтаксис запитів): {q4Query.Name}, {q4Query.Age} р.");
Console.WriteLine($"Результат (синтаксис методів): {q4Method.Name}, {q4Method.Age} р.");
PrintTime("Синтаксис запитів (OrderBy + First)", q4QueryFirst, q4QueryAvg);
PrintTime("Синтаксис методів (MinBy)", q4MethodFirst, q4MethodAvg);

static (T Result, double FirstMs, double AvgMs) Measure<T>(Func<T> query, int repeats = 1000)
{
    var sw = Stopwatch.StartNew();
    T result = query();
    sw.Stop();
    double firstMs = sw.Elapsed.TotalMilliseconds;

    sw.Restart();
    for (int i = 0; i < repeats; i++)
    {
        query();
    }
    sw.Stop();
    double avgMs = sw.Elapsed.TotalMilliseconds / repeats;

    return (result, firstMs, avgMs);
}

static void PrintTime(string label, double firstMs, double avgMs)
{
    Console.WriteLine($"  {label}: перший запуск = {firstMs:F4} мс, середній = {avgMs:F5} мс");
}

class Student
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public int CourseId { get; }

    public Student(int id, string name, int age, int courseId)
    {
        Id = id;
        Name = name;
        Age = age;
        CourseId = courseId;
    }
}

class Course
{
    public int Id { get; }
    public string Title { get; }
    public int Credits { get; }

    public Course(int id, string title, int credits)
    {
        Id = id;
        Title = title;
        Credits = credits;
    }
}