using FluentValidation;
using Plataforma_Agendamentos.DTOs;

namespace Plataforma_Agendamentos.Validators;

public class ServiceRequestValidator : AbstractValidator<ServiceRequest>
{
    public ServiceRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(100).WithMessage("Título deve ter no máximo 100 caracteres")
            .MinimumLength(3).WithMessage("Título deve ter pelo menos 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero")
            .LessThanOrEqualTo(999999.99m).WithMessage("Preço deve ser menor ou igual a R$ 999.999,99");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duração deve ser maior que zero minutos")
            .LessThanOrEqualTo(1440).WithMessage("Duração não pode exceder 1440 minutos (24 horas)")
            .Must(BeMultipleOfFifteen).WithMessage("Duração deve ser múltiplo de 15 minutos");
    }

    private bool BeMultipleOfFifteen(int minutes)
    {
        return minutes % 15 == 0;
    }
}

public class ScheduleRequestValidator : AbstractValidator<ScheduleRequest>
{
    public ScheduleRequestValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6).WithMessage("Dia da semana deve estar entre 0 (Domingo) e 6 (Sábado)");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Horário de início é obrigatório")
            .Must(BeValidTime).WithMessage("Horário de início deve ser válido");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Horário de fim é obrigatório")
            .Must(BeValidTime).WithMessage("Horário de fim deve ser válido")
            .GreaterThan(x => x.StartTime).WithMessage("Horário de fim deve ser posterior ao horário de início");

        RuleFor(x => x)
            .Must(HaveMinimumDuration).WithMessage("O horário deve ter pelo menos 30 minutos de duração")
            .Must(HaveMaximumDuration).WithMessage("O horário não pode ter mais de 12 horas de duração");
    }

    private bool BeValidTime(TimeSpan time)
    {
        return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
    }

    private bool HaveMinimumDuration(ScheduleRequest request)
    {
        return (request.EndTime - request.StartTime).TotalMinutes >= 30;
    }

    private bool HaveMaximumDuration(ScheduleRequest request)
    {
        return (request.EndTime - request.StartTime).TotalHours <= 12;
    }
}

public class BookingRequestValidator : AbstractValidator<BookingRequest>
{
    public BookingRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ID do serviço é obrigatório");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Data e horário são obrigatórios")
            .Must(BeInTheFuture).WithMessage("Data e horário devem estar no futuro")
            .Must(BeWithinBusinessHours).WithMessage("Horário deve estar dentro do horário comercial (6:00 às 22:00)")
            .Must(BeAtValidMinutes).WithMessage("Agendamento deve ser em horários válidos (00, 15, 30 ou 45 minutos)");
    }

    private bool BeInTheFuture(DateTime date)
    {
        return date > DateTime.Now.AddMinutes(30); // Pelo menos 30 minutos no futuro
    }

    private bool BeWithinBusinessHours(DateTime date)
    {
        var time = date.TimeOfDay;
        return time >= TimeSpan.FromHours(6) && time <= TimeSpan.FromHours(22);
    }

    private bool BeAtValidMinutes(DateTime date)
    {
        var validMinutes = new[] { 0, 15, 30, 45 };
        return validMinutes.Contains(date.Minute);
    }
}