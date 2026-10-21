using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace UniversityScheduleApp.CodeFiles
{
    public class Lesson
    {
        // Название, начало, конец


        [PrimaryKey, AutoIncrement]
        public int eventID { get; set; }
        public int groupID { get; set; }
        public bool isEvenWeek  { get; set; }
        public int  weekDay     { get; set; }
        public int  eventNumber { get; set; }
        public string eventName { get; set; }
        public string eventType { get; set; }
        public string eventRoom { get; set; }

        // Конструктор
        public Lesson(int id, int groupId, bool even, int day,int number, string name, string type, string room)
        {
            eventID = id;           // ID
            groupID = groupId;      // ID группы
            isEvenWeek = even;      // Чётность недели (0 - чётная, 1 - нечётная. По остатку от деления на 2 типа)
            weekDay = day;          // День недели (номер 0-6)
            eventNumber = number;   // Порядковый номер в расписании
            eventName = name;       // Название
            eventType = type;       // Тип (Практика, Лекция, Лаба)
            eventRoom = room;       // Аудитория
        }

        public Lesson()
        {
        }

    }
}
