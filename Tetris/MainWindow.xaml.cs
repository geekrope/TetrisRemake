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
using System.Windows.Threading;
using System.IO;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class Cell
        {
            public int Column
            {
                get; set;
            }
            public int Row
            {
                get; set;
            }           
            public bool Filled
            {
                get; set;
            }
            public Cell(int row, int column)
            {
                Row = row;
                Column = column;
            }
            public Cell(double row, double column)
            {
                Row = (int)Math.Ceiling(row);
                Column = (int)Math.Ceiling(column);
            }
        }
        class Canvas
        {
            public int Columns
            {
                get; set;
            }
            public int Rows
            {
                get; set;
            }
            public List<Cell> Cells
            {
                get; set;
            }
            public Canvas()
            {
                Columns = 10;
                Rows = 20;
                Cells = new List<Cell>();
                for (int row = 0; row < Rows; row++)
                {
                    for (int column = 0; column < Columns; column++)
                    {
                        Cells.Add(new Cell(row, column));
                    }
                }
            }
            public void ClearAllCells()
            {
                foreach (var cell in Cells)
                {
                    cell.Filled = false;
                }
            }
        }
        delegate void Stop();
        class Tetris
        {
            public Brush MyBrush
            {
                get; set;
            }
            public List<Cell> Cells
            {
                get; set;
            }
            public int Width
            {
                get; set;
            }
            public int Height
            {
                get; set;
            }
            public int X
            {
                get; set;
            }
            public bool Disabled
            {
                get; set;
            }
            public int Y
            {
                get; set;
            }
            public Stop OnStop
            {
                get; set;
            }     
            public void MoveToCenter(Canvas cnv)
            {
                var x = (int)((double)(cnv.Columns-Width)/2);
                foreach(var cell in Cells)
                {
                    cell.Column += x;
                }
                X = x;
            }
            protected double GetMaxX(Cell[] cells)
            {
                var max = double.MinValue;
                foreach (var cell in cells)
                {
                    if(cell.Column> max)
                    {
                        max = cell.Column;
                    }
                }
                return max;
            }
            protected double GetMaxY(Cell[] cells)
            {
                var max = double.MinValue;
                foreach (var cell in cells)
                {
                    if (cell.Row > max)
                    {
                        max = cell.Row;
                    }
                }
                return max;
            }
            protected double GetMinX(Cell[] cells)
            {
                var min = double.MaxValue;
                foreach (var cell in cells)
                {
                    if (cell.Column < min)
                    {
                        min = cell.Column;
                    }
                }
                return min;
            }
            protected double GetMinY(Cell[] cells)
            {
                var min = double.MaxValue;
                foreach (var cell in cells)
                {
                    if (cell.Row < min)
                    {
                        min = cell.Row;
                    }
                }
                return min;
            }
            public Rect GetBounds(Cell[] cells)
            {
                var x = GetMinX(cells);
                var y = GetMinY(cells);
                var width = GetMaxX(cells) - x;
                var height = GetMaxY(cells) - y;
                return new Rect(x,y,width+1,height + 1);
            }
            public void Rotate(Canvas cnv)
            {
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in Cells)
                    {
                        if(cell.Row == cell2.Row)
                        {
                            if (cell.Column == cell2.Column)
                            {
                                cell.Filled = false;
                            }
                        }
                    }
                }
                var canMove = true;
                var rotated = new Cell[Cells.Count];
                int index = 0;
                foreach (var cell in this.Cells)
                {                   
                    rotated[index] = new Cell(1 * ((int)cell.Column - X) + Y, -1*(cell.Row-Y) + X+(double)Width/2);                     
                    index++;
                }               
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in rotated)
                    {
                        if (cell.Column == cell2.Column && cell.Row == cell2.Row && cell.Filled)
                        {
                            canMove = false;
                        }
                    }
                }                                            
                var rectNext = GetBounds(rotated);                
                if (rectNext.X + rectNext.Width > cnv.Columns)
                {
                    canMove = false;
                }
                if (rectNext.Y + rectNext.Height > cnv.Rows)
                {
                    canMove = false;
                }
                if(rectNext.X<0)
                {
                    canMove = false;
                }
                if (rectNext.Y < 0)
                {
                    canMove = false;
                }
                if (canMove)
                {
                    X = (int)rectNext.X;
                    Y = (int)rectNext.Y;
                    Width = (int)rectNext.Width;
                    Height = (int)rectNext.Height;                    
                    Cells = rotated.ToList();                    
                }                
            }
            public void MoveLeft(Canvas cnv)
            {
                var copy = Cells.ToList();
                var canMove = true;
                foreach (var cell in this.Cells)
                {
                    var ind = cell.Column + (cell.Row) * cnv.Columns;
                    cnv.Cells[(int)ind].Filled = false;
                }
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in copy)
                    {
                        if (cell.Column == cell2.Column - 1 && cell.Row == cell2.Row && cell.Filled)
                        {
                            canMove = false;
                        }
                    }
                }
                if (this.X - 1 < 0)
                {
                    canMove = false;
                }
                if (canMove)
                {
                    foreach (var cell in copy)
                    {
                        cell.Column--;
                    }
                    this.Cells = copy;
                    X--;
                }
            }
            public void MoveRight(Canvas cnv)
            {
                var copy = Cells.ToList();
                var canMove = true;
                foreach (var cell in this.Cells)
                {
                    var ind = cell.Column + (cell.Row) * cnv.Columns;
                    cnv.Cells[(int)ind].Filled = false;
                }
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in copy)
                    {
                        if (cell.Column == cell2.Column + 1 && cell.Row == cell2.Row && cell.Filled)
                        {
                            canMove = false;
                        }
                    }
                }
                if (this.X + 1 + this.Width > cnv.Columns)
                {
                    canMove = false;
                }
                if (canMove)
                {
                    foreach (var cell in copy)
                    {
                        cell.Column++;
                    }
                    this.Cells = copy;
                    X++;
                }
            }
            public void MoveUp(Canvas cnv)
            {
                var copy = Cells.ToList();
                var canMove = true;
                foreach (var cell in this.Cells)
                {
                    var ind = cell.Column + (cell.Row) * cnv.Columns;
                    cnv.Cells[(int)ind].Filled = false;
                }
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in copy)
                    {
                        if (cell.Column == cell2.Column && cell.Row == cell2.Row - 1 && cell.Filled)
                        {
                            canMove = false;
                        }
                    }
                }
                if (this.Y - 1 < 0)
                {
                    canMove = false;
                }
                if (canMove)
                {
                    foreach (var cell in copy)
                    {
                        cell.Row--;
                    }
                    this.Cells = copy;
                    Y--;
                }

            }
            public void MoveDown(Canvas cnv, List<Tetris> objs)
            {
                var copy = Cells.ToList();
                var canMove = true;
                foreach (var cell in this.Cells)
                {
                    var ind = cell.Column + (cell.Row) * cnv.Columns;
                    cnv.Cells[(int)ind].Filled = false;
                }
                var index = objs.IndexOf(this);
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in Cells)
                    {
                        if (cell.Column == cell2.Column && cell.Row == cell2.Row + 1 && cell.Filled)
                        {
                            canMove = false;
                            if (OnStop != null && index == objs.Count - 1)
                            {
                                OnStop();                               
                            }
                            Disabled = true;
                        }
                    }
                }
                if (this.Y + 1 + this.Height > cnv.Rows)
                {
                    canMove = false;
                    if (OnStop != null && index == objs.Count - 1)
                    {
                        OnStop();
                        Disabled = true;
                    }
                }
                if (canMove && !Disabled)
                {
                    foreach (var cell in copy)
                    {
                        cell.Row++;
                    }
                    this.Cells = copy;
                    Y++;
                }
            }
        }
        class SmashBoy : Tetris
        {
            public SmashBoy()
            {
                Cells = new List<Cell>() { new Cell(0, 0), new Cell(1, 0), new Cell(1, 1), new Cell(0, 1) };
                Width = 2;
                Height = 2;
                MyBrush = Brushes.Red;
            }
        }
        class Hero : Tetris
        {
            public Hero()
            {
                Cells = new List<Cell>() { new Cell(0, 0), new Cell(0, 1), new Cell(0, 2), new Cell(0, 3) };
                Width = 4;
                Height = 1;
                MyBrush = Brushes.Purple;
            }
        }
        class ClevelandZ : Tetris
        {
            public ClevelandZ()
            {
                Cells = new List<Cell>() { new Cell(0, 0), new Cell(1, 0), new Cell(1, 1), new Cell(2, 1) };
                Width = 2;
                Height = 3;
                MyBrush = Brushes.Blue;
            }
        }
        class RhodeIslandZ : Tetris
        {
            public RhodeIslandZ()
            {
                Cells = new List<Cell>() { new Cell(0, 1), new Cell(1, 1), new Cell(1, 0), new Cell(2, 0) };
                Width = 2;
                Height = 3;
                MyBrush = Brushes.Blue;
            }
        }
        class Teewee : Tetris
        {
            public Teewee()
            {
                Cells = new List<Cell>() { new Cell(0, 0), new Cell(1, 0), new Cell(2, 0), new Cell(1, 1) };
                Width = 2;
                Height = 3;
                MyBrush = new SolidColorBrush(Color.FromArgb(255, 102, 255, 0));
            }
        }
        class OrangeRicky : Tetris
        {
            public OrangeRicky()
            {
                Cells = new List<Cell>() { new Cell(0, 1), new Cell(1, 1), new Cell(2, 1), new Cell(2, 0) };
                Width = 2;
                Height = 3;
                MyBrush = Brushes.Orange;
            }
        }
        class BlueRicky : Tetris
        {
            public BlueRicky()
            {
                Cells = new List<Cell>() { new Cell(0, 0), new Cell(1, 0), new Cell(1, 1), new Cell(1, 2) };
                Width = 3;
                Height = 2;
                MyBrush = Brushes.Orange;
            }
        }
        int Interval = 100;
        Canvas MainCnvs = new Canvas();
        Tetris CurrentTetris;
        List<Tetris> Objects = new List<Tetris>();
        DispatcherTimer Timer = new DispatcherTimer();
        int CellA = 50;
        int ScoreDelta = 10;
        int ScoreCount = 0;
        public void MoveLeft()
        {
            CurrentTetris.MoveLeft(MainCnvs);
            SetCells();
        }
        public void MoveRight()
        {
            CurrentTetris.MoveRight(MainCnvs);
            SetCells();
        }
        public void MoveUp()
        {
            CurrentTetris.MoveUp(MainCnvs);
            SetCells();
        }
        public void MoveDown()
        {
        }
        public void SetCells()
        {
            MainCnvs.ClearAllCells();
            foreach (var obj in Objects)
            {
                foreach (var cell in obj.Cells)
                {
                    var ind = cell.Column + (cell.Row) * MainCnvs.Columns;
                    MainCnvs.Cells[(int)ind].Filled = true;
                }
            }
        }
        public void SetCanvasCells()
        {
            int index = 0;
            foreach (var cell in MainCnvs.Cells)
            {
                var poly = (Polygon)Cnvs.Children[index];
                poly.Fill = Brushes.Transparent;
                index++;
            }
            foreach (var obj in Objects)
            {
                foreach (var cell in obj.Cells)
                {
                    var poly = (Polygon)Cnvs.Children[(int)cell.Column + (int)cell.Row * MainCnvs.Columns];
                    poly.Fill = obj.MyBrush;
                }
            }
        }
        public void ClearLines()
        {
            for (int row = 0; row <= MainCnvs.Rows; row++)
            {
                var clear = false;
                var colsFilled = 0;
                foreach (var obj in Objects)
                {
                    if (obj.Disabled)
                    {
                        foreach (var cell in obj.Cells)
                        {
                            if (cell.Row == row)
                            {
                                colsFilled++;
                            }
                        }
                    }
                }
                if (colsFilled == MainCnvs.Columns)
                {
                    clear = true;
                    ScoreCount += ScoreDelta;
                    Score.Content = "Score - " + ScoreCount;
                }
                for (int index1 = 0; index1 < Objects.Count; index1++)
                {
                    for (int index2 = 0; index2 < Objects[index1].Cells.Count; index2++)
                    {
                        if (Objects[index1].Cells[index2].Row == row && clear)
                        {
                            Objects[index1].Cells.RemoveAt(index2);
                            index2--;
                        }
                    }
                }
                if (clear)
                {
                    Interval -= 5;
                    if(Interval>=50)
                    {
                        Timer.Interval = new TimeSpan(Interval * 10000);
                    }                    
                    foreach (var obj in Objects)
                    {
                        if (obj.Disabled)
                        {
                            foreach (var cell in obj.Cells)
                            {
                                if (cell.Row < row)
                                {
                                    cell.Row++;
                                }
                            }
                        }
                    }
                }
            }
        }
        public bool StopDropping = false;        
        public void Die()
        {
            bool dead = false;
            foreach(var obj in Objects)
            {
                foreach(var cell in obj.Cells)
                {
                    if(cell.Row == 0&&obj.Disabled)
                    {
                        dead = true;
                    }
                }
            }
            if(dead)
            {
                Timer.Stop();
                var label = new Label();
                label.Content = "Game Over";
                label.Foreground = new SolidColorBrush(Color.FromArgb(255, 138, 3, 3));
                label.FontFamily = new FontFamily(new Uri(Environment.CurrentDirectory+"/" + "ShallowGraveBB.ttf"), "ShallowGrave BB");
                label.FontSize = 140;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.SetValue(Grid.RowProperty, 1);
                MainGrid.Children.Add(label);
            }
        }
        System.Media.SoundPlayer SoundPlayer;
        public MainWindow()
        {
            InitializeComponent();
            CurrentTetris = new SmashBoy();
            Objects.Add(CurrentTetris);
            Timer.Interval = new TimeSpan(100 * 10000);
            Timer.Tick += UpdateScene;
            for (int row = 0; row < MainCnvs.Rows; row++)
            {
                for (int column = 0; column < MainCnvs.Columns; column++)
                {
                    Cnvs.Children.Add(new Polygon() { Points = new PointCollection(new Point[4] { new Point(column * CellA, row * CellA), new Point(column * CellA + CellA, row * CellA), new Point(column * CellA + CellA, row * CellA + CellA), new Point(column * CellA, row * CellA + CellA) }) });
                }
            }
            Timer.Start();
            CurrentTetris.OnStop += CreateNewItem;
            var window = Window.GetWindow(this);
            window.KeyDown += MoveItem;
            Cnvs.Width = MainCnvs.Columns * CellA;
            Cnvs.Height = MainCnvs.Rows * CellA;
            for (int x = CellA; x < CellA * MainCnvs.Columns; x += CellA)
            {
                var y1 = 0;
                var y2 = MainCnvs.Rows * CellA;
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.Y1 = y1;
                line.Y2 = y2;
                line.X1 = x;
                line.X2 = x;
                Cnvs.Children.Add(line);
            }
            for (int y = CellA; y < CellA * MainCnvs.Rows; y += CellA)
            {
                var x1 = 0;
                var x2 = MainCnvs.Columns * CellA;
                var line = new Line();
                line.Stroke = Brushes.Black;
                line.Y1 = y;
                line.Y2 = y;
                line.X1 = x1;
                line.X2 = x2;
                Cnvs.Children.Add(line);
            }
            Title.FontFamily = new FontFamily(new Uri(Environment.CurrentDirectory + "/" + "NeonLights-22d.ttf"), "Neon Lights");
            Score.FontFamily = new FontFamily(new Uri(Environment.CurrentDirectory + "/" + "NeonLights-22d.ttf"), "Neon Lights");
            CurrentTetris.MoveToCenter(MainCnvs);
            SoundPlayer = new System.Media.SoundPlayer(Environment.CurrentDirectory + "/" + $"music{new Random().Next(3)}.wav");
            SoundPlayer.Load();
            System.Threading.Thread.Sleep(500);
            SoundPlayer.PlayLooping();
        }
        public void CreateNewItem()
        {
            var rand = new Random().Next(7);
            if (rand == 0)
            {
                CurrentTetris = new SmashBoy();
            }
            if (rand == 1)
            {
                CurrentTetris = new Hero();
            }
            if (rand == 2)
            {
                CurrentTetris = new ClevelandZ();
            }
            if (rand == 3)
            {
                CurrentTetris = new BlueRicky();
            }
            if (rand == 4)
            {
                CurrentTetris = new RhodeIslandZ();
            }
            if (rand == 5)
            {
                CurrentTetris = new Teewee();
            }
            if (rand == 6)
            {
                CurrentTetris = new OrangeRicky();
            }
            Objects.Add(CurrentTetris);
            CurrentTetris.OnStop += CreateNewItem;
            CurrentTetris.MoveToCenter(MainCnvs);
            StopDropping = true;
        }
        public void UpdateScene(object sender, EventArgs eventArgs)
        {
            Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);
            SetCells();
            SetCanvasCells();
            ClearLines();
            Die();
        }

        private void MoveItem(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MoveLeft();
            }
            if (e.Key == Key.Right)
            {
                MoveRight();
            }
            if (e.Key == Key.R)
            {
                Objects[Objects.Count - 1].Rotate(MainCnvs);
                SetCells();
                SetCanvasCells();
            }
            if (e.Key == Key.Down)
            {
                Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);
            }
            if (e.Key == Key.Space)
            {
                StopDropping = false;
                for (; !StopDropping; )
                {
                    Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);
                }                
            }
        }

        private void Exit_MouseEnter(object sender, MouseEventArgs e)
        {
            ExitFill.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            CloseLine1.Stroke = Brushes.White;
            CloseLine2.Stroke = Brushes.White;
        }

        private void Exit_MouseLeave(object sender, MouseEventArgs e)
        {
            ExitFill.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            CloseLine1.Stroke = Brushes.Red;
            CloseLine2.Stroke = Brushes.Red;
        }

        private void Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Cnvs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Timer.IsEnabled)
            {
                Timer.Stop();
            }
            else
            {
                Timer.Start();
            }
        }
    }
}
