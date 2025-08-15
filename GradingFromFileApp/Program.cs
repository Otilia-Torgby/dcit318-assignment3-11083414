using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id;
    public string FullName = "";
    public int Score;

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }

    public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
}


public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg) : base(msg) { } }
public class MissingFieldException : Exception { public MissingFieldException(string msg) : base(msg) { } }

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var list = new List<Student>();
        using var reader = new StreamReader(inputFilePath);
        string? line;
        int lineNo = 0;

        while ((line = reader.ReadLine()) != null)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 3)
                throw new MissingFieldException($"Line {lineNo}: expected 3 fields (Id, FullName, Score).");

            if (!int.TryParse(parts[0].Trim(), out int id))
                throw new InvalidScoreFormatException($"Line {lineNo}: invalid Id format.");

            string fullName = parts[1].Trim();

            if (!int.TryParse(parts[2].Trim(), out int score))
                throw new InvalidScoreFormatException($"Line {lineNo}: score is not an integer.");

            list.Add(new Student { Id = id, FullName = fullName, Score = score });
        }
        return list;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        foreach (var s in students)
            writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
    }
}

public class Program
{
    public static void Main()
    {
        try
        {
            string input = "students_input.txt";
            string output = "students_report.txt";

           
            if (!File.Exists(input))
            {
                Console.WriteLine("Input file not found. Creating sample file...");
                File.WriteAllLines(input, new[]
                {
                    "101, Alice Smith, 84",
                    "102, James Tetteh, 69",
                    "103, Abena Sekyi, 55"
                });
            }

            var proc = new StudentResultProcessor();
            var students = proc.ReadStudentsFromFile(input);
            proc.WriteReportToFile(students, output);

            Console.WriteLine("Report written to " + output);
        }
        catch (FileNotFoundException ex) { Console.WriteLine("File not found: " + ex.Message); }
        catch (InvalidScoreFormatException ex) { Console.WriteLine("Invalid score/Id: " + ex.Message); }
        catch (MissingFieldException ex) { Console.WriteLine("Missing field: " + ex.Message); }
        catch (Exception ex) { Console.WriteLine("Unexpected error: " + ex.Message); }
    }
}
