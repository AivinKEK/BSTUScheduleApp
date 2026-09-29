using System;
using System.Collections.Generic;
using System.Text;

namespace UniversityScheduleApp.CodeFiles
{
    public class Event
    {
        // Название, начало, конец
        public int eventNumber { get; set; }
        private TimeOnly startTime;
        private TimeOnly endTime;

        public string eventName { get; set; }

        public string eventType { get; set; }

        public string eventRoom { get; set; }

        public string StartEndTime
        {
            get
            {
                return startTime.ToString("HH:mm") + " - " + endTime.ToString("HH:mm");
            }
        }

        // Конструктор
        public Event(int number, TimeOnly start, TimeOnly end, string name, string type, string room)
        {
            eventNumber = number;   // Порядковый номер
            startTime = start;      // Время начала
            endTime = end;          // Время конца
            eventName = name;       // Название
            eventType = type;       // Тип (Практика, Лекция, Лаба)
            eventRoom = room;       // Аудитория
        }


        // Проверка, длится ли событие сейчас
        public bool IsEventNow()
        {
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            if (startTime < endTime)
            {
                return startTime <= currentTime && currentTime <= endTime;
            }
            else
            {
                return !(endTime <= currentTime && currentTime <= startTime);

            }
        }

        // Сколько времени до конца события
        public TimeSpan TimeLeft()
        {
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            if (startTime < endTime)
            {
                return endTime - currentTime;
            }
            else
            {
                if (currentTime >= startTime)
                {
                    return new TimeSpan(24, 0, 0) - currentTime.ToTimeSpan() + endTime.ToTimeSpan();
                }
                else
                {
                    return endTime - currentTime;
                }
            }
        }       

        // Сколько время до начала события
        public TimeSpan TimeStart()
        {
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            if ((startTime - currentTime) >= TimeSpan.Zero)
            {
                return startTime - currentTime;
            }
            else
            {
                return (startTime - currentTime) + new TimeSpan(24, 0, 0);
            }
        }

       

    }
}
