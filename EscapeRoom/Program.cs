using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeRoom
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Allyson's Escape Room");
            Console.WriteLine("Hit Enter to continue...");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("You wake up in an unfamiliar bedroom, on the floor. There is a bed, a dresser, a desk and a closet. The window over the bed faces North, the closet is on the opposite wall.");
            string userResponse = "";
            string currentCommand = "";
            string primaryObjectName = "";
            List<string> secondaryObjectsNames = new List<string>();
            string[] commandsList = { "inspect", "use", "help", "quit" };

            List<RoomObject> roomObjects = new List<RoomObject>();
            RoomObjectsCollection objectsCollection = new RoomObjectsCollection();
            InitializeRoomObjects(objectsCollection);

            while (userResponse != "quit")
            {
                Console.WriteLine("What would you like to do?");
                userResponse = Console.ReadLine().ToLower();
                string[] responseArray = userResponse.Split(' ');
                if (responseArray.Length > 0)
                {
                    if (IsValidCommand(responseArray[0], commandsList))
                    {
                        currentCommand = responseArray[0];
                        if (currentCommand == "help")
                        {
                            string availableCommands = "Available Commands: " + string.Join(", ", commandsList);
                            Console.WriteLine(availableCommands);
                            continue;
                        }
                        if (currentCommand == "quit")
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("I don't know how to do that.");
                        continue;
                    }
                }

                if (currentCommand == "inspect")
                {
                    primaryObjectName = responseArray[1];
                }
                if (currentCommand == "use")
                {
                    primaryObjectName = responseArray[responseArray.Length-1];
                    secondaryObjectsNames = ParseSecondaryObjects(responseArray);
                }
                bool objectFound = false;

                RoomObject primaryObject = objectsCollection.FindRoomObject(primaryObjectName);

                if (primaryObject == null)
                {
                    Console.WriteLine("No " + primaryObjectName + " exist to " + currentCommand + ".");
                    continue;
                }

                objectFound = true;
                if (currentCommand == "inspect")
                {
                    if (primaryObject.ParentName == "")
                    {
                        primaryObject.Inspect();
                        continue;
                    }
                    else
                    {
                        RoomObject parentObject = objectsCollection.FindRoomObject(primaryObject.ParentName);
                        if (parentObject != null && parentObject.HasBeenInspected == true)
                        {
                            primaryObject.Inspect();
                            continue;
                        }
                        else
                        {
                            objectFound = false;
                        }
                    }
                }
                else if (currentCommand == "use")
                {
                    RoomObject secondaryObject = null;

                    if (secondaryObjectsNames.Count() > 0)
                    {
                        secondaryObject = objectsCollection.FindRoomObject(secondaryObjectsNames[0]);
                    }
                    

                    if (primaryObject == null)
                    {
                        Console.WriteLine("You must use that ON something.");
                        continue;
                    }

                    if (secondaryObject == null)
                    {
                        Console.WriteLine("You can't use that on that object.");
                        continue;
                    }

                    if (primaryObject.ParentName == "")
                    {
                        if (secondaryObject.ParentName == "")
                        {
                            primaryObject.Use(secondaryObject);
                        }

                        else
                        {
                            RoomObject parentObject = objectsCollection.FindRoomObject(secondaryObject.ParentName);
                            if (parentObject.HasBeenInspected)
                            {
                                primaryObject.Use(secondaryObject);
                            }

                            else
                            {
                                Console.WriteLine("You don't see a " + secondaryObject.Name);
                                continue;
                            }
                        }
                    }

                    else
                    {
                        RoomObject parentObject = objectsCollection.FindRoomObject(primaryObject.ParentName);
                        if (parentObject.HasBeenInspected)
                        {
                            if (secondaryObject.ParentName == "")
                            {
                                primaryObject.Use(secondaryObject);
                            }

                            else
                            {
                                RoomObject secondarParentObject = objectsCollection.FindRoomObject(secondaryObject.ParentName);
                                if (secondarParentObject.HasBeenInspected)
                                {
                                    primaryObject.Use(secondaryObject);
                                }

                                else
                                {
                                    Console.WriteLine("You don't see a " + secondaryObject.Name);
                                    continue;
                                }
                            }
                        }

                        else
                        {
                            Console.WriteLine("You don't see a " + primaryObject.Name);
                            continue;
                        }
                    }

                   

                }
                if (objectFound == false)
                {
                    Console.WriteLine("You don't see that object");
                }
            }
        }

        private static List<string> ParseSecondaryObjects(string[] responseArray)
        {
            List<string> returnList = new List<string>();
            int ctr = 1;
            while(ctr < responseArray.Length - 2)
            {
                if (responseArray[ctr] == "with")
                {
                    ctr++;
                    continue;
                }

                if (responseArray[ctr] == "on")
                {
                    break;
                }

                returnList.Add(responseArray[ctr]);
                ctr++;
            }
            return returnList;
        }

        private static bool IsValidCommand(string valueToCheck, string[] commandsList)
        {
            foreach (string command in commandsList)
            {
                if (command == valueToCheck)
                {
                    return true;
                }
            }
            return false;
        }

        private static void InitializeRoomObjects(RoomObjectsCollection objectsCollection)
        {
            RoomObject desk = new RoomObject();
            desk.Name = "desk";
            desk.Description = "The desk is a flimsy particle board computer desk, circa 1991. The drawers are warped shut. Laying on the writing surface is a note. The desk lamp's flourescent light is blinking on and off. A coffee cup full of pens and pencils sits in one corner.";
            desk.HasBeenInspected = false;
            desk.ParentName = "";
            objectsCollection.AddRoomObject(desk);

            RoomObject note = new RoomObject();
            note.Name = "note";
            note.Description = "\"Hello, friend. I am sorry to have run out while you were sleeping. Please find the key in the dresser. I am sure to be back, soon\"";
            note.HasBeenInspected = false;
            note.ParentName = "desk";
            objectsCollection.AddRoomObject(note);

            RoomObject dresser = new RoomObject();
            dresser.Name = "dresser";
            dresser.Description = "The dresser is a well-worn cherry wood bureau. The drawers are closed, but bits of their contents are spilling out. As you open and rifle through each drawer, you find what you'd expect. Socks, underwear, t-shirts shoved in haphazardly. You dismiss most of this as junk. Finally in the fourth drawer, you find an old iron skeleton key.";
            dresser.HasBeenInspected = false;
            dresser.ParentName = "";
            objectsCollection.AddRoomObject(dresser);

            RoomObject closet = new RoomObject();
            closet.Name = "closet";
            closet.Description = "You slide open the closet door to find dresses for every season, hanging askew from their hangers. The shelf at the top of the closet is stacked with shoe boxes. The floor, is uncharacteriscally free of clutter. A soft light eminates from behind the clothes.";
            closet.HasBeenInspected = false;
            closet.ParentName = "";
            objectsCollection.AddRoomObject(closet);

            RoomObject dresses = new RoomObject();
            dresses.Name = "dresses";
            dresses.Description = "The dresses hanging in the closet are fashionable. They are arranged in no particular order; it looks like they've just been jammed in, and carelessly moved about. You push aside the dresses and reveal a large, wooden door.";
            dresses.HasBeenInspected = false;
            dresses.ParentName = "closet";
            objectsCollection.AddRoomObject(dresses);

            RoomObject key = new RoomObject();
            key.Name = "key";
            key.Description = "The skeleton key feels heavy in your hand. The handle is wrought to look like a smiling skull.";
            key.HasBeenInspected = false;
            key.ParentName = "dresser";
            objectsCollection.AddRoomObject(key);

            Door door = new Door();
            door.Name = "door";
            door.Locked = true;
            door.Description = "The door is a large, oak exterior door. It has brass hinges and there is a skeleton keyhole below the brass knob.";
            door.HasBeenInspected = false;
            door.ParentName = "dresses";
            objectsCollection.AddRoomObject(door);
        }

    }

    public class RoomObject
    {
        public string Name { get; set;}
        public string Description { get; set; }
        public bool HasBeenInspected { get; set; }
        public string ParentName { get; set;  }

        public virtual void Inspect()
        {
            Console.WriteLine(Description);
            HasBeenInspected = true;
        }
        public virtual void Use(RoomObject usableObject)
        {
            //if (usableObject.HasBeenInspected == false)
            //{
            //    Console.WriteLine("You haven't found a(n) " + usableObject.Name);
            //    return;
            //}
            Console.WriteLine("You use the " + usableObject.Name +" on the " + Name);
        }
    }
    
    public class RoomObjectsCollection
    {
        public List<RoomObject> RoomObjectsList { get; set; }

        public RoomObjectsCollection()
        {
            RoomObjectsList = new List<RoomObject>();
        }

        public void AddRoomObject (RoomObject objectToAdd)
        {
            RoomObjectsList.Add(objectToAdd);
        }

        public RoomObject FindRoomObject(string objectName)
        {
            foreach (RoomObject rObject in RoomObjectsList)
            {
                if (rObject.Name.ToLower() != objectName.Trim())
                    continue;
                else
                    return rObject;
            }
            return null;
        }

    }

    public class Door : RoomObject
    {
        public bool Locked { get; set; }

        public override void Inspect()
        {
            
            if (Locked == true)
            {
                Console.WriteLine(Description + " It appears to be locked.");
            }
            else {
                Console.WriteLine(Description + " It appears to be unlocked.");
            }
        }

        public override void Use(RoomObject item)
        {

           
            if (item.Name == "key")
            {
                Locked = false;
                Console.WriteLine("You hear the soft click as the key turns in the lock. The door is unlocked.");
            }
        }
    }
}
