/*==== DebugConsole.cs ====================================================
* Class for handling multi-line, multi-color debugging messages.
* Original Author: Jeremy Hollingsworth
* Based On: Version 1.2.1 Mar 02, 2006
* 
* Modified: Simon Waite
* Date: 22 Feb 2007
*
* Modification to original script to allow pixel-correct line spacing
*
* Setting the boolean pixelCorrect changes the units in lineSpacing property
* to pixels, so you have a pixel correct gui font in your console.
*
* It also checks every frame if the screen is resized to make sure the 
* line spacing is correct (To see this; drag and let go in the editor 
* and the text spacing will snap back)
*
* USAGE:
* ::Drop in your standard assets folder (if you want to change any of the
* default settings in the inspector, create an empty GameObject and attach
* this script to it from you standard assets folder.  That will provide
* access to the default settings in the inspector)
* 
* ::To use, call DebugConsole.functionOrProperty() where 
* functionOrProperty = one of the following:
* 
* -Log(string message, string color)  Adds "message" to the list with the
* "color" color. Color is optional and can be any of the following: "error",
* "warning", or "normal".  Default is normal.
* 
* Clear() Clears all messages
* 
* isVisible (true,false)  Toggles the visibility of the output.  Does _not_
* clear the messages.
* 
* isDraggable (true, false)  Toggles mouse drag functionality
* =========================================================================*/


using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class DebugConsole : MonoBehaviour
    {
        public GameObject DebugGui;             // The GUI that will be duplicated
        public Vector3 DefaultGuiPosition = new Vector3(0.01F, 0.98F, 0F);
        public Vector3 DefaultGuiScale = new Vector3(0.5F, 0.5F, 1F);
        public Color Normal = Color.green;
        public Color Warning = Color.yellow;
        public Color Error = Color.red;
        public int MaxMessages = 30;                   // The max number of messages displayed
        public float LineSpacing = 0.02F;              // The amount of space between lines
        public ArrayList Messages = new ArrayList();
        public ArrayList Guis = new ArrayList();
        public ArrayList Colors = new ArrayList();
        public bool Draggable = true;                  // Can the output be dragged around at runtime by default? 
        public bool Visible = true;                    // Does output show on screen by default or do we have to enable it with code? 
        public bool PixelCorrect = false; // set to be pixel Correct linespacing
        public static bool IsVisible
        {
            get
            {
                return Instance.Visible;
            }

            set
            {
                Instance.Visible = value;
                if (value)
                {
                    Instance.Display();
                }
                else if (value == false)
                {
                    Instance.ClearScreen();
                }
            }
        }

        public static bool IsDraggable
        {
            get
            {
                return Instance.Draggable;
            }

            set
            {
                Instance.Draggable = value;

            }
        }


        private static DebugConsole _sInstance;   // Our instance to allow this script to be called without a direct connection.
        public static DebugConsole Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                    if (_sInstance == null)
                    {
                        var console = new GameObject();
                        console.AddComponent<DebugConsole>();
                        console.name = "DebugConsoleController";
                        _sInstance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                        Instance.InitGuis();
                    }

                }

                return _sInstance;
            }
        }

        void Awake()
        {
            _sInstance = this;
            InitGuis();

        }

        protected bool GuisCreated;
        protected float ScreenHeight = -1;
        public void InitGuis()
        {
            var usedLineSpacing = LineSpacing;
            ScreenHeight = Screen.height;
            if (PixelCorrect)
                usedLineSpacing = 1.0F / ScreenHeight * usedLineSpacing;

            if (GuisCreated == false)
            {
                if (DebugGui == null)  // If an external GUIText is not set, provide the default GUIText
                {
                    DebugGui = new GameObject();
                    DebugGui.AddComponent<GUIText>();
                    DebugGui.name = "DebugGUI(0)";
                    DebugGui.transform.position = DefaultGuiPosition;
                    DebugGui.transform.localScale = DefaultGuiScale;
                }

                // Create our GUI objects to our maxMessages count
                var position = DebugGui.transform.position;
                Guis.Add(DebugGui);
                var x = 1;

                while (x < MaxMessages)
                {
                    position.y -= usedLineSpacing;
                    var clone = (GameObject)Instantiate(DebugGui, position, transform.rotation);
                    clone.name = string.Format("DebugGUI({0})", x);
                    Guis.Add(clone);
                    position = clone.transform.position;
                    x += 1;
                }

                x = 0;
                while (x < Guis.Count)
                {
                    var temp = (GameObject)Guis[x];
                    temp.transform.parent = DebugGui.transform;
                    x++;
                }
                GuisCreated = true;
            }
            else
            {
                // we're called on a screensize change, so fiddle with sizes
                var position = DebugGui.transform.position;
                for (var x = 0; x < Guis.Count; x++)
                {
                    position.y -= usedLineSpacing;
                    var temp = (GameObject)Guis[x];
                    temp.transform.position = position;
                }
            }
        }



        bool _connectedToMouse;
        void Update()
        {
            // If we are visible and the screenHeight has changed, reset linespacing
            if (Visible && ScreenHeight != Screen.height)
            {
                InitGuis();
            }
            if (Draggable)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_connectedToMouse == false && DebugGui.GetComponent<GUIText>().HitTest(Input.mousePosition))
                    {
                        _connectedToMouse = true;
                    }
                    else if (_connectedToMouse)
                    {
                        _connectedToMouse = false;
                    }

                }

                if (_connectedToMouse)
                {
                    var posX = Input.mousePosition.x / Screen.width;
                    var posY = Input.mousePosition.y / Screen.height;
                    DebugGui.transform.position = new Vector3(posX, posY, 0F);
                }
            }

        }
        //+++++++++ INTERFACE FUNCTIONS ++++++++++++++++++++++++++++++++
        public static void Log(string message, string color)
        {
            Instance.AddMessage(message, color);

        }
        //++++ OVERLOAD ++++
        public static void Log(string message)
        {
            //Instance.AddMessage(message);
        }

        public static void Clear()
        {
            Instance.ClearMessages();
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        //---------- void AddMesage(string message, string color) ------
        //Adds a mesage to the list
        //--------------------------------------------------------------

        public void AddMessage(string message, string color)
        {
            Messages.Add(message);
            Colors.Add(color);
            Display();
        }
        //++++++++++ OVERLOAD for AddMessage ++++++++++++++++++++++++++++
        // Overloads AddMessage to only require one argument(message)
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void AddMessage(string message)
        {
            Messages.Add(message);
            Colors.Add("normal");
            Display();
        }


        //----------- void ClearMessages() ------------------------------
        // Clears the messages from the screen and the lists
        //---------------------------------------------------------------
        public void ClearMessages()
        {
            Messages.Clear();
            Colors.Clear();
            ClearScreen();
        }


        //-------- void ClearScreen() ----------------------------------
        // Clears all output from all GUI objects
        //--------------------------------------------------------------
        void ClearScreen()
        {
            if (Guis.Count < MaxMessages)
            {
                //do nothing as we haven't created our guis yet
            }
            else
            {
                var x = 0;
                while (x < Guis.Count)
                {
                    var gui = (GameObject)Guis[x];
                    gui.GetComponent<GUIText>().text = "";
                    //increment and loop
                    x += 1;
                }
            }
        }


        //---------- void Prune() ---------------------------------------
        // Prunes the array to fit within the maxMessages limit
        //---------------------------------------------------------------
        void Prune()
        {
            int diff;
            if (Messages.Count > MaxMessages)
            {
                if (Messages.Count <= 0)
                {
                    diff = 0;
                }
                else
                {
                    diff = Messages.Count - MaxMessages;
                }
                Messages.RemoveRange(0, diff);
                Colors.RemoveRange(0, diff);
            }

        }

        //---------- void Display() -------------------------------------
        // Displays the list and handles coloring
        //---------------------------------------------------------------
        void Display()
        {
            //check if we are set to display
            if (Visible == false)
            {
                ClearScreen();
            }
            else if (Visible)
            {


                if (Messages.Count > MaxMessages)
                {
                    Prune();
                }

                // Carry on with display
                var x = 0;
                if (Guis.Count < MaxMessages)
                {
                    //do nothing as we havent created our guis yet
                }
                else
                {
                    while (x < Messages.Count)
                    {
                        var gui = (GameObject)Guis[x];

                        //set our color
                        switch ((string)Colors[x])
                        {
                            case "normal":
                                gui.GetComponent<GUIText>().material.color = Normal;
                                break;
                            case "warning":
                                gui.GetComponent<GUIText>().material.color = Warning;
                                break;
                            case "error":
                                gui.GetComponent<GUIText>().material.color = Error;
                                break;
                        }

                        //now set the text for this element
                        gui.GetComponent<GUIText>().text = (string)Messages[x];

                        //increment and loop
                        x += 1;
                    }
                }

            }
        }


    }
}// End DebugConsole Class