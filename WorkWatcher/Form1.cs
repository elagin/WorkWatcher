using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace WorkWatcher
{

    public partial class Form1 : Form
    {
        static Timer myTimer = new Timer();
        //static bool exitFlag = false;

        public class MyEvent
        {
            public DateTime time;
            public int type;

            public MyEvent(long type, DateTime time)
            {
                this.type = unchecked((int)type);
                this.time = time;
            }
        }

        public class Session
        {
            public DateTime start;
            public DateTime end;
        }

        public class Day
        {
            public DateTime start;
            public DateTime end;
        }

        public class Week
        {
            public List<Day> dayList;
            public Week()
            {
                dayList = new List<Day>();
            }
        }

        public class Month
        {
            public List<Week> weekList;

            public Month()
            {
                weekList = new List<Week>();
            }
        }

        static int hourPerDay = 8;
        static Form1 myForm = null;

        static DateTime dateSatrtWeek = DateTime.Today;
        static Month month = new Month();

        public Form1()
        {
            InitializeComponent();

            /* Adds the event and the event handler for the method that will process the timer event to the timer. */
            myTimer.Tick += new EventHandler(TimerEventProcessor);

            // Sets the timer interval to 5 seconds.
            myTimer.Interval = 60 * 1000;
            myTimer.Start();

            listView.View = View.Details;
            // Allow the user to edit item text.
            //            listView.LabelEdit = true;
            // Allow the user to rearrange columns.
            listView.AllowColumnReorder = true;
            // Display check boxes.
            //            listView.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            listView.FullRowSelect = true;
            // Display grid lines.
            listView.GridLines = true;
            // Sort the items in the list in ascending order.

            // Create columns for the items and subitems.
            // Width of -2 indicates auto-size.
            listView.Columns.Add("Дата", -2, HorizontalAlignment.Left);
            listView.Columns.Add("Начало", -2, HorizontalAlignment.Right);
            listView.Columns.Add("Окончание", -2, HorizontalAlignment.Right);
            listView.Columns.Add("Всего", -2, HorizontalAlignment.Right);
            listView.Columns.Add("Дельта", -2, HorizontalAlignment.Right);
            myForm = this;
            work();
        }

        // This is the method to run when the timer is raised.
        private static void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            work();
            myTimer.Enabled = true;
        }

        private static void work()
        {
            month.weekList.Clear();
            myForm.listView.Items.Clear();

            dateSatrtWeek = DateTime.Today;
            TimeSpan totalWork = new TimeSpan(0);

            getStartWeek();
            myForm.textBoxUserName.Text = Environment.UserDomainName + "\\" + Environment.UserName;

            List<MyEvent> eventList = Start();
            parseEventList3(eventList);
            myForm.textBoxEventCount.Text = String.Format("Обнаружено {0} событий.", eventList.Count);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt"))
            {
                foreach (Week week in month.weekList)
                {
                    foreach (Day day in week.dayList)
                    {
                        TimeSpan diff = day.end - day.start;
                        TimeSpan total = new TimeSpan(diff.Hours - hourPerDay, diff.Minutes, diff.Seconds);
                        string[] row = { day.start.ToShortDateString(), day.start.ToShortTimeString(), day.end.ToShortTimeString(), diff.ToString(@"hh\:mm"), total.ToString() };
                        var listViewItem = new ListViewItem(row);
                        myForm.listView.Items.Add(listViewItem);
                        TimeSpan spanDay = new TimeSpan(diff.Hours, diff.Minutes, diff.Seconds);
                        totalWork = totalWork.Add(spanDay);
                    }
                }
            }

            myForm.textBoxTotalWork.Text = ((int)totalWork.TotalHours).ToString() + ":" + totalWork.Minutes.ToString();
            TimeSpan timeLeft = new TimeSpan(hourPerDay * 4, 0, 0) - totalWork;
            myForm.textBoxTotalWait.Text = timeLeft.ToString(@"hh\:mm");
        }

        private static bool isFoundByUsername(EventLogEntry entry, string userName)
        {
            if (entry.UserName != null)
            {
                return entry.UserName.Equals(userName);
            }
            else
            {
                foreach (string item in entry.ReplacementStrings)
                {
                    if (item.Equals(userName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private DateTime startOfDay(DateTime value)
        {
            DateTime res = value.AddHours(-value.Hour).AddMinutes(-value.Minute).AddSeconds(-value.Second);
            return res;
        }

        private static void parseEventList3(List<MyEvent> eventList)
        {
            Session currentSession = new Session();
            //for (i = ev.Entries.Count - 1; i >= 0; i--)
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt"))
            {
                Week week = null;
                Day day = null;
                MyEvent prevEvent = null;
                foreach (MyEvent item in eventList)
                {
                    //file.WriteLine(String.Format("Событие.{0};{1}", item.type, item.time));
                    if (day == null)
                    {
                        day = new Day();
                        if (item.time.Day == 15)
                        {
                            item.time = item.time.AddHours(7).AddMinutes(39);
                        }
                        day.start = item.time;
                    }
                    else
                    {
                        //Изменился день
                        if (myForm.startOfDay(day.start) != myForm.startOfDay(item.time))
                        {
                            day.end = prevEvent.time;
                            if (week == null)
                            {
                                week = new Week();
                            }
                            week.dayList.Add(day);
                            //file.WriteLine(String.Format("---- Смена суток ---- {0}", day.end - day.start));
                            day = null;
                        }
                    }

                    if (item.Equals(eventList[eventList.Count - 1])) // Последняя запись, нужно завершить день
                    {
                        file.WriteLine(String.Format("Смена суток или последяя запись;{0}", item.time));
                        if (myForm.startOfDay(day.start) == myForm.startOfDay(item.time))
                        {
                            DateTime now = DateTime.Now;
                            day.end = now.AddMilliseconds(-now.Millisecond); ; //Мы еще на работе.
                            if (week == null)
                            {
                                week = new Week();
                            }
                            week.dayList.Add(day);
                            file.WriteLine(String.Format("---- Смена суток ---- {0}", day.end - day.start));
                            day = null;
                        }
                    }
                    prevEvent = item;
                }
                month.weekList.Add(week);
            }
        }

        private void parseEventList2()
        {
            List<MyEvent> eventList = new List<MyEvent>();
            Session currentSession = new Session();
            //for (i = ev.Entries.Count - 1; i >= 0; i--)
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt"))
            {
                Week week = null;
                Day day = null;
                MyEvent prevEvent = null;
                foreach (MyEvent item in eventList)
                {
                    switch (item.type)
                    {
                        case 4801:
                        //file.WriteLine(String.Format("Разблокирована рабочая станция.;{0}", item.time));
                        case 4800:
                            //file.WriteLine(String.Format("Разблокирована рабочая станция.;{0}", item.time));
                            file.WriteLine(String.Format("Событие.{0};{1}", item.type, item.time));
                            if (day == null)
                            {
                                day = new Day();
                                day.start = item.time;
                            }
                            else
                            {
                                //Изменился день или последняя запись
                                if (startOfDay(day.start) != startOfDay(item.time))
                                {
                                    //file.WriteLine(String.Format("Смена суток или последяя запись;{0}", item.time));
                                    day.end = prevEvent.time;
                                    if (week == null)
                                    {
                                        week = new Week();
                                    }
                                    week.dayList.Add(day);
                                    day = null;
                                }
                            }
                            break;
/*
                        case 4800:
                            file.WriteLine(String.Format("Заблокирована рабочая станция.;{0}", item.time));
                            if (day == null)
                            {
                                day = new Day();
                                day.start = item.time;
                            }
                            else
                            {
                                day.end = item.time;
                                if (week == null)
                                {
                                    week = new Week();
                                    week.dayList.Add(day);
                                    day = null;
                                }
                            }
                            break;
*/
/*
                        case 4648:
                            file.WriteLine(String.Format("Выполнена попытка входа в систему с явным указанием учетных данных.;{0}", item.time));
                            break;
                        case 4624:
                            file.WriteLine(String.Format("Вход в учетную запись выполнен успешно.;{0}", item.time));
                            break;
                        case 4672:
                            file.WriteLine(String.Format("Новому сеансу входа назначены специальные привилегии.;{0}", item.time));
                            break;
                        case 4634:
                            file.WriteLine(String.Format("Выполнен выход учетной записи из системы.;{0}", item.time));
                            break;
                        case 4798:
                            file.WriteLine(String.Format("Перечислено участие пользователя в локальных группах.;{0}", item.time));
                            break;
                        case 4647:
                            file.WriteLine(String.Format("Выход, запрошенный пользователем.;{0}", item.time));
                            break;
                        case 4719:
                            file.WriteLine(String.Format("Изменена политика аудита системы.;{0}", item.time));
                            break;
                        case 4776:
                            file.WriteLine(String.Format("Сведения об участии в группе.;{0}", item.time));
                            break;
                        case 4627:
                            file.WriteLine(String.Format("Компьютер попытался проверить учетные данные учетной записи.;{0}", item.time));
                            break;
                        default:
                            file.WriteLine(String.Format("{0};{1}", item.type, item.time));
                            break;
*/
                    }
                    if (item.Equals(eventList[eventList.Count - 1])) // Это может быть последняя запись, нужно завершить день
                    {
                        file.WriteLine(String.Format("Смена суток или последяя запись;{0}", item.time));
                        if (startOfDay(day.start) == startOfDay(item.time))
                        {
                            day.end = item.time;
                            if (week == null)
                            {
                                week = new Week();
                            }
                            week.dayList.Add(day);
                            day = null;
                        }
                    }
                    prevEvent = item;
                }
            }
        }

        private static void getStartWeek()
        {
            while (dateSatrtWeek.DayOfWeek != System.DayOfWeek.Monday)
            {
                dateSatrtWeek = dateSatrtWeek.AddDays(-1);
            }
            myForm.textBoxStartWeek.Text = dateSatrtWeek.ToShortDateString();
        }

        private static List<MyEvent> Start()
        {
            string logType = "Security";
            List<MyEvent> eventList = new List<MyEvent>();
            try
            {
                EventLogEntry lastEntry = null;
                EventLog ev = new EventLog(logType, System.Environment.MachineName);
                int LastLogToShow = ev.Entries.Count;
                if (LastLogToShow <= 0)
                    Console.WriteLine("No Event Logs in the Log :" + logType);

                string userName = Environment.UserName;
                DateTime currentDay = dateSatrtWeek;
                int i;
                //for (i = ev.Entries.Count - 1; i >= LastLogToShow - 2; i--)
                for (i = ev.Entries.Count - 1; i >= 0; i--)
                {
                    EventLogEntry CurrentEntry = ev.Entries[i];
                    //Для начала режем по датам
                    //if (CurrentEntry.TimeGenerated > DateTime.Today.AddDays(0)) // Данные до нужного диапазона
                    //    continue;
                    //if (CurrentEntry.TimeGenerated < DateTime.Today.AddDays(-1)) // Данные после нужного диапазона
                    //    break;

                    if (CurrentEntry.TimeGenerated < dateSatrtWeek) // Данные после нужного диапазона
                        break;

                    //Режем по пользователю
                    if (!isFoundByUsername(CurrentEntry, userName))
                        continue;

                    MyEvent evnt = new MyEvent(CurrentEntry.InstanceId, CurrentEntry.TimeGenerated);
                    if (CurrentEntry.InstanceId != 4798) //Перечислено участие пользователя в локальных группах.
                        eventList.Add(evnt);
                    lastEntry = CurrentEntry;
                }

                ev.Close();
                eventList.Sort((x, y) => DateTime.Compare(x.time, y.time));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/" + ex.StackTrace, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return eventList;
        }

        public class repo
        {
            public string name;
        }

        public class Err
        {
            public string code;
        }

        public class Data
        {
            public string typeID { get; set; }
            public string typeName { get; set; }
            public string number { get; set; }
            public string info { get; set; }
        }

        public class Response
        {
            public Data[] data { get; set; }
            public Err err { get; set; }
        }
    }
}
