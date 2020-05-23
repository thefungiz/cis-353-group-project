using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Group_project
{
    public interface ISummerizable // interface defining contract for returning summary as string
    {
        string GetSummary();
    }

    // class to represent the customer entity
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
            return string.Format("Customer> ID: [{0}] Last name: [{1}] First name: [{2}]", ID, LastName, FirstName);
        }

        // implemented interface method
        public string GetSummary()
        {
            return ToString();
        }
    }

    // class to represent the cone entity
    public class Cone : ISummerizable
    {
        // constant static data members for scoop and cone
        // constants are always static
        public const double SCOOP = 0.5;
        public const double CONE = 0.75;
        public const string ICE_CREAM = "Ice Cream";
        public const string YOGURT = "Yogurt";
        public const string CHOCOLATE = "Chocolate";
        public const string VANILLA = "Vanilla";
        public const string STRAWBERRY = "Strawberry";
        public const string WAFFLE = "Waffle Cone";
        public const string CAKE_CONE = "Cake Cone";

        public int CreamVYogurt { get; }//Ice cream has int 1 val, Yogurt is int 2 val
        public int Flavor { get; }
        public int ScoopsRequested { get; }
        public int ConeSelection { get; }
        public double Price { get; }

        public Cone(int iycheck, int flavorCheck, int scoopCheck, int coneCheck) // constructor
        {
            CreamVYogurt = iycheck;//assignment of  Cream v. yogurt value
            Flavor = flavorCheck;//assignemnt of Flavor value
            ScoopsRequested = scoopCheck;//Assignemnt of ScoopsRequested
            ConeSelection = coneCheck;

            //calculate order cost
            Price = (CONE + (SCOOP * ScoopsRequested));
        }

        public string GetSummary()// implemented interface method
        {
            string iceVYog = "0";//initialized strings with meaningless info
            string flavor = "0";
            string coneSelection = "0";

            //switch statements to assign string values
            switch (CreamVYogurt)
            {
                case 1:
                    iceVYog = ICE_CREAM;
                    break;
                case 2:
                    iceVYog = YOGURT;
                    break;
                default:
                    break;
            }

            switch (Flavor)
            {
                case 1:
                    flavor = CHOCOLATE;
                    break;
                case 2:
                    flavor = VANILLA;
                    break;
                case 3:
                    flavor = STRAWBERRY;
                    break;
                default:
                    break;
            }

            switch (ConeSelection)
            {
                case 1:
                    coneSelection = WAFFLE;
                    break;
                case 2:
                    coneSelection = CAKE_CONE;
                    break;
                default:
                    break;
            }

            //  cone info
            return string.Format("The cone with {0} scoops of {1} {2} served in a {3} will cost {4:c}.",
                ScoopsRequested, flavor, iceVYog, coneSelection, Price);
        }
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
                    "How many scoops would you like?\nEnter your selection >> ",
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

        public string GetSummary() // implemented interface method
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
            Console.WriteLine("~~~ Ice Cream ~~~\n Enter 'q' at any prompt to exit.");
            bool isAnotherCustomer = true; // sentinel to determine if to ask for another customer
            while (isAnotherCustomer) // main program loop
            {
                Dictionary<int, Customer> customerDictionary = IOHelper.GetCustomerDictionary();
                Customer customer = null;

                if (customerDictionary.Count > 0)
                {
                    Console.WriteLine("Customers found. Please select from any of these\n0. New customer");
                    new List<KeyValuePair<int, Customer>>(customerDictionary).ForEach(
                        keyvaluePair => Console.WriteLine("{0}. {1}", keyvaluePair.Key, keyvaluePair.Value.GetSummary()));// print all the customers that can be selected
                                                                                                                          
                    customerDictionary.TryGetValue(InputHelper.PromptForInt("Please enter a value >> ", // try to get customer from selection
                         x => customerDictionary.ContainsKey(x) || x == 0),
                         out customer); // if 0 selected, a new customer will be created. If an index of an existing customer is selected, use that customer object
                }

                if (customer == null)
                {
                    // prompt for customer
                    customer = new Customer(
                        // predicate delegate to check for id use
                        InputHelper.PromptForInt("Customer ID >> ", x => !customerDictionary.Values.Select(c => c.ID).Contains(x), "Sorry, that customer id is in use.\nPlease try again >> "),
                        InputHelper.PromptFor("Customer first name >> "),
                        InputHelper.PromptFor("Customer last name >> "));
                    IOHelper.SaveCustomer(new List<Customer>(customerDictionary.Values), customer); // save valid customer to the file
                }
                Console.WriteLine("\nInputting order for customer:\n{0}\n", customer.GetSummary());

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
        private const string fileName = "Customers.dat"; // static const data member
        private static BinaryFormatter formatter = new BinaryFormatter(); // used for serializing/deserializing objects to binary data and back

        // saves provied customer object to file
        public static void SaveCustomer(List<Customer> customerList, Customer newCustomer)
        {
            customerList.Add(newCustomer);
            try
            {// try appending the new customer to the file
                using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    formatter.Serialize(stream, customerList);
                }
                MessageBox.Show(string.Format("{0} \nwas saved.", newCustomer.GetSummary()));
            }
            catch (Exception e)
            {// show error if customer save failed
                MessageBox.Show("There was an issue saving the customer\n{0}", e.ToString());
            }
        }

        // loads a customers from a filename
        private static List<Customer> LoadCustomersFromFile()
        {
            List<Customer> customers = new List<Customer>();
            if (File.Exists(fileName)) // see if the customer file exists
            {
                try
                {
                    // if the file exists, load a list of customers
                    using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        customers = (List<Customer>)formatter.Deserialize(stream);
                    }
                    // display success message if customers loaded
                    MessageBox.Show(string.Format("{0} customers were successfully loaded from {1}", customers.Count, fileName));
                }
                catch (Exception e)
                {// show error message box if error
                    MessageBox.Show(string.Format("There was an issue loading the customer file\n{0}", e.ToString()));
                }
            }
            else
            {// if no file is found, tell user and create the file on next customer addition
                MessageBox.Show(string.Format("{0} was not found. A new file to store customers will be created", fileName));
            }
            return customers;
        }

        // generates a dictionary that maps customers to an index value (starting at 1)
        public static Dictionary<int, Customer> GetCustomerDictionary()
        {
            Dictionary<int, Customer> customerDictionary = new Dictionary<int, Customer>();
            int i = 1; // start at index 1
            foreach (Customer customer in LoadCustomersFromFile()) // loop through each customer
            {
                customerDictionary.Add(i, customer); // map index to customer for easy reference id
                i++;
            }
            return customerDictionary;
        }
    }

    // helper class for user input
    class InputHelper
    {
        public delegate bool IntegerInputPredicate(int toCheck); // delegate for provided additional input criteria

        // tries to get the integer input and requests them to input again if failed to parse
        public static int PromptForInt(string promptFor)
        {
            Console.Write(promptFor);
            string input = Console.ReadLine();
            ExitIfQuitEntered(input); // close program if 'q' entered
            int value;
            while (!int.TryParse(input, out value))
            {
                Console.Write("Sorry, that selection is invalid.\nPlease try again >> ");
                input = Console.ReadLine();
            }
            return value;
        }

        // close program if 'q' entered
        private static void ExitIfQuitEntered(string input)
        {
            if (input != null && "q".Equals(input.ToLower()))
            {
                Console.Write("Exiting... ");
                Environment.Exit(0); // exit with envionment code '0'
            }
        }

        // tries to get the integer input and requests them to input again if failed to parse
        // uses provided delegate to determine additional conditions to ask for reinput
        public static int PromptForInt(string promptFor, IntegerInputPredicate predicate, // delegate use
            string invalidMessage = "Sorry, that selection is invalid.\nPlease try again >> ") // optional param
        {
            Console.Write(promptFor);
            string input = Console.ReadLine();
            ExitIfQuitEntered(input); // close program if 'q' entered
            int value;
            while (!int.TryParse(input, out value) || !predicate(value))
            {
                Console.Write(invalidMessage);
                input = Console.ReadLine();
            }
            return value;
        }

        public static string PromptFor(string promptFor)
        {
            Console.Write(promptFor);
            string input = Console.ReadLine();
            ExitIfQuitEntered(input); // close program if 'q' entered
            return input;
        }
    }
}