using System;
using System.Collections.Generic;
using System.Linq;


public class Repository<T>
{
    private readonly List<T> items = new();
    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var obj = items.FirstOrDefault(predicate);
        return obj != null && items.Remove(obj);
    }
}


public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }

    public override string ToString() => $"{Id}: {Name}, {Age}, {Gender}";
}


public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string med, DateTime dateIssued)
    {
        Id = id; PatientId = patientId; MedicationName = med; DateIssued = dateIssued;
    }

    public override string ToString() => $"Rx#{Id} for Patient {PatientId}: {MedicationName} on {DateIssued:d}";
}


public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Ama Mensah", 28, "F"));
        _patientRepo.Add(new Patient(2, "Kojo Owusu", 41, "M"));
        _patientRepo.Add(new Patient(3, "Abena Serwaa", 35, "F"));

        _prescriptionRepo.Add(new Prescription(100, 1, "Amoxicillin", DateTime.Today.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(101, 1, "Ibuprofen", DateTime.Today));
        _prescriptionRepo.Add(new Prescription(102, 2, "Metformin", DateTime.Today.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(103, 3, "Loratadine", DateTime.Today.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(104, 2, "Atorvastatin", DateTime.Today));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo
            .GetAll()
            .GroupBy(r => r.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        foreach (var p in _patientRepo.GetAll())
            Console.WriteLine(p);
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId) =>
        _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();

    public void PrintPrescriptionsForPatient(int id)
    {
        var list = GetPrescriptionsByPatientId(id);
        if (list.Count == 0) Console.WriteLine("No prescriptions found.");
        foreach (var rx in list) Console.WriteLine(rx);
    }

    public static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        Console.WriteLine("\nPrescriptions for PatientId 2:");
        app.PrintPrescriptionsForPatient(2);
    }
}
