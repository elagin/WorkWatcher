using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace WorkWatcher
{
    public partial class Form1 : Form
    {

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
                //                weekList.Add(new Week);
            }
        }

        DateTime dateSatrtWeek = DateTime.Today;
        Month month = new Month();

        List<Session> sessionList = new List<Session>();
        List<MyEvent> eventList = new List<MyEvent>();
        int hourPerDay = 8;

        TimeSpan totalWork;

        public Form1()
        {
            InitializeComponent();

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

            getStartWeek();
            textBoxUserName.Text = Environment.UserDomainName + "\\" + Environment.UserName;
            Start();
            parseEventList3();
            textBoxEventCount.Text = String.Format("Обнаружено {0} событий.", eventList.Count);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt"))
            {
                foreach (Week week in month.weekList)
                {
                    foreach (Day day in week.dayList)
                    {
                        TimeSpan diff = day.end - day.start;
                        TimeSpan total = new TimeSpan(diff.Hours- hourPerDay, diff.Minutes, diff.Seconds);
                        string[] row = { day.start.ToShortDateString(), day.start.ToShortTimeString(), day.end.ToShortTimeString(), diff.ToString(@"hh\:mm"), total.ToString() };
                        var listViewItem = new ListViewItem(row);
                        listView.Items.Add(listViewItem);
                        TimeSpan spanDay = new TimeSpan(diff.Hours, diff.Minutes, diff.Seconds);
                        totalWork = totalWork.Add(spanDay);
                    }
                }
            }
            textBoxTotalWork.Text = totalWork.TotalHours.ToString();
            textBoxTotalWait.Text = ((hourPerDay * 4) - totalWork.TotalHours).ToString();
        }

        private bool isFoundByUsername(EventLogEntry entry, string userName)
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
        /*
                private void parseEventList()
                {
                    Session currentSession = new Session();
                    //for (i = ev.Entries.Count - 1; i >= 0; i--)
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt"))
                    {
                        foreach (MyEvent item in eventList)
                        {
                            //count++;
                            switch (item.type)
                            {
                                case 528://res = "Успешный вход в систему";
                                case 4624:
                                    if (currentSession == null)
                                    {
                                        currentSession = new Session();
                                        currentSession.start = item.time;
                                        file.WriteLine(String.Format("Вход в систему;{0}", item.time));
                                    }
                                    else
                                    {
                                        file.WriteLine(String.Format("Повторный вход в систему invalid;{0}", item.time));
                                    }
                                    break;
                                case 538://res = "Выход пользователя из системы";
                                case 4634:
                                    if (currentSession != null)
                                    {
                                        currentSession.end = item.time;
                                        sessionList.Add(currentSession);
                                        currentSession = null;
                                        file.WriteLine(String.Format("Выход из системы;{0}", item.time));
                                        file.WriteLine("======");
                                    }
                                    else
                                    {
                                        file.WriteLine(String.Format("Повторынй выход из системы invalid;{0}", item.time));
                                    }
                                    break;
                                case 682:
                                    break;
                                case 576:
                                    file.WriteLine(String.Format("Присвоение специальных прав для нового сеанса входа;{0}", item.time));
                                    break;
                                default:
                                    file.WriteLine(String.Format("{0};{1}", item.type, item.time));
                                    break;
                            }
                            //TimeSpan span = session.end - session.start;
                            //span.ToString();
                        }
                    }
                }
        */

        private DateTime startOfDay(DateTime value)
        {
            DateTime res = value.AddHours(-value.Hour).AddMinutes(-value.Minute).AddSeconds(-value.Second);
            return res;
        }

        private void parseEventList3()
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
                        if (startOfDay(day.start) != startOfDay(item.time))
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
                        if (startOfDay(day.start) == startOfDay(item.time))
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
            Session currentSession = new Session();
            //for (i = ev.Entries.Count - 1; i >= 0; i--)
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt"))
            {
                Week week = null;
                Day day = null;
                MyEvent prevEvent = null;
                foreach (MyEvent item in eventList)
                {
                    //file.WriteLine(String.Format("{0};{1}", item.type, item.time));
                    //count++;

                    //Week week = new Week();
                    //Day day = new Day();

                    //if(month.weekList.Count == 0)
                    //  month.weekList.Add(W)

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

        private void getStartWeek()
        {
            while (dateSatrtWeek.DayOfWeek != System.DayOfWeek.Monday)
            {
                dateSatrtWeek = dateSatrtWeek.AddDays(-1);
            }
            textBoxStartWeek.Text = dateSatrtWeek.ToShortDateString();
        }

        private void Start()
        {
            string logType = "Security";
            try
            {
                EventLogEntry lastEntry = null;

                EventLog ev = new EventLog(logType, System.Environment.MachineName);
                int LastLogToShow = ev.Entries.Count;
                if (LastLogToShow <= 0)
                    Console.WriteLine("No Event Logs in the Log :" + logType);

                int i;
                //for (i = ev.Entries.Count - 1; i >= LastLogToShow - 2; i--)
                string userName = Environment.UserName;

                DateTime currentDay = dateSatrtWeek;

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
        }
    }
}
