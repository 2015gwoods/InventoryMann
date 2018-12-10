using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InventoryMann
{
    class Program
    {
        //CTRL+k+c:Comment

        //CTR+k+u:UnComment
        static void Main(string[] args)
        {
            List<Item> defaultDatabase=new List<Item>();
            DisplayWelcome();
            //defaultDatabase = DisplayInitialize();
            DisplayMenu(defaultDatabase);
            DisplayClosing();
        }

        private static void DisplayMenu(List<Item> database)
        {
            bool isUserIncorrect = false; //The user is always right except when they are wrong (Read:always)
            bool isUsing = true; //This runs the menu loop
            List<Item> userInventory = new List<Item>();
            string inventoryPath=@"data\inventories", databasePath=@"data\databases";

            do
            {
                DisplayHeader("Main Menu");

                //Database
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("F) Show Database");
                Console.WriteLine("G) Load Database File");
                Console.WriteLine("H) Save Database To File");
                Console.WriteLine("I) Add Item To Database");
                Console.WriteLine("J) Remove Item From Database");
                Console.WriteLine();
                
                //Dangerous Commands: Will wipe data
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("X)Dump Active Database");
                Console.WriteLine("Z)Dump Active Inventory");
                Console.WriteLine("Q) Quit");

                if (isUserIncorrect)//When the user makes a wrong entry
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a choice above.");
                    isUserIncorrect = false;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("Menu:> ");
                Console.ForegroundColor = ConsoleColor.White;

                switch (Console.ReadLine().ToLower())//Let's save some typing
                {
                    //case "a":DisplayShowUserInventory(userInventory);
                    //    break;
                    //case "b":DisplayShowAddItemUserInventory(database, userInventory);
                    //    break;
                    //case "c":DisplayShowRemoveItemUserInventory(userInventory);
                    //    break;
                    //case "d":DisplayShowSaveInventory(userInventory,inventoryPath);
                    //    break;
                    //case "e": DisplayShowLoadInventory(userInventory, inventoryPath);
                    //    break;
                    case "f":DisplayShowDatabase(database);
                        break;
                    case "g":DisplayLoadDatabase(database,databasePath,"database");
                        break;
                    case "h":DisplaySaveDatabase(database, databasePath, "database");
                        break;
                    case "i":DisplayAddToDatabase(database);
                        break;
                    case "j":DisplayRemoveToDatabase(database);
                        break;
                    case "x":DisplayDumpDatabase(database);
                        break;
                   case "z":DisplayDumpInventory(userInventory);
                       break;
                    case "q":isUsing = false;
                        break;
                    default:
                        isUserIncorrect = true;
                        break;
                }




            } while (isUsing);
        }

        private static void DisplayLoadDatabase(List<Item> database, string databasePath,string databaseType)
        {
            string targetFile, targetFilePath, itemLine;
            string[] fileBuffer; string[] stringBuffer = new string[3];
            Item newItem = new Item();
            Item.ItemType typeHolder;
            bool wasRead = false;
            
            DisplayHeader($"Load a {databaseType} from file");

            //got this from the web
            string[] fileArray = Directory.GetFiles(databasePath);

            foreach (var file in fileArray)
            {
                Console.Write(file + " ");
            }

            //https://stackoverflow.com/questions/14877237/getting-all-file-names-from-a-folder-using-c-sharp
            Console.WriteLine();
            Console.WriteLine("Please enter a file to load (You only need the name, not the path)");
            targetFile = Console.ReadLine();
            targetFilePath = databasePath + @"\" + targetFile + ".txt";

            try
            {
                File.ReadAllLines(targetFilePath);
                wasRead = true;
                
            }
            
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to read " + targetFile);
            }
            if (wasRead)
            {
                //For some reason it was unhappy when I removed throw
                fileBuffer = File.ReadAllLines(targetFilePath);
                for (int i = 0; i < fileBuffer.Length; i++)
                {
                    
                    itemLine = fileBuffer[i];
                    if (itemLine=="")
                    {
                        break;
                    }
                    stringBuffer = itemLine.Split(';');
                    newItem.Name = stringBuffer[0];
                    Enum.TryParse<Item.ItemType>(stringBuffer[1], out typeHolder);
                    newItem.itemType = typeHolder;

                    newItem.damDesc = stringBuffer[2];
                    database.Add(newItem);
                }
                Console.WriteLine($"All items added {databaseType} now has {database.Count}");
            }
            DisplayContinue();

        }

        private static void DisplaySaveDatabase(List<Item> database, string databasePath,string databaseType)
        {
            string targetFile, targetFilePath, itemLine;
            string[] fileBuffer=new string[database.Count];
            Item newItem = new Item();
            string typeHolder;
            bool wasWrite = false;
            int index = 0;
            DisplayHeader($"Save a {databaseType} from file");
            
            //got this from the web
            string[] fileArray = Directory.GetFiles(databasePath);
            
            foreach (var file in fileArray)
            {
                Console.Write(file +" ");
            }
            
            //https://stackoverflow.com/questions/14877237/getting-all-file-names-from-a-folder-using-c-sharp
            Console.WriteLine();
            Console.WriteLine("Please enter a file to write to (You only need the name, not the path(You may make new files))");
            targetFile=Console.ReadLine();
            targetFilePath = databasePath + @"\"+targetFile + ".txt";

            Console.WriteLine("Do you want to save to this file?");
            if (Console.ReadLine().ToLower()=="yes")
            {
                foreach (var item in database)
                {

                    itemLine = item.Name + ";" + item.itemType.ToString() + ";" + item.damDesc;
                    fileBuffer[index] = itemLine;
                    index++;
                }

                
            }
            try
            {
                File.AppendAllLines(targetFilePath, fileBuffer);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to write to file");
                
            }
            if (wasWrite)
            {
                Console.WriteLine("All items written");
            }

            DisplayContinue();
            
        }








        #region List Parrent Methods
        //These are the methods that make use of all child methods to perform the menu functions
        private static void DisplayRemoveToDatabase(List<Item> database)
        {
            bool isValid = false;
            string userResponse;
            Item targetItem;
            DisplayHeader("Add An Item To the Database");
            DisplayShowDatabaseCount(database);
            DisplayPromptFilterDatabase(database);
            Console.Write("Please Enter the Name of the Item you want to remove: ");
            targetItem = FindItem(Console.ReadLine().ToLower(), database);

            Console.WriteLine("This is the item you have selected for removal");
            DisplayItem(targetItem);
            Console.WriteLine("Do you want to remove this item?(Yes/No)");
            do
            {
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "yes")
                {
                    database.Remove(targetItem);
                    isValid = true;
                }
                else if (userResponse == "no")
                {
                    Console.WriteLine("Cancling");
                    isValid = true;

                }
                else
                {
                    Console.WriteLine($"Please enter yes or no to confirm");
                    isValid = false;
                }
            } while (!isValid);

            DisplayContinue();
        }
        private static void DisplayAddToDatabase(List<Item> database)
        {
            bool isValid = false;
            string userResponse;
            Item newItem = new Item();
            Item.ItemType typeHolder;
            DisplayHeader("Add An Item To the Database");

            Console.Write("Please Enter the Name of the Item: ");
            newItem.Name = Console.ReadLine();

            do
            {
                Console.WriteLine("Please Enter the Item Type(OPTIONS: generic, armor, weapon, magic)");
            } while (!Enum.TryParse<Item.ItemType>(Console.ReadLine(), out typeHolder));
            newItem.itemType = typeHolder;
            Console.WriteLine("Please enter a description of the damage the item deals if any");
            newItem.damDesc = Console.ReadLine();

            Console.WriteLine("This is what you put in for the item's data:");
            DisplayItem(newItem);

            Console.WriteLine("Is this what you want to add to the database?(Yes/No)");
            do
            {
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "yes")
                {
                    database.Add(newItem);
                    isValid = true;
                }
                else if (userResponse == "no")
                {
                    Console.WriteLine("Cancling");
                    isValid = true;

                }
                else
                {
                    Console.WriteLine($"Please enter yes or no to confirm");
                    isValid = false;
                }
            } while (!isValid);


            DisplayContinue();

        }
        static void DisplayShowDatabaseCount(List<Item> database)
        {
            Console.WriteLine($"There are {database.Count} items in the active database");
            Console.WriteLine("You may wish to filter your search as the default will show everything.");
            Console.WriteLine();

        }
        private static void DisplayDumpDatabase(List<Item> database)
        {
            bool isValid = false;
            string userResponse;
            DisplayHeader("Clear Database List");
            Console.WriteLine("This will delete your database list. Please only use this");
            Console.WriteLine("when you don't want your data back");
            Console.WriteLine("Are you sure you want to do this?");
            do
            {
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "yes")
                {
                    database.Clear();
                    isValid = true;
                }
                else if (userResponse == "no")
                {

                    isValid = true;

                }
                else
                {
                    Console.WriteLine($"Please enter yes or no as there are {database.Count} items.");
                    isValid = false;
                }

            } while (!isValid);
            DisplayContinue();
        }
        private static void DisplayDumpInventory(List<Item> userInventory)
        {
            bool isValid = false;
            string userResponse;
            DisplayHeader("Clear Inventory List");
            Console.WriteLine("This will delete your inventory list. Please only use this");
            Console.WriteLine("when you don't want your data back");
            Console.WriteLine("Are you sure you want to do this?");
            do
            {
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "yes")
                {
                    foreach (var item in userInventory)
                    {
                        userInventory.Remove(item);
                    }

                    isValid = true;
                }
                else if (userResponse == "no")
                {

                    isValid = true;

                }
                else
                {
                    Console.WriteLine($"Please enter yes or no as there are {userInventory.Count} items.");
                    isValid = false;
                }

            } while (!isValid);
            DisplayContinue();
        }
        #endregion
        #region List User Interface Methods
        //These display information to the user and in some cases make requests
        private static void DisplayPromptFilterDatabase(List<Item> database)
        {
            bool isValid = false;
            string userResponse;
            do
            {
                Console.WriteLine("Would you like to filter?(Yes/No)");
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "yes")
                {
                    ShowSpecificItemType(database);
                    isValid = true;
                }
                else if (userResponse == "no")
                {
                    ShowListNameContents(database);
                    isValid = true;

                }
                else
                {
                    Console.WriteLine($"Please enter yes or no as there are {database.Count} items.");
                    isValid = false;
                }
            } while (!isValid);
        }
        private static void DisplayShowDatabase(List<Item> database)
        {
            string userResponse;
            bool isValid = false;
            DisplayHeader("View the Database");
            DisplayShowDatabaseCount(database);
            DisplayPromptFilterDatabase(database);

            do
            {
                Console.WriteLine("This is what we found. Would you like to view an item in detail? (Yes/No)");
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "yes")
                {
                    ShowSpecificItem(database);
                    isValid = true;
                }
                else if (userResponse == "no")
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine("Yes or No Please!");
                    isValid = false;
                }
            } while (!isValid);

        }

        #endregion
        #region File IO

        #endregion
        #region List Navigational Methods
        //These let other methods interact with the lists
        private static void ShowSpecificItem(List<Item> database)
        {
            Item targetItem;
            Console.WriteLine("Please enter the Name of the item");
            targetItem = FindItem(Console.ReadLine().ToLower(), database);
            DisplayItem(targetItem);
        }
        private static void DisplayItem(Item targetItem)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("////////////////////////////////////////////////////////////");
            Console.WriteLine($"Name: " + targetItem.Name);
            Console.WriteLine($"Type: " + targetItem.itemType.ToString());
            Console.WriteLine($"Damage Descriptor: "+targetItem.damDesc);
            Console.WriteLine("////////////////////////////////////////////////////////////");
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static Item FindItem(string name, List<Item> database)
        {
            bool isItemFound = false, isValid = true;
            string userResponse;
            Item targetItem = new Item();
            do
            {
                foreach (var item in database)
                {
                    if (name == item.Name)
                    {
                        targetItem = item;
                        isItemFound = true;
                        break;
                    }
                }
                if (!isItemFound)
                {
                    do
                    {

                        Console.WriteLine("Item was not found! Would you like to enter another Name?(Yes/No)");
                        userResponse = Console.ReadLine().ToLower();
                        if (userResponse == "yes")
                        {
                            Console.WriteLine("Please enter a new Name");
                            name = Console.ReadLine().ToLower();
                            isValid = true;
                        }
                        else if (userResponse == "no")
                        {
                            isItemFound = true;
                            isValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Yes or No Please!");
                            isValid = false;
                        }
                    } while (!isValid);


                }

            } while (!isItemFound);
            return targetItem;
        }
        private static void ShowSpecificItemType(List<Item> database)
        {
            Item.ItemType itemTypeSearch;

            do
            {
                Console.WriteLine("What would you like to find?");
                Console.WriteLine("OPTIONS: generic, armor, weapon, magic ");
            } while (!Enum.TryParse<Item.ItemType>(Console.ReadLine(), out itemTypeSearch));

            ShowListFilterContents(database, itemTypeSearch);

        }
        static void ShowListNameContents(List<Item> listSet)
        {
            Console.WriteLine();
            foreach (Item item in listSet)
            {
                Console.Write(item.Name + " ");
            }
            Console.WriteLine();
        }
        static void ShowListFilterContents(List<Item> listSet, Item.ItemType searchQu)
        {
            Console.WriteLine();
            foreach (Item item in listSet)
            {
                if (item.itemType == searchQu)
                {
                    Console.Write(item.Name + " ");
                }

            }
            Console.WriteLine();
        }
        #endregion
        #region Helper Methods
        static void DisplayClosing()
        {
            DisplayHeader("Thank You For using InventoryMann!");
            Console.WriteLine();
            Console.WriteLine("Press any key to Quit");
            Console.ReadKey();
        }

        static void DisplayWelcome()
        {
            DisplayHeader("Welcome To InventoryMann");
            Console.WriteLine();
            Console.WriteLine("This program is designed to help you keep track of your inventories");
            Console.WriteLine("without the paywall of DnD Beyond! Add any items you like to your list");
            Console.WriteLine("and save them for later. Note that only the files located in the default");
            Console.WriteLine("database folder will be loaded on program start and other files will need");
            Console.WriteLine("to be loaded manually. New default data can be added to the folder at any");
            Console.WriteLine("time by moving a text file into the default folder. You may want to use");
            Console.WriteLine("the program to format your items for you.");
            Console.WriteLine();
            Console.WriteLine("Enjoy!");
            DisplayContinue();
        }
        static void DisplayContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to Continue");
            Console.ReadKey();
        }
        static void DisplayHeader(string text)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\t\t{text}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
        #endregion
    }
}
