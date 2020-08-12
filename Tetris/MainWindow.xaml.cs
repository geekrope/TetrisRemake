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
                Columns = 40;
                Rows = 40;
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
            protected Point ApplyTransform(Matrix matr,Point p)
            {
                var xNew = matr.M11 * p.X + matr.M21 * p.Y + matr.OffsetX;
                var yNew = matr.M12 * p.X + matr.M22 * p.Y + matr.OffsetY;
                return new Point(xNew, yNew);
            }
            public void Rotate()
            {
                foreach (var cell in this.Cells)
                {
                    var rowPrev = cell.Row-Y;
                    cell.Row = (cell.Column-X)+ Y;
                    cell.Column = rowPrev+X;
                    var widthPrev = Width;
                    Width = Height;
                    Height = widthPrev;
                }
            }
            public void MoveLeft(Canvas cnv)
            {
                var copy = Cells.ToList();
                var canMove = true;
                foreach (var cell in this.Cells)
                {
                    var ind = cell.Column + (cell.Row) * cnv.Columns;
                    cnv.Cells[ind].Filled = false;
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
                    cnv.Cells[ind].Filled = false;
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
                    cnv.Cells[ind].Filled = false;
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
                    cnv.Cells[ind].Filled = false;
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
                                Disabled = true;
                            }
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
                Cells = new List<Cell>() { new Cell(0,0), new Cell(1, 0), new Cell(2, 0), new Cell(1, 1) };
                Width = 2;
                Height = 3;
                MyBrush = new SolidColorBrush(Color.FromArgb(255,102,255,0));
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
        Canvas MainCnvs = new Canvas();
        Tetris CurrentTetris;
        List<Tetris> Objects = new List<Tetris>();
        DispatcherTimer Timer = new DispatcherTimer();
        int CellA = 25;
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
                    MainCnvs.Cells[ind].Filled = true;
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
                    var poly = (Polygon)Cnvs.Children[cell.Column+ cell.Row*MainCnvs.Columns];
                    poly.Fill= obj.MyBrush;                    
                }                
            }
        }
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
        }
        public void CreateNewItem()
        {
            var rand = new Random().Next(7);
            if(rand == 0)
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
        }
        public void UpdateScene(object sender, EventArgs eventArgs)
        {
            Objects[Objects.Count - 1].MoveDown(MainCnvs, Objects);
            SetCells();
            SetCanvasCells();
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
            if(e.Key == Key.R)
            {
                Objects[Objects.Count - 1].Rotate();
                SetCells();
                SetCanvasCells();
            }
        }
    }
}
