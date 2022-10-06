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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer Timer;
        private System.Windows.Threading.DispatcherTimer Clock;

        private int time;   // пройденное время игры (сек)

        private int Bonuses_Kol;   // пройденное время игры (сек)

        private bool LeftKeyHold;
        private bool RightKeyHold;  
        private double ShipVelocity;
        private List<Rectangle> Bricks;
        private List<Rectangle> Bonuses;
        public MainWindow()
        {
            InitializeComponent();
            Title = "00:00:00  BONUSES"+ Bonuses_Kol.ToString();
            Timer = new () { Interval = new TimeSpan(0, 0, 0, 0, 20)};
            Timer.Tick += this.Timer_Tick;
            Bricks = new();

            Clock = new() { Interval = new TimeSpan(0, 0, 0, 1) };
            Clock.Tick += this.ClockTick;

            Bricks = new();
            Bonuses = new();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // событие готовности окна работа елемнтами Ui(using interface)
            // реализовать в етом событие
            Timer.Start();
            time = 0;
            Clock.Start();
            ball.Tag = new BallDate { Vx = 2, Vy = -2 };
            ShipVelocity = 5;
           // Rectangle? rem = null;
            foreach (var child in Field.Children)
            {
                if (child is Rectangle rect)
                {
                    if (rect != Ship) Bricks.Add(rect);
                }
            }          
        }
        private void ClockTick(object? sender, EventArgs e)
        {
            ++time;
            int h = time / 3600;         // часы
            int m = (time % 3600) / 60;  // минуты
            int s = time % 60;           // секунды
            Title = h.ToString("00") + ":" +
                m.ToString("00") + ":" + s.ToString("00")+ "  BONUSES  : "+ Bonuses_Kol.ToString();;


        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            #region движ шарика

            Rectangle? rem = null;
            if (ball.Tag is BallDate balldata)
            {
                double ballX = Canvas.GetLeft(ball);//коод
                double ballY = Canvas.GetTop(ball);

                ballX += balldata.Vx;
                ballY += balldata.Vy;
                //
                if (ballX <= 0) // выход за левую грань
                {
                    balldata.Vx = -balldata.Vx;
                    ballX = 0;//подравниваем если вышел за хостл
                }
                if (ballY <= 0) // выход за левую грань
                {
                    balldata.Vy = -balldata.Vy;
                    ballY = 0;//подравниваем если вышел за хостл
                }
                if (ballX >= Field.ActualWidth - ball.ActualWidth)
                {
                    balldata.Vx = -balldata.Vx;
                    ballX = Field.ActualWidth - ball.ActualWidth;

                }

                if (ballY >= Field.ActualHeight - ball.ActualHeight / 2)
                {
                    Clock.Stop();
                    MessageBox.Show("GAME OVER");
                    Timer.Stop();  
                    Bonuses_Kol = 0;
                }
                if (ballY >= Canvas.GetTop(Ship) - ball.ActualHeight)
                {
                    if ((ballX + ball.ActualWidth / 2) >= Canvas.GetLeft(Ship)
                    && (ballX + ball.ActualWidth / 2) <= Canvas.GetLeft(Ship) + Ship.ActualWidth)
                    {
                        balldata.Vy = -balldata.Vy;
                        ballY = Canvas.GetTop(Ship) - ball.ActualHeight;
                    }
                }
                foreach (var brick in Bricks)
                {
                    double brickX = Canvas.GetLeft(brick);
                    double brickY = Canvas.GetTop(brick);
                    if (ballX + ball.ActualWidth / 2 >= brickX
                     && ballX + ball.ActualWidth / 2 <= brickX + brick.ActualWidth)
                    {
                        if (ballY + ball.ActualHeight >= brickY &&
                            ballY + ball.ActualHeight <= brickY + Math.Abs(balldata.Vy))
                        {
                            // Cверху
                            balldata.Vy = -balldata.Vy;
                            rem = brick;
                        }
                        if (ballY <= brickY + brick.ActualHeight &&
                            ballY >= brickY + brick.ActualHeight - Math.Abs(balldata.Vy))
                        {
                            // Снизу
                            balldata.Vy = -balldata.Vy;
                            rem = brick;
                        }
                    }
                    if (ballY + ball.ActualHeight / 2 >= brickY
                     && ballY + ball.ActualHeight / 2 <= brickY + brick.ActualHeight)
                    {
                        if (ballX + ball.ActualWidth >= brickX &&
                            ballX + ball.ActualWidth <= brickX + Math.Abs(balldata.Vx))
                        {
                            // левый
                            balldata.Vx = -balldata.Vx;
                            rem = brick;
                        }
                        if (ballX <= brickX + brick.ActualWidth &&
                            ballX >= brickX + brick.ActualWidth - Math.Abs(balldata.Vx))
                        {
                            // правый
                            balldata.Vx = -balldata.Vx;
                            rem = brick;
                        }
                    }
                }
                if (rem != null)
                {
                    var bonus = new Rectangle
                    {
                        Fill = rem.Fill,
                        Width = rem.Width,
                        Height = rem.Height
                    };
                    Bonuses.Add(bonus);
                    Field.Children.Add(bonus);
                    Canvas.SetLeft(bonus, Canvas.GetLeft(rem));
                    Canvas.SetTop(bonus, Canvas.GetTop(rem));

                    // удаляем блок из коллекции и с поля
                    Bricks.Remove(rem);
                    Field.Children.Remove(rem);
                }

                Canvas.SetLeft(ball, ballX);
                Canvas.SetTop(ball, ballY);
                foreach (var bonus in Bonuses)
                {
                    double by = Canvas.GetTop(bonus);
                    by += 3;
                    Canvas.SetTop(bonus, by);
                    if (Canvas.GetTop(bonus) >= Canvas.GetTop(Ship) - bonus.ActualHeight)
                    {
                        if ((Canvas.GetLeft(bonus) + bonus.ActualWidth / 2) >= Canvas.GetLeft(Ship)
                         && (Canvas.GetLeft(bonus) + bonus.ActualWidth / 2) <= Canvas.GetLeft(Ship) + Ship.ActualWidth)
                        {
                            Bonuses.Remove(bonus);
                            Field.Children.Remove(bonus);
                            Bonuses_Kol++;
                            break;
                        }
                    }
                    if (Canvas.GetTop(bonus) >= Field.ActualHeight - bonus.ActualHeight )
                    {
                        balldata.Vx++;
                        balldata.Vy++;
                        Bonuses.Remove(bonus);
                        Field.Children.Remove(bonus);
                        break;
                    }
                    /* Д.З. Арканоид: движение бонусов
                     * Обеспечить исчезновение бонусов 
                     * а) при пересечении с ракеткой
                     * б) при выходе за пределы поля
                     * ** Отобразить кол-во пойманных бонусов и время игры
                     */
                }



                #endregion
                #region karetka
                if (LeftKeyHold)
                {
                    double x = Canvas.GetLeft(Ship);
                    if (x > ShipVelocity) x -= ShipVelocity;
                    else x = 0;
                    Canvas.SetLeft(Ship, x);
                }
                if (RightKeyHold)
                {
                    double x = Canvas.GetLeft(Ship);
                    if (x < Field.ActualWidth - Ship.ActualWidth - ShipVelocity) x += ShipVelocity;
                    else x = Field.ActualWidth - Ship.ActualWidth;
                    Canvas.SetLeft(Ship, x);
                }
                #endregion
            }
        }

        // слабее чем наследование... агрегация
        class BallDate // данные шарика
        {
            public double Vx { get; set; }//скорость по вертикале
            public double Vy { get; set; } //по горизонтале
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Timer.Stop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Escape) this.Close();
            else if (e.Key== Key.Left)
            {
                this.LeftKeyHold=true;
            }
            else if (e.Key== Key.Right)
            {
                this.RightKeyHold=true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) this.LeftKeyHold = false;
            if (e.Key == Key.Right) this.RightKeyHold = false;
        }
    }
}
