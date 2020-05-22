using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Group_project
{
    public interface ISummerizable // interface defining contract for returning summary as string
    {
        string GetSummary();
    }

    [Serializable]
    public class Customer : ISummerizable //customer class
    {
        public string FirstName { get; }
        public string LastName { get; }
        public int ID { get; }

        public Customer(int idno, string fname, string lname)  //constructor
        {
            ID = idno;
            FirstName = fname;
            LastName = lname;
        }

        public Customer(string fname, string lname) : this(new Random().Next(10000), fname, lname) { } // overloaded custructor

        public override bool Equals(object obj) //override equals method
        {
            var customer = obj as Customer;
            return customer != null &&
                   ID == customer.ID;
        }

        public override int GetHashCode() //override Hashcode method
        {
            return -489879552 + ID.GetHashCode();
        }

        public override string ToString() //override To String method
        {
            return "Customer: ID number: " + ID + " Last Name: " + FirstName + " First name: " + LastName;
        }

        public string GetSummary()
        {
            return ToString();
        }
    }

    public class Cone : ISummerizable
    {
        //constant values for scoop and cone
        const double SCOOP = 0.5;
        const double CONE = 0.75;
        public int CreamVYogurt { get; }//Ice cream has int 1 val, Yogurt is int 2 val
        public int Flavor { get; }
        public int ScoopsRequested { get; }
        public int ConeSelection { get; }
        public double Price { get; }

        public Cone(int iycheck, int flavorCheck, int scoopCheck, int coneCheck)
        {
            CreamVYogurt = iycheck;//assignment of  Cream v. yogurt value
            Flavor = flavorCheck;//assignemnt of Flavor value
            ScoopsRequested = scoopCheck;//Assignemnt of ScoopsRequested
            ConeSelection = coneCheck;

            //calculate order cost
            Price = (CONE + (SCOOP * ScoopsRequested));
        }

        public string GetSummary()
        {
            string iCream = "Ice Cream";
            string yogurt = "Yogurt";
            string choc = "Chocolate";
            string vanilla = "Vanilla";
            string strawb = "Strawberry";
            string waffle = "Waffle Cone";
            string cake = "Cake Cone";
            string iceVYog = "0";//initialized strings with meaningless info
            string flavor = "0";
            string coneSelection = "0";


            //switch statements to assign string values
            switch (CreamVYogurt)
            {
                case 1:
                    iceVYog = iCream;
                    break;
                case 2:
                    iceVYog = yogurt;
                    break;
                default:
                    break;
            }

            switch (Flavor)
            {
                case 1:
                    flavor = choc;
                    break;
                case 2:
                    flavor = vanilla;
                    break;
                case 3:
                    flavor = strawb;
                    break;
                default:
                    break;
            }

            switch (ConeSelection)
            {
                case 1:
                    coneSelection = waffle;
                    break;
                case 2:
                    coneSelection = cake;
                    break;
                default:
                    break;
            }

            //  cone info
            return string.Format("The cone with {0} scoops of {1} {2} served in a {3} will cost {4:c}.",
                ScoopsRequested, flavor, iceVYog, coneSelection, Price);
        }

        public class Order : ISummerizable
        {
            private Customer Customer { get; }
            private Cone[] customerCones { get; }
            public Order(Customer customer) // constructor
            {
                Customer = customer;
                customerCones = new Cone[10];
            }

            public void GetOrder()
            {
                bool isAnotherCone = true;
                for (int i = 0; i < customerCones.Length && isAnotherCone; i++)
                {
                    // prompt for ice cream or yogurt
                    int iycheck = InputHelper.PromptForInt(
                        "Would you like Ice Cream or Yogurt?\n1. Ice Cream\n2. Yogurt\nEnter your selection >> ",
                        x => x == 1 || x == 2); // ensure valid type
                    //prompt for flavor
                    int flavorCheck = InputHelper.PromptForInt(
                        "What flavor would you like?\n1. Chocolate\n2. Vanilla\n3. Strawberry\nEnter your selection >> ",
                        x => x >= 1 && x <= 3); //manual error checking for values outside 1-3
                    // prompt for scoops
                    int scoopCheck = InputHelper.PromptForInt(
                        "How many scoops would you like?\nEnter your selection: ",
                        x => x > 0); // ensure x is a positive integer
                    // prompt for cone type
                    int coneCheck = InputHelper.PromptForInt(
                        "What type of cone would you like?\n1. Waffle cone\n2. Cake Cone\nEnter your selection >> ",
                        x => x == 1 || x == 2); // ensure valid cone type
                    customerCones[i] = new Cone(iycheck, flavorCheck, scoopCheck, coneCheck);
                    isAnotherCone = InputHelper.PromptForInt("Would you like another cone?\n1. Yes\n2. No\nEnter your selection >> ",
                        x => x == 1 || x == 2) == 1;
                }
            }


            public string GetSummary()
            {
                StringBuilder stringBuilder = new StringBuilder("\n");
                stringBuilder.Append(Customer.GetSummary());
                stringBuilder.Append("\n");
                double totalPrice = 0;
                for (int i = 0; i < customerCones.Length && customerCones[i] != null; i++)
                {
                    // generate individual cone summary
                    stringBuilder.Append(customerCones[i].GetSummary());
                    stringBuilder.Append("\n");
                    totalPrice += customerCones[i].Price;
                }
                stringBuilder.Append(string.Format("Your total price for this order comes to {0:c}", totalPrice));
                return stringBuilder.ToString();
            }
        }
        class Program
        {
            static void Main(string[] args) //main
            {
                bool isAnotherCustomer = true;
                
                while (isAnotherCustomer)
                {
                    Dictionary<int, string> customerMap = IOHelper.GetCustomerFileMap();
                    Customer customer = null;
                     
                    if (customerMap.Count > 0)
                    {
                        Console.WriteLine("Customers found. Please select from any of these\n0. New customer");
                        new List<KeyValuePair<int, string>>(customerMap).ForEach(
                            keyvaluePair => Console.WriteLine("{0}. {1}", keyvaluePair.Key, keyvaluePair.Value));
                        string selectedCustomerFileName;
                        if (customerMap.TryGetValue(InputHelper.PromptForInt("Please enter a value >> ",
                            x => customerMap.ContainsKey(x) || x == 0),
                            out selectedCustomerFileName))
                        {
                            customer = IOHelper.LoadCustomer(selectedCustomerFileName);
                        }
                    }
                
                    if (customer == null)
                    {
                        // prompt for customer
                        customer = new Customer(
                            InputHelper.PromptFor("Customer first name >> "),
                            InputHelper.PromptFor("Customer last name >> "));
                        IOHelper.SaveCustomer(customer);
                    }
                    Console.WriteLine("Inputting order for customer:\n{0}\n", customer.GetSummary());
                    // TODO use static data members

                    // begin customer order
                    Order order = new Order(customer);
                    order.GetOrder();

                    // display summary
                    Console.WriteLine(order.GetSummary());
                    Console.WriteLine("\nThank you come again!\n");

                    // ask for another customer
                    isAnotherCustomer = InputHelper.PromptForInt("Is there another customer?\n1. Yes\n2. No\n Enter your selection >>",
                        x => x == 1 || x == 2) == 1;
                }
            }
        }

        // helper class for IO operations
        class IOHelper
        {
            private static BinaryFormatter formatter = new BinaryFormatter();
            public static void SaveCustomer(Customer customer)
            {
                string fileName = string.Format("Customer{0}.dat", customer.ID);
                try
                {
                    using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        formatter.Serialize(stream, customer);
                    }
                    MessageBox.Show(string.Format("{0} was saved.", fileName));
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an issue saving the customer.");
                    Console.WriteLine(e.ToString());
                }
            }

            public static Customer LoadCustomer(string fileName)
            {
                Customer customer = null;
                try
                {
                    using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        customer = (Customer) formatter.Deserialize(stream);
                    }
                    MessageBox.Show(string.Format("{0} was loaded.", fileName));
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an issue saving the customer.");
                    Console.WriteLine(e.ToString());
                }
                return customer;
            }

            private static List<string> GetCustomerNames()
            {
                List<string> customerFiles = new List<String>();
                string[] files = Directory.GetFiles(".");
                foreach(string file in files)
                {
                    if (file.Contains("Customer") && file.Contains(".dat"))
                    {
                        customerFiles.Add(file);
                    }
                }
                return customerFiles;
            }

            public static Dictionary<int, string> GetCustomerFileMap()
            {
                Dictionary<int, string> customerMap = new Dictionary<int, string>();
                int i = 1;
                foreach (string customerFileName in GetCustomerNames())
                {
                    customerMap.Add(i, customerFileName);
                    i++;
                }
                return customerMap;
            }
        }

        // helper class for user input
        class InputHelper
        {
            // tries to get the integer input and requests them to input again if failed to parse
            public static int PromptForInt(string promptFor)
            {
                Console.Write(promptFor);
                string input = Console.ReadLine();
                int value;
                while (!int.TryParse(input, out value))
                {
                    Console.Write("Sorry, that selection is invalid.\nPlease try again >> ");
                    input = Console.ReadLine();
                }
                return value;
            }

            // tries to get the integer input and requests them to input again if failed to parse
            // uses provided delegate to determine additional conditions to ask for reinput
            public static int PromptForInt(string promptFor, Func<int, bool> additionalCondition) // delegate use
            {
                Console.Write(promptFor);
                string input = Console.ReadLine();
                int value;
                while (!int.TryParse(input, out value) || !additionalCondition(value))
                {
                    Console.Write("Sorry, that selection is invalid.\nPlease try again >> ");
                    input = Console.ReadLine();
                }
                return value;
            }

            public static string PromptFor(string promptFor)
            {
                Console.Write(promptFor);
                return Console.ReadLine();
            }
        }
    }
}