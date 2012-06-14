using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Calculator.Models;
using System.Windows.Threading;
using System.Diagnostics;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int questToAns = 2; //要答 25 題

        int questLevel = 4;
        IList<Calculator.CalcItem> calcItems = new List<CalcItem>();
        IDictionary<int, Level> levels = new Dictionary<int, Level>();
        int maxResult = 9;
        int numCount = 3;
        int okCount = 0;
        int failCount = 0;
        int minutesToShow = 20; //可玩 20 分鐘
        private int skipCount;
        DateTime beginTime = DateTime.Now;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;

            
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.IsEnabled = false;

            InitLevels();

            //MakeQuestion(numCount, maxResult);
            MakeQuestion(4); //level 4

            Display();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                MessageBox.Show(string.Format("已經過了{0}分鐘囉，該休息一下了!!", minutesToShow));
                MessageBox.Show(string.Format("你還有 1 分鐘的時間把戰鬥結束", minutesToShow));
                WindowState = System.Windows.WindowState.Normal;
                dispatcherTimer.Interval = new TimeSpan(0, minutesToShow, 0);
            }else
            {
                dispatcherTimer.IsEnabled = false;
                WindowState = System.Windows.WindowState.Maximized;
                this.Show();
            }
        }

        private void InitLevels()
        {
            Random rnd = new Random();

            levels.Add(1, new Level()
            {
                LevelId = 1,
                Caption = "Level 1 小於5加法",
                MaxResult = 4,
                NumberCount = 2,
                NextNumber = delegate(int currResult)
                {
                    int n = 0;
                    if (currResult == 0) n = rnd.Next(maxResult);
                    else
                    {
                        n = rnd.Next(4);
                        while ((currResult + n) > maxResult){
                            n = rnd.Next(4);
                        }
                    }
                    return new CalcItem() { Number = n, Sign = 1 };
                }
            });

            levels.Add(2, new Level()
            {
                LevelId = 2,
                Caption = "Level 2 小於5減法",
                MaxResult = 4,
                NumberCount = 2,
                NextNumber = delegate(int currResult)
                {
                    int maxResult = 4;
                    int n = 0;
                    int sign = 1;
                    if (currResult == 0) n = rnd.Next(maxResult);
                    else
                    {
                        sign = -1;
                        n = rnd.Next(maxResult);
                        while ((currResult - n) < 0) 
                        {
                            n = rnd.Next(maxResult);
                        }
                    }
                    return new CalcItem() { Number = n, Sign = sign };
                }
            });

            levels.Add(3, new Level()
            {
                LevelId = 3,
                Caption = "Level 3 小於9加法",
                MaxResult = 4,
                NumberCount = 2,
                NextNumber = delegate(int currResult)
                {
                    int maxResult = 9;
                    int n = 0;
                    int sign = 1;
                    if (currResult == 0) n = rnd.Next(maxResult);
                    else
                    {
                        sign = 1;
                        n = rnd.Next(maxResult);
                        while ((currResult + n) < 0)
                        {
                            n = rnd.Next(maxResult);
                        }
                    }
                    return new CalcItem() { Number = n, Sign = sign };
                }
            });

            levels.Add(4, new Level()
            {
                LevelId = 4,
                Caption = "Level 4 2位數加1位數加淢法",
                MaxResult = 100,
                NumberCount = 2,
                NextNumber = delegate(int currResult)
                {
                    int maxResult = 100,
                        sign,
                        n;
                    if (currResult == 0)
                    {
                        n = rnd.Next(maxResult);
                        sign = 1;
                    }
                    else
                    {
                        sign = rnd.Next(2);
                        n = rnd.Next(maxResult) % 10; 
                        sign = (sign == 1 ? 1 : -1);
                    }
                    return new CalcItem() { Number = n, Sign = sign };
                }
            });
        }

        private void Display()
        {
            string s = "";
            foreach (var item in calcItems)
            {
                s += item.ToString() + Environment.NewLine;
            }

            lblQuestion.Content = s;
            textBox1.Text = "";
            textBox1.Focus();

            //show message
            var msg = string.Format("今天要總共要答 {0} 題", questToAns)+Environment.NewLine;
            msg += string.Format("現在答對 {0} 題", okCount)+Environment.NewLine;
            msg += string.Format("現在答錯 {0} 題", failCount)+Environment.NewLine;
            msg += string.Format("現在跳過 {0} 題", skipCount) + Environment.NewLine;

            var db = new PlayContext();
            var playCount = db.PlayRecords.Count();
            if (playCount != 0)
            {
                var play = db.PlayRecords.OrderByDescending(m => m.PlayRecordId).First();
                //var duration = (play.BeginTime - play.EndTime).ToString("mm:ss");
                msg += string.Format("這是你第 {0} 次玩囉", playCount + 1) + Environment.NewLine;
                msg += string.Format("上次你花了  {0:mm}分 {0:ss}秒", (play.BeginTime - play.EndTime)) + Environment.NewLine;
            }
            lblMsg.Content = msg;



            //make text invisible
            var da = new DoubleAnimation();
            da.From = 100;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromSeconds(3));
            label2.BeginAnimation(Label.FontSizeProperty, da);
        }

        private void CheckResult()
        {
            int result = 0;
            foreach (var item in calcItems)
            {
                result += item.Value;
            }

            int r;
            if (int.TryParse(textBox1.Text, out r) && (result == Convert.ToInt16(textBox1.Text)))
            {
                label2.Content = "答對了~~";
                MakeQuestion(questLevel);
                okCount++;
                Display();
            }
            else
            {
                label2.Content = "答錯了~~ 再來一次~";
                failCount++;
                Display();
            }

            if (okCount >= questToAns)
            {
                MessageBox.Show(string.Format("恭禧你!! 已經答對{0}題了。可以開始玩囉", okCount));

                SaveRecord();

                //設定 timer
                dispatcherTimer.Interval = new TimeSpan(0, minutesToShow, 0);
                dispatcherTimer.IsEnabled = true;
                WindowState = System.Windows.WindowState.Minimized;
                this.Hide();
            }
        }


        private void SaveRecord()
        {
            var db = new PlayContext();
            db.PlayRecords.Add(new PlayRecord()
            {
                BeginTime = beginTime,
                EndTime = DateTime.Now,
                OkCount = okCount,
                FailCount = failCount,
                SkipCount = skipCount
            });
            db.Save();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CheckResult();
        }

        private void MakeQuestion(int levelId)
        {
            var level = levels[levelId];
            var itemsToMake = level.NumberCount;
            var result = 0;
            calcItems.Clear();

            for (var i = 0; i < itemsToMake; i++)
            {
                var item = level.NextNumber(result);
                calcItems.Add(item);
                result += item.Value;
            }
        }

        //private void MakeQuestion(int itemsToMake, int max)
        //{
        //    IList<CalcItem> items = new List<CalcItem>();
        //    Random rnd = new Random();

        //    int result = 0;
        //    int sign = 1;
        //    int number = 0;

        //    for(var i = 0; i<itemsToMake; i++){
        //        number = rnd.Next(max - 1) + 1;
        //        sign = (i ==0)? 1 : (rnd.Next(2) == 1) ? 1: -1;

        //        while (((result + number*sign) > max) ||  ((result + number*sign) < 0))
        //        {
        //            number = rnd.Next(max - 1) + 1;
        //            sign = (rnd.Next(2) == 1) ? 1 : -1;
        //        }

        //        result += number * sign;

        //        items.Add(new CalcItem() { Number = number, Sign = sign });
        //    }

        //    calcItems.Clear();
        //    calcItems = items;
        //}

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            skipCount++;
            MakeQuestion(questLevel);
            Display();            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckResult();
            }
        }


    }
}
