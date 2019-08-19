using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Shell;

namespace GameBoy
{
    enum State
    {
        rightFoot, stand, leftFoot, handOn, handOff
    }
    enum Direction
    {
        up, down, left, right
    }



    public partial class Form1 : Form
    {
        const int step = 4;//how many pixels does the player move by a step (of the foot)
        Rectangle[,] images = new Rectangle[4, 3]{/*down*/{new Rectangle(0, 0, 22, 25)/*stand*/, new Rectangle(23, 0, 22, 25)/*left leg*/, new Rectangle(46, 0, 22, 25)/*right leg*/},
                                         /*left or right*/{new Rectangle(0, 26, 22, 25)/*stand*/, new Rectangle(23, 26, 22, 25)/*left leg*/, new Rectangle(46, 26, 22, 25)/*right leg*/},
                                                    /*up*/{new Rectangle(0, 52, 22, 25)/*stand*/, new Rectangle(23, 52, 22, 25)/*left leg*/, new Rectangle(46, 52, 22, 25)/*right leg*/},
                                             /*hand-down*/{new Rectangle(0, 78, 22, 26)/*on*/, new Rectangle(23, 78, 22, 26)/*off*/,  new Rectangle(0, 0, 22, 25)/*stand*/} };
        //------------------------------------------------------------------------------------------------------------------
        State mainS;//the main state of the player
        private int stating = 0;//the indexing number which points to the right image to put in player
        private bool isSpaceDown = true/*indicats if the SPACE bar is down*/, canMakeAMove = true;//indicates if the player can make a move by the moment
        Direction mainD;// the main direction of the player
        Timer move = new Timer();// the timer who is in charge of the steps of the player
        Timer handing = new Timer();//the timer who is in charge of the לנופף לשלום
        public Form1()
        {
            InitializeComponent();
            TopMost = true;//sets the form to allways apeare at the front of the screen.
            ShowInTaskbar = false;//hides the icon in the taskbar

            //set back image
            BackGround.Size = Properties.Resources.back.Size;//set the size of the picturebox to the background's image size.
            BackGround.Height--;//decrese the height of the background by one to get ridd of a bug.

            //set form
            Size = BackGround.Size;// set the form's size to the background's picturebox size


            //set timers
            move.Interval = 100;//set the interval of the timer to 100 miliseceonds
            move.Tick += new EventHandler(Move_Tick);//making the Move_Tick method as the timer's Tick event
            handing.Interval = 500;//set the interval of the timer to 500 miliseceonds
            handing.Tick += new EventHandler(Handing_Tick);//making the Move_Tick method as the timer's Tick event


            //set player
            mainD = Direction.down;// set the diraction of the player to face down
            mainS = State.stand;// set the state of the player to stand
            player.Left = 0;// place the player's left to left
            player.Top = Size.Height - player.Height + 1;// set the player's top to the bottom of the bottom of the form
            player.BackColor = Color.Transparent;// set the back color of the player to transparent
            player.Image = GetPlayer(mainD, mainS);// set the image of the player.
            player.BackgroundImage = GetBackGround(player); //set the backimage of the player to the part of the image behind him.
        }

        private void Handing_Tick(object sender, EventArgs e)
        {
            //this method controlls the handing operation
            if (stating % 2 == 0)
            {
                // if stating is even set the player's image to the handon state with the down diraction
                player.Image = GetPlayer(Direction.down, State.handOn);
            }
            else
            {
                // if stating is not even set the player's image to the handoff state with the down diraction
                player.Image = GetPlayer(Direction.down, State.handOff);
            }
            player.BackgroundImage = GetBackGround(player);//set the backimage of the player to the part of the image behind him.
            stating++;//continue counting
            if (stating > 4 && !isSpaceDown && stating % 2 == 0)
            {
                //if you have waved more then two times, the user stoped pressing the SPACE bar, and the player is at the handon possition then:
                handing.Enabled = false;//stop the timer
                canMakeAMove = true;//enable the user to start a new move
                player.Image = GetPlayer(Direction.down, State.stand);// set the player to stand down
                stating = 0;// reset the counting.
            }
        }

        private void Move_Tick(object sender, EventArgs e)
        {
            //this method controlls the moveing operation
            //the one movment have 4 steps: right, stand, left, stand. we will use the enum properties to count by the 4th base.
            switch (stating % 4)
            {
                case 0:
                    {
                        mainS = 0;
                        player.Image = GetPlayer(mainD, mainS);
                        break;
                    }
                case 1:
                    {
                        mainS = (State)1;
                        player.Image = GetPlayer(mainD, mainS);
                        break;
                    }
                case 2:
                    {
                        mainS = (State)2;
                        player.Image = GetPlayer(mainD, mainS);
                        break;
                    }
                case 3:
                    {
                        mainS = (State)1;
                        player.Image = GetPlayer(mainD, mainS);
                        break;
                    }
            }

            switch (mainD)//move the player to the side he is facing
            {
                case Direction.up:
                    {
                        player.Top -= step;
                        break;
                    }
                case Direction.down:
                    {
                        player.Top += step;
                        break;
                    }
                case Direction.left:
                    {
                        player.Left -= step;
                        break;
                    }
                case Direction.right:
                    {
                        player.Left += step;
                        break;
                    }
            }
            //after every step we will set the backgound image of the player to the background image's area he is on
            player.BackgroundImage = GetBackGround(player);

            stating++;// continue the counting
            if (stating > 3)
            {
                // if you have made 4 steps then:
                canMakeAMove = true;//enable the user to make a new move
                move.Enabled = false;// disable the timer
                stating = 0;// reset the count
            }
        }

        private Bitmap GetPlayer(Direction direction, State state)
        {
            //this method takes the desire diraction and state, and returns the part of the player's full image that contains the desire part
            Bitmap myBitmap = new Bitmap(Properties.Resources.pika);// get the full image
            int row = 0, col = 0;// declare two indexers to the row and colomn

            //set row
            switch (direction)
            {
                case Direction.up:
                    {
                        row = 2;
                        break;
                    }

                case Direction.down:
                    {
                        row = 0;
                        break;
                    }
                case Direction.left:
                    {
                        row = 1;
                        break;
                    }
                case Direction.right:
                    {
                        row = 1;
                        break;
                    }
            }

            //set column
            switch (state)
            {
                case State.stand:
                    {
                        col = 0;
                        break;
                    }
                case State.leftFoot:
                    {
                        col = 1;
                        break;
                    }
                case State.rightFoot:
                    {
                        col = 2;
                        break;
                    }
                case State.handOn:
                    {
                        //at the handing, the diraction will always be down
                        col = 1;
                        row = 3;
                        mainD = Direction.down;
                        break;
                    }
                case State.handOff:
                    {
                        //at the handing, the diraction will always be down
                        col = 0;
                        row = 3;
                        mainD = Direction.down;
                        break;
                    }
            }

            Bitmap cloneBitmap = myBitmap.Clone(images[row, col], myBitmap.PixelFormat);// get the part of the image by the rectangle at images[row,col]
                                                                                        //which we had set at the beginning
            if (direction == Direction.right)
            {
                //because we have only up, down, left and hand diractions at the full image, then we are gonna mirror the left image and get the right side 
                cloneBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            return cloneBitmap;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //this method bridges the user's keys and the player
            switch (e.KeyCode)//if the key is one of the arrows or the SPACE bar then.... but if it is not, then don't do anything
            {
                case Keys.Up:
                    {
                        if (canMakeAMove && CanMakeMove(Direction.up, player, (Point)Size))//if the player is not in a move and he can finnish the move without geiing out of the frame
                        {
                            mainD = Direction.up;
                            move.Enabled = true;
                            canMakeAMove = false;
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        if (canMakeAMove && CanMakeMove(Direction.down, player, (Point)Size))//if the player is not in a move and he can finnish the move without geiing out of the frame
                        {
                            mainD = Direction.down;
                            move.Enabled = true;
                            canMakeAMove = false;
                        }
                        break;
                    }
                case Keys.Left:
                    {
                        if (canMakeAMove && CanMakeMove(Direction.left, player, (Point)Size))//if the player is not in a move and he can finnish the move without geiing out of the frame
                        {
                            mainD = Direction.left;
                            move.Enabled = true;
                            canMakeAMove = false;
                        }
                        break;
                    }
                case Keys.Right:
                    {
                        if (canMakeAMove && CanMakeMove(Direction.right, player, (Point)Size))//if the player is not in a move and he can finnish the move without geiing out of the frame
                        {
                            mainD = Direction.right;
                            move.Enabled = true;
                            canMakeAMove = false;
                        }
                        break;
                    }
                case Keys.Space:
                    {
                        if (canMakeAMove)//if the player is not in a move
                        {
                            isSpaceDown = true;
                            mainS = State.handOff;
                            handing.Enabled = true;
                            canMakeAMove = false;
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //this method detect's when is the user stops pushing the SPACE bar
            if (e.KeyData == Keys.Space)
            {
                //if it has, then turn isSpaceDown to false.
                isSpaceDown = false;
            }
        }

        private bool CanMakeMove(Direction d, Control player, Point max)
        {
            //this method predict if the player could make the move without getting out of the frame
            switch (d)
            {
                case Direction.up:
                    return (player.Top - step * 4) >= 0;
                case Direction.down:
                    return (player.Bottom + step * 4) <= max.Y;
                case Direction.left:
                    return (player.Left - step * 4) >= 0;
                case Direction.right:
                    return (player.Right + step * 4) <= max.X;
                default:
                    return true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //setting a close option for the application
            Application.Exit();
        }

        private Bitmap GetBackGround(Control player)
        {
            //this method makes a rectangle py the player's place and size and cuts a rectange with an image from the background image from the same place and return it.
            return Properties.Resources.back.Clone(new Rectangle(player.Location, player.Size), Properties.Resources.back.PixelFormat);//cloneBitmap;
        }
    }
}
