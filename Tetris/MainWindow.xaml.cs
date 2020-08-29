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
                var x = (int)((double)(cnv.Columns - Width) / 2);
                foreach (var cell in Cells)
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
                    if (cell.Column > max)
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
                return new Rect(x, y, width + 1, height + 1);
            }
            public void Rotate(Canvas cnv)
            {
                foreach (var cell in cnv.Cells)
                {
                    foreach (var cell2 in Cells)
                    {
                        if (cell.Row == cell2.Row)
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
                    rotated[index] = new Cell(1 * ((int)cell.Column - X) + Y, -1 * (cell.Row - Y) + X + (double)Width / 2);
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
                if (rectNext.X < 0)
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
                MyBrush = Brushes.MediumPurple;
            }
        }
        class ClevelandZ : Tetris
        {
            public ClevelandZ()
            {
                Cells = new List<Cell>() { new Cell(0, 0), new Cell(1, 0), new Cell(1, 1), new Cell(2, 1) };
                Width = 2;
                Height = 3;
                MyBrush = Brushes.LightSkyBlue;
            }
        }
        class RhodeIslandZ : Tetris
        {
            public RhodeIslandZ()
            {
                Cells = new List<Cell>() { new Cell(0, 1), new Cell(1, 1), new Cell(1, 0), new Cell(2, 0) };
                Width = 2;
                Height = 3;
                MyBrush = Brushes.LightSkyBlue;
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
        int Interval = 300;
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
                var poly2 = (Polygon)Cnvs2.Children[index];
                var poly3 = (Polygon)Cnvs3.Children[index];
                poly.Fill = Brushes.Transparent;
                poly2.Fill = Brushes.Transparent;
                poly3.Fill = Brushes.Transparent;
                index++;
            }
            foreach (var obj in Objects)
            {
                foreach (var cell in obj.Cells)
                {
                    var poly = (Polygon)Cnvs.Children[(int)cell.Column + (int)cell.Row * MainCnvs.Columns];
                    var poly2 = (Polygon)Cnvs2.Children[(int)cell.Column + (int)cell.Row * MainCnvs.Columns];
                    var poly3 = (Polygon)Cnvs3.Children[(int)cell.Column + (int)cell.Row * MainCnvs.Columns];
                    poly2.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                    poly3.Fill = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
                    poly.Fill = obj.MyBrush;
                }
            }
        }
        public void ClearLines()
        {
            var lines = 0;
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
                    lines++;
                    clear = true;                    
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
                    if (Interval >= 50)
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
            if (lines == 1)
            {
                ScoreCount += 100;
                Score.Content = "Score - " + ScoreCount;
            }
            if (lines == 2)
            {
                ScoreCount += 300;
                Score.Content = "Score - " + ScoreCount;
            }
            if (lines == 3)
            {
                ScoreCount += 700;
                Score.Content = "Score - " + ScoreCount;
            }
            if (lines == 4)
            {
                ScoreCount += 1500;
                Score.Content = "Score - " + ScoreCount;
            }
        }
        public bool StopDropping = false;
        public bool Dead = false;
        public void Restart()
        {
            this.KeyDown += MoveItem;
            Objects.Clear();
            CurrentTetris = new SmashBoy();
            Objects.Add(CurrentTetris);
            CurrentTetris.OnStop += CreateNewItem;
            CurrentTetris.MoveToCenter(MainCnvs);
            Timer.Start();
            Interval = 300;
            Timer.Interval = TimeSpan.FromSeconds(Interval / 1000.0);            
            for(int index=0;index<MainGrid.Children.Count;index++)
            {
                var isLabel = MainGrid.Children[index] as Label;
                if(isLabel!=null)
                {
                    MainGrid.Children.RemoveAt(index);
                    break;
                }
            }
            Dead = false;
        }
        public void Die()
        {
            bool dead = false;
            foreach (var obj in Objects)
            {
                foreach (var cell in obj.Cells)
                {
                    if (cell.Row == 0 && obj.Disabled)
                    {
                        dead = true;
                    }
                }
            }
            if (dead)
            {
                Timer.Stop();
                var label = new Label();
                label.Content = "Game Over";
                label.Name = "GameOverLabel";
                label.Foreground = new SolidColorBrush(Color.FromArgb(255, 138, 3, 3));
                var fontUri = new Uri(Environment.CurrentDirectory + "/" + "ShallowGraveBB.ttf");
                if(File.Exists(fontUri.AbsolutePath))
                {
                    label.FontFamily = new FontFamily(fontUri, "ShallowGrave BB");
                }                
                label.FontSize = 140;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.SetValue(Grid.RowProperty, 1);
                MainGrid.Children.Add(label);
                this.KeyDown -= MoveItem;
                Dead = true;
            }            
        }
        System.Media.SoundPlayer SoundPlayer;
        public MainWindow()
        {
            InitializeComponent();
            CurrentTetris = new SmashBoy();
            Objects.Add(CurrentTetris);
            CurrentTetris.OnStop += CreateNewItem;
            CurrentTetris.MoveToCenter(MainCnvs);
            Timer.Interval = TimeSpan.FromSeconds(Interval / 1000.0);
            Timer.Tick += UpdateScene;
            for (int row = 0; row < MainCnvs.Rows; row++)
            {
                for (int column = 0; column < MainCnvs.Columns; column++)
                {
                    var poly = new Polygon() { Points = new PointCollection(new Point[4] { new Point(column * CellA, row * CellA), new Point(column * CellA + CellA, row * CellA), new Point(column * CellA + CellA, row * CellA + CellA), new Point(column * CellA, row * CellA + CellA) }) };
                    int width = 10;
                    var w = CellA;
                    var h = CellA;
                    var poly2 = new Polygon() { Points = new PointCollection(new Point[6] {
                        new Point(w-width, h-width),
                        new Point(width,h-width),
                        new Point(0, h),
                        new Point(w , h),
                        new Point(w, 0),
                        new Point(w-width, width)
                    }) };
                    poly2.RenderTransform = new MatrixTransform(new Matrix(1, 0, 0, 1, column * CellA, row * CellA));                    
                    var poly3 = new Polygon() { Points = new PointCollection(new Point[4] { new Point(width, width), new Point(width, h- width), new Point(w - width, h - width), new Point(w - width, width) }) };     
                    poly3.RenderTransform = new MatrixTransform(new Matrix(1, 0, 0, 1, column * CellA, row * CellA));
                    poly2.Fill = Brushes.Transparent;
                    poly3.Fill = Brushes.Transparent;
                    Cnvs.Children.Add(poly);
                    Cnvs2.Children.Add(poly2);
                    Cnvs3.Children.Add(poly3);
                }
            }
            Timer.Start();            
            var window = Window.GetWindow(this);
            window.KeyDown += MoveItem;
            Cnvs.Width = (MainCnvs.Columns + 1) * CellA;
            Cnvs.Height = MainCnvs.Rows * CellA;
            Cnvs2.Width = (MainCnvs.Columns + 1) * CellA;
            Cnvs2.Height = MainCnvs.Rows * CellA;
            Cnvs3.Width = (MainCnvs.Columns + 1) * CellA;
            Cnvs3.Height = MainCnvs.Rows * CellA;
            for (int x = CellA; x < CellA * (MainCnvs.Columns+1); x += CellA)
            {
                var y1 = 0;
                var y2 = MainCnvs.Rows * CellA;
                var line = new Line();
                line.Stroke = Brushes.CornflowerBlue;
                line.Y1 = y1;
                line.Y2 = y2;
                line.X1 = x;
                line.X2 = x;
                if(x == CellA * (MainCnvs.Columns))
                {
                    line.Stroke = Brushes.Red;
                }
                Cnvs.Children.Add(line);
            }
            for (int y = CellA; y < CellA * MainCnvs.Rows; y += CellA)
            {
                var x1 = 0;
                var x2 = (MainCnvs.Columns + 1) * CellA;
                var line = new Line();
                line.Stroke = Brushes.CornflowerBlue;
                line.Y1 = y;
                line.Y2 = y;
                line.X1 = x1;
                line.X2 = x2;
                Cnvs.Children.Add(line);
            }
            var font1Uri = new Uri(Environment.CurrentDirectory + "/" + "NeonLights-22d.ttf");        
            if (File.Exists(font1Uri.AbsolutePath))
            {
                Title.FontFamily = new FontFamily(font1Uri, "Neon Lights");
                Score.FontFamily = new FontFamily(font1Uri, "Neon Lights");
            }                                 
            var music = Environment.CurrentDirectory + "/" + $"music{new Random().Next(3)}.wav";
            if (File.Exists(music))
            {
                SoundPlayer = new System.Media.SoundPlayer(music);
                SoundPlayer.Load();
               // SoundPlayer.PlayLooping();
            }
            else
            {
                Console.WriteLine("Music file was not found");
            }
        }
        public void CreateNewItem()
        {
            SetCells();
            Die();
            if(Dead)
            {
                return;
            }
            var rand = new Random().Next(7);
            if (rand == 0)
            {
                CurrentTetris = new SmashBoy();
            }
            else if (rand == 1)
            {
                CurrentTetris = new Hero();
            }
            else if (rand == 2)
            {
                CurrentTetris = new ClevelandZ();
            }
            else if (rand == 3)
            {
                CurrentTetris = new BlueRicky();
            }
            else if (rand == 4)
            {
                CurrentTetris = new RhodeIslandZ();
            }
            else if (rand == 5)
            {
                CurrentTetris = new Teewee();
            }
            else
            {
                CurrentTetris = new OrangeRicky();
            }
            StopDropping = true;
            Objects.Add(CurrentTetris);
            CurrentTetris.OnStop += CreateNewItem;
            CurrentTetris.MoveToCenter(MainCnvs);            
        }
        public void UpdateGraphics()
        {
            SetCells();
            SetCanvasCells();
            ClearLines();
            Die();
        }
        public void UpdateScene(object sender, EventArgs eventArgs)
        {
            SetCells();
            Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);            
            UpdateGraphics();
        }

        private void MoveItem(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                SetCells();               
                MoveLeft();
                UpdateGraphics();
            }
            if (e.Key == Key.Right)
            {
                SetCells();               
                MoveRight();
                UpdateGraphics();
            }
            if (e.Key == Key.R)
            {
                SetCells();
                Objects[Objects.Count - 1].Rotate(MainCnvs);
                UpdateGraphics();
            }
            if (e.Key == Key.Up)
            {
                SetCells();
                Objects[Objects.Count - 1].Rotate(MainCnvs);
                UpdateGraphics();
            }
            if (e.Key == Key.Down)
            {
                SetCells();
                SetCanvasCells();
                Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);
                UpdateGraphics();
            }
            if (e.Key == Key.Space)
            {
                StopDropping = false;
                for (; !StopDropping;)
                {
                    SetCells();                   
                    Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);
                    UpdateGraphics();
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

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Dead)
            {
                Restart();
            }
        }
    }
}
