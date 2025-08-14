// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthSystemQ2
{
    // a) Generic repository
    public class Repository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);

        public List<T> GetAll() => _items;

        public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item != null)
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // b) Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }

    // c) Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"Prescription ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:d}";
    }

    // g) HealthSystemApp
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // Seed sample data
        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Charlie Brown", 25, "Male"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 2, "Ibuprofen", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(103, 1, "Vitamin C", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Paracetamol", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(105, 2, "Cough Syrup", DateTime.Today));
        }

        // Build dictionary mapping patient ID -> prescriptions
        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var pres in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(pres.PatientId))
                    _prescriptionMap[pres.PatientId] = new List<Prescription>();

                _prescriptionMap[pres.PatientId].Add(pres);
            }
        }

        // Print all patients
        public void PrintAllPatients()
        {
            Console.WriteLine("=== Patients ===");
            foreach (var p in _patientRepo.GetAll())
            {
                Console.WriteLine(p);
            }
        }

        // Print prescriptions for a specific patient
        public void PrintPrescriptionsForPatient(int patientId)
        {
            Console.WriteLine($"\nPrescriptions for Patient ID: {patientId}");
            if (_prescriptionMap.ContainsKey(patientId))
            {
                foreach (var pres in _prescriptionMap[patientId])
                    Console.WriteLine(pres);
            }
            else
            {
                Console.WriteLine("No prescriptions found.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();

            // Seed data
            app.SeedData();

            // Build prescription dictionary
            app.BuildPrescriptionMap();

            // Display patients
            app.PrintAllPatients();

            // Show prescriptions for patient with ID 1
            app.PrintPrescriptionsForPatient(1);

            // Show prescriptions for patient with ID 2
            app.PrintPrescriptionsForPatient(2);

            // Show prescriptions for patient with ID 3
            app.PrintPrescriptionsForPatient(3);
        }
    }
}

