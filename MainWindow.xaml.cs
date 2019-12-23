using model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace Minolovac

{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Object[,] mapa;
        private static readonly List<Position> openedPositions = new List<Position>();
        private static readonly List<Position> flaggedPositions = new List<Position>();
        private static readonly List<Position> bombPositions = new List<Position>();
        public static int numOfFlags;
        public static int time = 0;
        private static Difficility difficility;
        private static Boolean end = false;
        private object locker = new object();
        private static Random r = new Random();
        private static int row;
        private static int col;




        public MainWindow(Difficility difficility)
        {
            InitializeComponent();

            MainWindow.difficility = difficility;

            Setup();
            LoadGrid();

            NewGame();





        }

        private void LoadGrid()
        {

            for (int i = 0; i < col; i++)
            {

                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(20, GridUnitType.Auto);
                gridMain.ColumnDefinitions.Add(def);

            }


            for (int i = 0; i < row; i++)
            {

                RowDefinition def = new RowDefinition();
                def.Height = new GridLength(20, GridUnitType.Auto);
                gridMain.RowDefinitions.Add(def);

            }

        }

        private void Setup()
        {




            if (difficility.Equals(Difficility.EASY))
            {

                numOfFlags = 10;
                row = col = 10;
                mapa = new object[row, col];

            }
            else if (difficility.Equals(Difficility.INTERMEDIATE))
            {

                numOfFlags = 40;
                row = col = 17;
                mapa = new object[row, col];
            }
            else if (difficility.Equals(Difficility.HARD))
            {

                numOfFlags = 99;
                row = 17;
                col = 31;
                mapa = new object[row, col];
            }
            else
            {
                MessageBox.Show("An error occured!");
            }


            Grid.SetColumnSpan(top_layer, col);

        }

        private void GenerateMines()
        {




            HashSet<Position> positions = new HashSet<Position>();
            while (positions.Count < numOfFlags)
            {
                int x = r.Next(1, row);
                int y = r.Next(1, col);
                positions.Add(new Position(x, y));


            }

            foreach (Position p in positions)
            {

                mapa[p.getX(), p.getY()] = new Bomb();
                bombPositions.Add(new Position(p.getX(), p.getY()));

            }


        }

        private void CountMinesAndMapIt()
        {

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {

                    int numOfBombs = 0;
                    if (!(mapa[i, j] is Bomb))
                    {


                        if (j < (col - 1) && mapa[i, j + 1] is Bomb) numOfBombs++;
                        if (j > 0 && mapa[i, j - 1] is Bomb) numOfBombs++;
                        if (i < (row - 1) && mapa[i + 1, j] is Bomb) numOfBombs++;
                        if (i > 0 && mapa[i - 1, j] is Bomb) numOfBombs++;
                        if (i > 0 && j > 0 && mapa[i - 1, j - 1] is Bomb) numOfBombs++;
                        if (i > 0 && j < (col - 1) && mapa[i - 1, j + 1] is Bomb) numOfBombs++;
                        if (i < (row - 1) && j > 0 && mapa[i + 1, j - 1] is Bomb) numOfBombs++;
                        if (i < (row - 1) && j < (col - 1) && mapa[i + 1, j + 1] is Bomb) numOfBombs++;






                        if (numOfBombs != 0) { mapa[i, j] = numOfBombs; }




                    }

                }


        }


        private void OpenCell(object sender, RoutedEventArgs args)
        {

            Button b = (Button)sender;

            int x = Grid.GetRow(b);
            int y = Grid.GetColumn(b);
            Position p = new Position(x, y);

            if (!flaggedPositions.Contains(p) || !openedPositions.Contains(p))
            {
                SetImageForMapItem(x, y, b);
                openedPositions.Add(p);

                if (mapa[x, y] is Bomb)
                {

                    SetImageForUIItem(new Uri(@"resources/injured_emoji.png", UriKind.Relative), smile);
                    ShowMines();
                    SetImageForUIItem(new Uri(@"resources/mine_stepped.png", UriKind.Relative), b);
                    MessageBox.Show("You failed!");
                    GameOver();

                }
                else if (!(mapa[x, y] is Bomb) && !(mapa[x, y] is int)) OpenRandomFields(b);



            }




        }

        private void FlagCell(object sender, RoutedEventArgs args)
        {



            Button b = (Button)sender;
            int x = Grid.GetRow(b);
            int y = Grid.GetColumn(b);
            Position p = new Position(x, y);
            if (!openedPositions.Contains(p) && !flaggedPositions.Contains(p) && numOfFlags > 0)
            {

                flaggedPositions.Add(p);
                SetImageForUIItem(new Uri(@"resources/flag.png", UriKind.Relative), b);

                --numOfFlags;

                kk.Text = numOfFlags.ToString();
                CheckVictory();

            }
            else if (flaggedPositions.Contains(p))
            {
                flaggedPositions.Remove(p);
                SetImageForUIItem(new Uri(@"resources/cell.png", UriKind.Relative), b);
                ++numOfFlags;
                kk.Text = numOfFlags.ToString();


            }









        }

        private void GameOver()
        {
            end = true;





            Welcome welcomeWindow = new Welcome();
            welcomeWindow.Show();
            this.Close();
        }

        private void PopulateMap()
        {

            for (int i = 1; i < row; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    Button MyControl1 = new Button
                    {

                        Width = 30,
                        Height = 30,
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri(@"resources/cell.png", UriKind.Relative)),
                            Stretch = Stretch.Fill,
                            VerticalAlignment = VerticalAlignment.Center


                        }

                    };


                    MyControl1.Click += OpenCell;
                    MyControl1.MouseRightButtonDown += FlagCell;

                    gridMain.Children.Add(MyControl1);
                    Grid.SetRow(MyControl1, i);
                    Grid.SetColumn(MyControl1, j);


                }

            }
        }

        private void NewGame()
        {
            time = 0;
            lock (locker)
            {

                end = false;
            }

            Setup();
            bombPositions.Clear();
            openedPositions.Clear();
            flaggedPositions.Clear();
            SetImageForUIItem(new Uri(@"resources/smile_emoji.png", UriKind.Relative), this.smile);


            kk.Text = numOfFlags.ToString();
            GenerateMines();
            CountMinesAndMapIt();
            PopulateMap();


            Thread timer = new Thread(new ThreadStart(StartTimer));
            timer.Start();



        }

        private void CheckVictory()
        {

            if (bombPositions.Count == flaggedPositions.Count && Enumerable.SequenceEqual<Position>(flaggedPositions, bombPositions))
            {

                SetImageForUIItem(new Uri(@"resources/sunglasses_emoji.png", UriKind.Relative), this.smile);
                MessageBox.Show("Congrats, you solved Minesweeper");
                NewGame();



            }


        }

        private void StartTimer()
        {


            while (!end)
            {

                ui_time.Dispatcher.BeginInvoke(new Action(() =>
                {

                    ui_time.Text = (++time).ToString();

                }));





                Thread.Sleep(1000);



            }


        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            lock (locker)
            {
                end = true;
            }

        }

        private void OpenRandomFields(Button sender)
        {




            UIElementCollection collection = gridMain.Children;
            int index = collection.IndexOf(sender);
            int max = r.Next(index, collection.Count);





            for (int i = index; i < max; i++)
            {

                Button b = collection[i] as Button;
                int x = Grid.GetRow(b);
                int y = Grid.GetColumn(b);
                Position p = new Position(x, y);
                if (bombPositions.Contains(p)) break;
                else { SetImageForMapItem(x, y, b); openedPositions.Add(p); }


            }



        }

        private void SetImageForMapItem(int x, int y, Button b)
        {

            Image image = new Image();

            if (mapa[x, y] is Bomb)
            {


                image.Source = new BitmapImage(new Uri(@"resources/mine.png", UriKind.Relative));


            }
            else if (mapa[x, y] is int)
            {


                switch (mapa[x, y])
                {

                    case 1:
                        image.Source = new BitmapImage(new Uri(@"resources/1.png", UriKind.Relative));

                        break;
                    case 2:
                        image.Source = new BitmapImage(new Uri(@"resources/2.png", UriKind.Relative));
                        break;
                    case 3:
                        image.Source = new BitmapImage(new Uri(@"resources/3.png", UriKind.Relative));
                        break;
                    case 4:
                        image.Source = new BitmapImage(new Uri(@"resources/4.png", UriKind.Relative));
                        break;

                    case 5:
                        image.Source = new BitmapImage(new Uri(@"resources/5.png", UriKind.Relative));
                        break;
                    case 6:
                        image.Source = new BitmapImage(new Uri(@"resources/6.png", UriKind.Relative));
                        break;
                    case 7:
                        image.Source = new BitmapImage(new Uri(@"resources/7.png", UriKind.Relative));
                        break;
                    case 8:
                        image.Source = new BitmapImage(new Uri(@"resources/8.png", UriKind.Relative));
                        break;


                    default:
                        image.Source = new BitmapImage(new Uri(@"resources/cell_0.png", UriKind.Relative));
                        break;
                }


            }
            else
            {


                image.Source = new BitmapImage(new Uri(@"resources/cell_0.png", UriKind.Relative));

            }

            b.Content = image;

        }

        private void SetImageForUIItem(Uri uri, Button b)
        {

            Image img = new Image();
            img.Source = new BitmapImage(uri);
            b.Content = img;




        }

        private void ShowMines()
        {

            UIElementCollection collection = gridMain.Children;
            foreach (UIElement element in collection)
            {

                if (element is Button)
                {

                    Button b = element as Button;
                    int y = Grid.GetColumn(b);
                    int x = Grid.GetRow(b);
                    Position p = new Position(x, y);
                    if (flaggedPositions.Contains(p) && bombPositions.Contains(p)) SetImageForUIItem(new Uri(@"resources/mine_crossed.png", UriKind.Relative), b);

                    else if (bombPositions.Contains(new Position(x, y))) SetImageForUIItem(new Uri(@"resources/mine.png", UriKind.Relative), b);


                }


            }


        }


    }
}
