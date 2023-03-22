using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BrugerProgram
{
    internal class Program
    {
        public static string path = @"C:\kundedata";
        static void Main(string[] args)
        {
            menu();

        }

        public static void menu()
        {
            Console.Clear();
            Console.WriteLine("Importer kunder");
            Console.WriteLine("Opret ny kunde");
            Console.WriteLine("Søg kunde");
            Console.WriteLine("Udskriv kunder");
            Console.WriteLine("Afslut");

            ConsoleKey key = Console.ReadKey(true).Key;
            List<Kunde> kunder = GetCustomersFromJsonFile();
            switch (key)
            {
                case ConsoleKey.I:
                    importCustomers();
                    break;
                case ConsoleKey.O:
                    createUser();
                    break;
                case ConsoleKey.S:
                    Console.Write("Indtast det du søger efter: ");
                    string search = Console.ReadLine();
                    DisplayCustomers(kunder, 20, search);
                    break;
                case ConsoleKey.U:
                    DisplayCustomers(kunder, 20);
                    break;
                case ConsoleKey.A:
                    ConsoleKey k;
                    do
                    {
                        Console.WriteLine("Ønsker du at afslutte programmet tryk? (j/n)");
                        k = Console.ReadKey(true).Key;
                    }while(k != ConsoleKey.J && k != ConsoleKey.N);
                    if (k == ConsoleKey.J) Environment.Exit(0);
                    if (k == ConsoleKey.N) menu();

                    break;
                default:
                    menu();
                    break;
            }


            Console.Read();
        }
        public static void createUser()
        {
            List <Kunde> kunder = GetCustomersFromJsonFile();
            string fullname, email, postalcode, city, streetName;
            int tlfnr;
            Console.WriteLine("Oprettelse af kunde:");
            Console.Write("Indtast fulde navn: ");
            fullname = Console.ReadLine();
            do
            {
                Console.Write("Indtast email: ");
                email = Console.ReadLine();
            } while (CheckEmailExists(email));
            do
            {
                Console.Write("Indtast tlf nr: ");
                tlfnr = int.Parse(Console.ReadLine());
            } while (CheckPhoneNumberExist(tlfnr));
            Console.Write("Indtast postnummer: ");
            postalcode = Console.ReadLine();

            Console.Write("Indtast by: ");
            city = Console.ReadLine();

            Console.Write("Indtast vejnavn: ");
            streetName = Console.ReadLine();



            //kunder.Add(new Kunde(fullname, email, postalcode, city, streetName));
            var test = JsonConvert.SerializeObject(kunder, Formatting.Indented);
            File.WriteAllText(path + @"\kundedata.json", test);
            Console.WriteLine("Kunde oprettet! Tryk en tast for at komme tilbage!");
            Console.ReadKey();
            menu();



        }
        public static void importCustomers()
        {

            Random random = new Random();

            Console.Write("Indtast hvor mange kunder du vil importere: ");
            int amount = int.Parse(Console.ReadLine());

            string[] drengenavne = File.ReadAllLines(path + @"\drengenavne.dat");
            string[] pigenavne = File.ReadAllLines(path + @"\pigenavne.dat");
            string[] fornavne = new string[drengenavne.Length + pigenavne.Length];
            drengenavne.CopyTo(fornavne, 0);
            pigenavne.CopyTo(fornavne, drengenavne.Length);

            string[] efternavne = File.ReadAllLines(path + @"\efternavne.dat");
            string[] emails = File.ReadAllLines(path + @"\email.dat");
            string[] postby = File.ReadAllLines(path + @"\postby.dat");
            string[] vejnavne = File.ReadAllLines(path + @"\vejnavne.dat");

            string randomNavn, randomEfternavn, randomEmail, randomPostBy, randomVejnavn;
            int randomTlfnr; 
            List<Kunde> list = GetCustomersFromJsonFile();
            for (int i = 0; i < amount; i++)
            {
                randomNavn = fornavne[random.Next(0, fornavne.Length)];
                randomEfternavn = efternavne[random.Next(0, efternavne.Length)];
                randomPostBy = postby[random.Next(0, postby.Length)];
                randomVejnavn = vejnavne[random.Next(0, vejnavne.Length)];
                string[] cityAndPostalcode = randomPostBy.Split(";");

                do
                {
                    randomTlfnr = random.Next(00000000, 99999999);
                } while (CheckPhoneNumberExist(randomTlfnr));

                do
                {
                    randomEmail = randomNavn + "." + randomEfternavn + "@" + emails[random.Next(0, emails.Length)];
                } while (CheckEmailExists(randomEmail));


                Console.WriteLine(randomNavn + " " + randomEfternavn + " " + randomEmail + "" + randomPostBy + " " + randomVejnavn);
                list.Add(new Kunde(randomNavn + " "+ randomEfternavn, randomEmail, randomTlfnr, cityAndPostalcode[0], cityAndPostalcode[1], randomVejnavn));
                var test = JsonConvert.SerializeObject(list, Formatting.Indented);
                File.WriteAllText(path + @"\kundedata.json", test);
            }

            Console.WriteLine($"\nDer er blevet orettet: {amount} kunder");
            Console.WriteLine("Tryk en tast for at komme tilbage til start");
            Console.ReadKey();
            menu();




        }

        public static List<Kunde> GetCustomersFromJsonFile()
        {
            string json = File.ReadAllText(path + @"\kundedata.json");
            List<Kunde> customers = JsonConvert.DeserializeObject<List<Kunde>>(json);
            return customers;
        }
        private static void DisplayCustomers(List<Kunde> customers, int pageSize, string search = "")
        {
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling(customers.Count / (double)pageSize);
            while (true)
            {

                //Console.Clear();
                //Console.WriteLine($"Side {currentPage + 1}/{totalPages}");
                //Console.WriteLine(new string('-', 40));

                IEnumerable<Kunde> displayedCustomers = customers;

                if (!string.IsNullOrEmpty(search))
                {
                    displayedCustomers = displayedCustomers
                        .Where(customer =>
                            customer.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            customer.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            customer.PhoneNumber.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            customer.City.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            customer.PostalCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            customer.StreetName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    totalPages = (int)Math.Ceiling(displayedCustomers.Count() / (double)pageSize);
                }

                Console.Clear();
                Console.WriteLine($"Side {currentPage + 1}/{totalPages}");
                Console.WriteLine(new string('-', 40));

                for (int i = currentPage * pageSize; i < (currentPage + 1) * pageSize && i < displayedCustomers.Count(); i++)
                {
                    Kunde customer = displayedCustomers.ElementAt(i);
                    Console.WriteLine($"Navn: {customer.Name}");
                    Console.WriteLine($"E-mail: {customer.Email}");
                    Console.WriteLine($"Tlf. nr: {customer.PhoneNumber}");
                    Console.WriteLine($"Adresse: {customer.StreetName} {customer.City} {customer.PostalCode}");
                    Console.WriteLine(new string('-', 40));
                }

                Console.WriteLine("Brug piltasterne for at navigere, eller tryk på en anden tast for at afslutte.");
                Console.WriteLine("Søg efter kunde: " + search);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.RightArrow && currentPage < totalPages - 1)
                {
                    currentPage++;
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow && currentPage > 0)
                {
                    currentPage--;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && !string.IsNullOrEmpty(search))
                {
                    search = search.Substring(0, search.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    search += keyInfo.KeyChar;
                }
                else
                {
                    menu();
                    break;

                }
            }
        }
        public static bool CheckEmailExists(string email)
        {
            string path = @"C:\kundedata\kundedata.json";
            List<Kunde> kunder = JsonConvert.DeserializeObject<List<Kunde>>(File.ReadAllText(path));

            foreach (Kunde k in kunder)
            {
                if (k.Email == email)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckPhoneNumberExist(int number)
        {
            string path = @"C:\kundedata\kundedata.json";
            List<Kunde> kunder = JsonConvert.DeserializeObject<List<Kunde>>(File.ReadAllText(path));
            if (number.ToString().Substring(0, 1) == "0") return true;
            if (number.ToString().Length != 8) return true;
            foreach (Kunde k in kunder)
            {
                if (k.PhoneNumber == number)
                {
                    return true;
                }
            }
            return false;
        }
    }

}