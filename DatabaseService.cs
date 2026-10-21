using SQLite;
using UniversityScheduleApp.CodeFiles;

namespace UniversityScheduleApp;

public class DatabaseService
{
    private readonly SQLiteConnection database;


    public DatabaseService()
    {
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "UniversitySchedule.db");

        database = new SQLiteConnection(dbPath);

        database.CreateTable<Lesson>();
    }

    public List<Lesson> GetAllLessons()
    {
        return database.Table<Lesson>().ToList();
    }

    public List<Lesson> GetGroupLessons(int groupID)
    {
        return database.Table<Lesson>()
        .Where(lesson => lesson.groupID == groupID)
        .ToList();
    }

    public void InsertLesson(Lesson lesson)
    {
        database.Insert(lesson);
    }

    public void ClearLessons()
    {
        database.DeleteAll<Lesson>();
    }


}




