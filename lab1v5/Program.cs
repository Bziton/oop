using System; 

namespace Lab1
{
    class Student 
    {
        private string _name;
        private double _grades; 

        public string Name 
        {
            get { return _name; }
            set { _name = value; }
        }

        public double AvarageGrade
        {
            get { return _grades; }
            set
            {
                if (value >= 2 && value <= 12)
                {
                    _grades = value;
                }
            }
        }

        public Student(string name, double grade)
        {
            _name = name;
            _grades = grade; 
        }
        
        public void Checking() 
        {
            if (_grades >= 10)
            {
                Console.WriteLine($"Студент {_name} отримує підвищену стипендію.");
            }
            else
            {
                Console.WriteLine($"Студенту {_name} потрібно підтягнути оцінки.");
            }
        }
    }

    class Students 
    {
       static void Main(string[] args)
       {
            Student student1 = new Student("Коля", 11);
            student1.Checking();

            Student student2 = new Student("Даня", 8);
            student2.Checking();
       }
    }
}
