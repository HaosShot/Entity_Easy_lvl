using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Easy_DB_C_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new DB())
            {
                db.Database.EnsureCreated();
            }

            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("Система управления задачами сотрудников");
                Console.WriteLine("1. Показать всех сотрудников");
                Console.WriteLine("2. Добавить сотрудника");
                Console.WriteLine("3. Показать задачи сотрудника");
                Console.WriteLine("4. Добавить задачу");
                Console.WriteLine("5. Отметить задачу как выполненную");
                Console.WriteLine("6. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    ShowEmployees();
                }
                else if (choice == "2")
                {
                    AddEmployee();
                }
                else if (choice == "3")
                {
                    ShowTasksForEmployee();
                }
                else if (choice == "4")
                {
                    AddTask();
                }
                else if (choice == "5")
                {
                    CompleteTask();
                }
                else if (choice == "6")
                {
                    exitProgram = true;
                }
                else
                {
                    Console.WriteLine("Неизвестная команда!");
                }

                if (!exitProgram)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void ShowEmployees()
        {
            using (var db = new DB())
            {
                List<Employee> employees = new List<Employee>();
                foreach (var emp in db.Employees)
                {
                    employees.Add(emp);
                }

                if (employees.Count == 0)
                {
                    Console.WriteLine("\nСотрудников нет в базе данных!");
                    return;
                }

                Console.WriteLine("\nСписок сотрудников:");
                foreach (Employee emp in employees)
                {
                    Console.WriteLine($"{emp.EmployeeID}. {emp.FullName} - {emp.Position}");
                }
            }
        }

        static void AddEmployee()
        {
            Console.Write("\nВведите имя сотрудника: ");
            string name = Console.ReadLine();

            Console.Write("Введите должность сотрудника: ");
            string position = Console.ReadLine();

            Employee newEmployee = new Employee();
            newEmployee.FullName = name;
            newEmployee.Position = position;

            using (var db = new DB())
            {
                db.Employees.Add(newEmployee);
                db.SaveChanges();
            }

            Console.WriteLine("\nСотрудник успешно добавлен!");
        }

        static void ShowTasksForEmployee()
        {
            Console.Write("\nВведите ID сотрудника: ");
            string input = Console.ReadLine();
            int employeeId;

            if (!int.TryParse(input, out employeeId))
            {
                Console.WriteLine("Неправильный ID сотрудника!");
                return;
            }

            using (var db = new DB())
            {
                Employee employee = null;
                foreach (var emp in db.Employees)
                {
                    if (emp.EmployeeID == employeeId)
                    {
                        employee = emp;
                        break;
                    }
                }

                if (employee == null)
                {
                    Console.WriteLine("Сотрудник не найден!");
                    return;
                }

                List<Task> tasks = new List<Task>();
                foreach (var task in db.Tasks)
                {
                    if (task.EmployeeID == employeeId)
                    {
                        tasks.Add(task);
                    }
                }

                Console.WriteLine($"\nЗадачи сотрудника {employee.FullName}:");

                if (tasks.Count == 0)
                {
                    Console.WriteLine("Задач нет!");
                    return;
                }

                foreach (Task task in tasks)
                {
                    Console.WriteLine($"ID задачи: {task.TaskID}");
                    Console.WriteLine($"Описание: {task.Description}");
                    Console.WriteLine($"Статус: {(task.IsCompleted ? "Выполнена" : "В работе")}");

                    if (task.Deadline.HasValue)
                    {
                        Console.WriteLine($"Дедлайн: {task.Deadline.Value.ToShortDateString()}");
                    }
                    else
                    {
                        Console.WriteLine("Дедлайн: не установлен");
                    }

                    Console.WriteLine($"Дата создания: {task.CreatedDate.ToShortDateString()}");
                    Console.WriteLine();
                }
            }
        }

        static void AddTask()
        {
            Console.Write("\nВведите ID сотрудника для задачи: ");
            string input = Console.ReadLine();
            int employeeId;

            if (!int.TryParse(input, out employeeId))
            {
                Console.WriteLine("Неправильный ID сотрудника!");
                return;
            }

            using (var db = new DB())
            {
                bool employeeExists = false;
                foreach (var emp in db.Employees)
                {
                    if (emp.EmployeeID == employeeId)
                    {
                        employeeExists = true;
                        break;
                    }
                }

                if (!employeeExists)
                {
                    Console.WriteLine("Сотрудник не найден!");
                    return;
                }

                Console.Write("Введите описание задачи: ");
                string description = Console.ReadLine();

                Console.Write("Установить срок выполнения? (y/n): ");
                string setDeadline = Console.ReadLine();

                DateTime? deadline = null;
                if (setDeadline.ToLower() == "y")
                {
                    Console.Write("Введите срок (гггг-мм-дд): ");
                    string dateInput = Console.ReadLine();

                    try
                    {
                        deadline = DateTime.Parse(dateInput);
                    }
                    catch
                    {
                        Console.WriteLine("Неправильный формат даты! Срок не установлен.");
                    }
                }

                Task newTask = new Task();
                newTask.EmployeeID = employeeId;
                newTask.Description = description;
                newTask.Deadline = deadline;
                newTask.IsCompleted = false;
                newTask.CreatedDate = DateTime.Now;

                db.Tasks.Add(newTask);
                db.SaveChanges();

                Console.WriteLine("\nЗадача успешно добавлена!");
            }
        }

        static void CompleteTask()
        {
            Console.Write("\nВведите ID задачи: ");
            string input = Console.ReadLine();
            int taskId;

            if (!int.TryParse(input, out taskId))
            {
                Console.WriteLine("Неправильный ID задачи!");
                return;
            }

            using (var db = new DB())
            {
                Task taskToComplete = null;
                foreach (var task in db.Tasks)
                {
                    if (task.TaskID == taskId)
                    {
                        taskToComplete = task;
                        break;
                    }
                }

                if (taskToComplete == null)
                {
                    Console.WriteLine("Задача не найдена!");
                    return;
                }

                taskToComplete.IsCompleted = true;
                db.SaveChanges();

                Console.WriteLine("Задача отмечена как выполненная!");
            }
        }
    }
}