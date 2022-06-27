using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int squareSize = 20;
        private Random rnd = new Random();
        HashSet<int> mines = new HashSet<int>();
        bool firstClick = true;
        bool gameOver = false;
        private int minesAmount = 40;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Window_ContentRendered(object sender, EventArgs e)
        {          
            DrawGameArea();
        }
        private void DrawGameArea()
        {
            bool isDone = false;
            int nextX = 0, nextY = 0;
            while(isDone == false)
            {           
                Rectangle sqr = new Rectangle
                {
                    Width = squareSize,
                    Height = squareSize,
                    Fill = Brushes.LightGray
                };
                GameArea.Children.Add(sqr);
                Canvas.SetTop(sqr, nextY);
                Canvas.SetLeft(sqr, nextX);               
                nextX += squareSize;
                if(nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;                    
                    nextY += squareSize;
                }
                if (nextY >= GameArea.ActualHeight)
                    isDone = true;               
            }          
        }
        private void DrawMines(int index)
        {
            GenerateMines(index);
            bool isDone = false;
            int nextX = 0, nextY = 0, counter = 0;
            while (isDone == false)
            {
                if (mines.Contains(counter))
                {
                    Ellipse mine = new Ellipse()
                    {
                        Width = squareSize,
                        Height = squareSize,
                        Fill = Brushes.Black
                    };
                    GameArea.Children.Add(mine);
                    Canvas.SetZIndex(mine, -1);
                    Canvas.SetTop(mine, nextY);
                    Canvas.SetLeft(mine, nextX);
                }
                nextX += squareSize;
                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += squareSize;
                }
                if (nextY >= GameArea.ActualHeight)
                    isDone = true;
                counter++;
            }
        }
        private void GenerateMines(int index)
        {
            int x;
            HashSet<int> noMines = new HashSet<int>();
            noMines.Add(index);
            if (index % 16 == 0) 
            {
                noMines.Add(index - 16);
                noMines.Add(index - 15);
                noMines.Add(index + 1);
                noMines.Add(index + 16);
                noMines.Add(index + 17);
            }
            else if(index % 16 == 15)
            {
                noMines.Add(index - 17);
                noMines.Add(index - 16);
                noMines.Add(index - 1);
                noMines.Add(index + 15);
                noMines.Add(index + 16);
            }
            else
            {
                noMines.Add(index - 17);
                noMines.Add(index - 16);
                noMines.Add(index - 15);
                noMines.Add(index - 1);
                noMines.Add(index + 1);
                noMines.Add(index + 15);
                noMines.Add(index + 16);
                noMines.Add(index + 17);
            }
            while(mines.Count <= 40)
            {
                x = rnd.Next(0, 256);
                if(noMines.Contains(x)==false)
                    mines.Add(x);
            }
        }
        public void Click(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is Rectangle && gameOver == false)
            {
                int index = GameArea.Children.IndexOf((Rectangle)e.OriginalSource);
                if(firstClick == true)
                {
                    DrawMines(index);
                    firstClick = false;
                }
                Uncover(index);        
            }     
        }
        private void Uncover(int index)
        {
            if (index >= 0 && index <= 255)
            {
                Rectangle rect = (Rectangle)GameArea.Children[index];
                if (rect.Fill != Brushes.Transparent)
                {
                    if (rect.Fill != Brushes.Red)
                    {
                        rect.Fill = Brushes.Transparent;
                        double top = (double)rect.GetValue(Canvas.TopProperty);
                        double left = (double)rect.GetValue(Canvas.LeftProperty);
                        TextBlock txt = new TextBlock()
                        {
                            Text = MinesNerby(index),
                            Width = 20,
                            Height = 20,
                            TextAlignment = TextAlignment.Center
                        };
                        Canvas.SetTop(txt, top);
                        Canvas.SetLeft(txt, left);
                        GameArea.Children.Add(txt);
                    }
                } 
            }            
        }
        public void Flag(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is Rectangle && gameOver == false)
            {
                Rectangle clicked = (Rectangle)e.OriginalSource;
                if (clicked.Fill == Brushes.LightGray)
                {
                    clicked.Fill = Brushes.Red;
                    minesAmount--;
                } 
                else
                {
                    clicked.Fill = Brushes.LightGray;
                    minesAmount++;
                }
                minesLeft.Text = minesAmount.ToString();
            }
        }
        private string MinesNerby(int pos)
        {
            int counter = 0;
            if(mines.Contains(pos))
            {
                gameOver = true;
                return "";
            }
            if (pos % 16 == 0)
            {
                if (mines.Contains(pos - 16)) counter++;
                if (mines.Contains(pos - 15)) counter++;
                if (mines.Contains(pos + 1)) counter++;
                if (mines.Contains(pos + 16)) counter++;
                if (mines.Contains(pos + 17)) counter++;
            }
            else if (pos % 16 == 15) 
            {
                if (mines.Contains(pos - 17)) counter++;
                if (mines.Contains(pos - 16)) counter++;
                if (mines.Contains(pos - 1)) counter++;
                if (mines.Contains(pos + 15)) counter++;
                if (mines.Contains(pos + 16)) counter++;
            }
            else
            {
                if (mines.Contains(pos - 17)) counter++;
                if (mines.Contains(pos - 16)) counter++;
                if (mines.Contains(pos - 15)) counter++;
                if (mines.Contains(pos - 1)) counter++;
                if (mines.Contains(pos + 1)) counter++;
                if (mines.Contains(pos + 15)) counter++;
                if (mines.Contains(pos + 16)) counter++;
                if (mines.Contains(pos + 17)) counter++;
            }
            if(counter == 0)
            {
                if (pos % 16 == 0) 
                {
                    Uncover(pos - 16);
                    Uncover(pos - 15);
                    Uncover(pos + 1);
                    Uncover(pos + 16);
                    Uncover(pos + 17);
                }
                else if (pos % 16 == 15)
                {
                    Uncover(pos - 17);
                    Uncover(pos - 16);
                    Uncover(pos - 1);
                    Uncover(pos + 15);
                    Uncover(pos + 16);
                }
                else
                {
                    Uncover(pos - 17);
                    Uncover(pos - 16);
                    Uncover(pos - 15);
                    Uncover(pos - 1);
                    Uncover(pos + 1);
                    Uncover(pos + 15);
                    Uncover(pos + 16);
                    Uncover(pos + 17);
                }
                return "";
            }
            return counter.ToString();
        }
        public void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void Window_Move(object sender, EventArgs e)
        {
            this.DragMove();
        }
        public void Reset(object sender, EventArgs e)
        {
            gameOver = false;
            firstClick = true;
            mines.Clear();
            int x = GameArea.Children.Count;
            for(int i = x-1; i>255; i--)
            {
                GameArea.Children.RemoveAt(i);
            }
            foreach(Rectangle rect in GameArea.Children)
            {
                rect.Fill = Brushes.LightGray;
            }
        }
    }
}
