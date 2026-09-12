using System;
using System.Collections.Generic;
using System.IO;

namespace GradingSystem
{
    // a. Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100)
                return "A";
            else if (Score >= 70 && Score <= 79)
                return "B";
            else if (Score >= 60 && Score <= 69)
                return "C";
            else if (Score >= 50 && Score <= 59)
                return "D";
            else
                return "F";
        }
    }

    // b. Custom exception: invalid score format
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message)
        {
        }
    }

    // c. Custom exception: missing field
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message)
        {
        }
    }

    // d. StudentResultProcessor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var fields = line.Split(',');

                    if (fields.Length < 3)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: Expected 3 fields (ID, Name, Score) but found {fields.Length}.");
                    }

                    string idText = fields[0].Trim();
                    string fullName = fields[1].Trim();
                    string scoreText = fields[2].Trim();

                    if (!int.TryParse(idText, out int id))
                    {
                        throw new MissingFieldException($"Line {lineNumber}: Student ID '{idText}' is invalid or missing.");
                    }

                    if (!int.TryParse(scoreText, out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: Score '{scoreText}' could not be converted to an integer.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== School Grading System ===\n");

            string inputFilePath = "students.txt";
            string outputFilePath = "grade_report.txt";

            var processor = new StudentResultProcessor();

            try
            {
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine($"Successfully processed {students.Count} student(s).");
                Console.WriteLine($"Report written to: {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: Input file not found. {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: Invalid score format. {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: Missing field. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
