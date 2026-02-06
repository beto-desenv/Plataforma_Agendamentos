namespace Plataforma_Agendamentos.DTOs.Schedule;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
