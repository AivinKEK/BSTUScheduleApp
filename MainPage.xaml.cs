using System.Diagnostics;
using System.Net.WebSockets;
using UniversityScheduleApp.CodeFiles;

namespace UniversityScheduleApp;

public partial class MainPage : ContentPage
{
   
    // Список Расписание выбранного дня       
    List<Event> events = new List<Event>();

    // Список Полное расписание выбранной группы 
    List<Lesson> groupSchedule = new List<Lesson>();

    // БД с расписанием
    private DatabaseService databaseService;


    int weekDay;
    bool invertWeek = false; // Переключение Чётности недели

    // Сегодняшний День Недели
    DayOfWeek selectedDay;
    // Словарик Дней недели на русском
    Dictionary<DayOfWeek, string> dayNames = new()
    {
        { DayOfWeek.Monday, "Понедельник" },
        { DayOfWeek.Tuesday, "Вторник" },
        { DayOfWeek.Wednesday, "Среда" },
        { DayOfWeek.Thursday, "Четверг" },
        { DayOfWeek.Friday, "Пятница" },
        { DayOfWeek.Saturday, "Суббота" },
        { DayOfWeek.Sunday, "Воскресенье" }
    };

    // Расписание звонков. Да захардкодено, лень думать, как по другому
    Dictionary<int, (TimeOnly start, TimeOnly end)> eventTimes = new()
    {
        { 1, (new TimeOnly(8, 0),  new TimeOnly(9, 35)) },
        { 2, (new TimeOnly(9, 45), new TimeOnly(11, 20)) },
        { 3, (new TimeOnly(11, 30), new TimeOnly(13, 5)) },
        { 4, (new TimeOnly(13, 20), new TimeOnly(14, 55)) },
        { 5, (new TimeOnly(15, 5), new TimeOnly(16, 40)) },
        { 6, (new TimeOnly(16, 50), new TimeOnly(18, 25)) },
        { 7, (new TimeOnly(18, 40), new TimeOnly(20, 15)) },
        { 8, (new TimeOnly(20, 25), new TimeOnly(22, 00)) },
    };




    public MainPage()
    {

        InitializeComponent();

        // Инициализация БД
        databaseService = new DatabaseService();
        

        // Авевеве
        groupSchedule = databaseService.GetGroupLessons(1);

        // Назначение сегодняшнего дня недели
        selectedDay = DateTime.Now.DayOfWeek;

        // Запускаем логику расписания при открытии страницы
        LoadSchedule();

        // Я не знаю, как этот кусок кода работает, но он вызывает функцию UpdateTimer() каждую секунду
        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            UpdateTimer();
            return true;
        });
    }

    private void LoadSchedule()
    {
        events.Clear();
        // Обновление данных о дате на экране
        weekDayLabel.Text = dayNames[selectedDay];
        string evenNotEven;
        if (IsEvenWeek())
        {
            evenNotEven = "ЧЕТ";
        }
        else
        {
            evenNotEven = "НЕЧЕТ";
        }
        dateAndEven.Text = DateTime.Now.ToString("dd.MM.yyyy") + " " + evenNotEven;

        // Заполнение расписания (списка events)
        foreach(var lesson in groupSchedule)
        {
            if (lesson.isEvenWeek == IsEvenWeek() && lesson.weekDay == (int)selectedDay)
            {
                events.Add(new Event(lesson.eventNumber, eventTimes[lesson.eventNumber].start, eventTimes[lesson.eventNumber].end, lesson.eventName, lesson.eventType, lesson.eventRoom));
            }
        }

        // обнуляет и снова назначает источник данных для ScheduleCollectionView, чтобы обновить отображение
        ScheduleCollectionView.ItemsSource = null;
        ScheduleCollectionView.ItemsSource = events;

    }

    // Обновление главного таймера
    private void UpdateTimer()
    {
        // Будущий вывод
        string outputTime;

        // Поиск текущей пары
        foreach (Event thatEvent in events)
        {
            if (thatEvent.IsEventNow() && thatEvent.eventName != null)
            {
                outputTime = thatEvent.TimeLeft().ToString(@"hh\:mm\:ss");
                Timer.Text = outputTime;
                return;
            }
        }

        // Поиск ближайшей пары
        Event closestEvent = null;
        TimeSpan minTime = TimeSpan.MaxValue;
        foreach(Event thatEvent in events)
        {
            if (thatEvent.TimeStart() < minTime && thatEvent.eventName != null)
            {
                minTime = thatEvent.TimeStart();
                closestEvent = thatEvent;
            }
        }
        if (closestEvent != null)
        {
            outputTime = closestEvent.TimeStart().ToString(@"hh\:mm\:ss");
            Timer.Text = outputTime;
        }
        return;

    }

    // Определение чётности текущей недели
    private bool IsEvenWeek()
    {
        int weeksPassed = (DateTime.Today - new DateTime(2026, 9, 21)).Days / 7;
        return (weeksPassed % 2 == 0) ^ invertWeek;
    }
    //      Переключение дня недели
    // Назад
    private void PreviousDay(object sender, EventArgs e)
    {
        selectedDay = (DayOfWeek)(((int)selectedDay + 6) % 7);
        LoadSchedule();
    }
    // Вперёд
    private void NextDay(object sender, EventArgs e)
    {
        selectedDay = (DayOfWeek)(((int)selectedDay + 1) % 7);
        LoadSchedule();
    }
    // Смена Чётности недели
    private void InvertWeek(object sender, EventArgs e)
    {
        invertWeek = !invertWeek;
        LoadSchedule();
    }

}