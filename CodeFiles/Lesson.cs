using System;
using System.Collections.Generic;
using System.Text;

namespace UniversityScheduleApp.CodeFiles
{
    public class Lesson
    {
        // Название, начало, конец
        public int eventNumber { get; set; }

        public string eventName { get; set; }

        public string eventType { get; set; }

        public string eventRoom { get; set; }

        // Конструктор
        public Lesson(int number, string name, string type, string room)
        {
            eventNumber = number;
            eventName = name;       // Название
            eventType = type;       // Тип (Практика, Лекция, Лаба)
            eventRoom = room;       // Аудитория
        }

    }
}
