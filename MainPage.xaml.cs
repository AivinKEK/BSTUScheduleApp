using UniversityScheduleApp.CodeFiles;
using System.Diagnostics;
using System.Net.WebSockets;

namespace UniversityScheduleApp;

public partial class MainPage : ContentPage
{
   
    // Пустое расписание       
    List<Event> events = new List<Event>();

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

    // Расписание, пока харкод, потом будет SQL, а потом может быть и парсинг с сайта

    // Нечётная неделя
    List<Lesson> mondayNotEven = new List<Lesson>()
    {
        new Lesson(2, "Математический Анализ", "Практика", "А314"),
        new Lesson(3, "Информатика", "Лабораторная", "409"),
    };
    List<Lesson> tuesdayNotEven = new List<Lesson>()
    {
        new Lesson(2, "Программирование", "Лекция", "302"),
        new Lesson(3, "Матан", "Лекция", "Б402"),
        new Lesson(4, "Алгем", "Практика", "А318"),
    };
    List<Lesson> wednesdayNotEven = new List<Lesson>()
    {
        new Lesson(1, "Программирование", "Лабораторная", "425"),
        new Lesson(2, "Физкультура", "Практика", "Б105"),
        new Lesson(3, "Программирование", "Лабораторная", "425"),
    };
    List<Lesson> thursdayNotEven = new List<Lesson>()
    {
        new Lesson(2, "Основы Российской Государственности", "Практика", "ЧЗ"),
        new Lesson(3, "Основы Российской Государственности", "Лекция", "ЧЗ"),
        new Lesson(4, "Системный Анализ", "Практика", "Б303"),
    };
    List<Lesson> fridayNotEven = new List<Lesson>()
    {
        new Lesson(1, "Алгебра Геометрия", "Лекция", "51"),
    };

    // Чётная неделя
    List<Lesson> mondayEven = new List<Lesson>()
    {
        new Lesson(2, "Математический Анализ", "Практика", "А314"),
        new Lesson(3, "Дискретная математика", "Практика", "А230"),
    };
    List<Lesson> tuesdayEven = new List<Lesson>()
    {
        new Lesson(2, "Программирование", "Лекция", "302"),
        new Lesson(3, "Матан", "Лекция", "Б402"),
        new Lesson(4, "Алгем", "Практика", "А318"),
    };
    List<Lesson> wednesdayEven = new List<Lesson>()
    {
        new Lesson(1, "Программирование", "Лабораторная", "425"),
        new Lesson(2, "Физкультура", "Практика", "Б105"),
        new Lesson(3, "Программирование", "Лабораторная", "425"),
    };
    List<Lesson> thursdayEven = new List<Lesson>()
    {
        new Lesson(2, "Основы Российской Государственности", "Практика", "ЧЗ"),
        new Lesson(3, "Основы Российской Государственности", "Лекция", "ЧЗ"),
        new Lesson(4, "Дискретная математика", "Лекция", "51"),
    };
    List<Lesson> fridayEven = new List<Lesson>()
    {
        new Lesson(1, "Алгебра Геометрия", "Лекция", "51"),
        new Lesson(2, "ТЛПР", "Практика", "А230"),
    };

    // Словарик для этой хрени. Боже, как же хорошо, что это временно и я потом сделаю SQL таблицу, а не эту простыню кода
    Dictionary<(bool, DayOfWeek), List<Lesson>> schedule;

    public MainPage()
    {

        InitializeComponent();

        // Временная херня, пока не сделана SQL таблица
        schedule = new()
        {
            { (false, DayOfWeek.Monday), mondayNotEven },
            { (false, DayOfWeek.Tuesday), tuesdayNotEven },
            { (false, DayOfWeek.Wednesday), wednesdayNotEven },
            { (false, DayOfWeek.Thursday), thursdayNotEven },
            { (false, DayOfWeek.Friday), fridayNotEven },

            { (true, DayOfWeek.Monday), mondayEven },
            { (true, DayOfWeek.Tuesday), tuesdayEven },
            { (true, DayOfWeek.Wednesday), wednesdayEven },
            { (true, DayOfWeek.Thursday), thursdayEven },
            { (true, DayOfWeek.Friday), fridayEven }
        };



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

        // Заполнение расписания

        if (schedule.ContainsKey((IsEvenWeek(), selectedDay)))
        {
            foreach (var lesson in schedule[(IsEvenWeek(), selectedDay)])
            {
                events.Add(new Event(lesson.eventNumber, eventTimes[lesson.eventNumber].start, eventTimes[lesson.eventNumber].end, lesson.eventName, lesson.eventType, lesson.eventRoom));
            }
        }

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