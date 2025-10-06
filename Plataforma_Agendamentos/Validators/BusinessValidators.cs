using FluentValidation;
using Plataforma_Agendamentos.DTOs;

namespace Plataforma_Agendamentos.Validators;

public class ServiceRequestValidator : AbstractValidator<ServiceRequest>
{
    public ServiceRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Titulo e obrigatorio")
            .MaximumLength(100).WithMessage("Titulo deve ter no maximo 100 caracteres")
            .MinimumLength(3).WithMessage("Titulo deve ter pelo menos 3 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Descricao deve ter no maximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Preco deve ser maior que zero")
            .LessThanOrEqualTo(999999.99m).WithMessage("Preco deve ser menor ou igual a R$ 999.999,99");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duracao deve ser maior que zero minutos")
            .LessThanOrEqualTo(1440).WithMessage("Duracao nao pode exceder 1440 minutos (24 horas)")
            .Must(BeMultipleOfFifteen).WithMessage("Duracao deve ser multiplo de 15 minutos");
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
            .InclusiveBetween(0, 6).WithMessage("Dia da semana deve estar entre 0 (Domingo) e 6 (Sabado)");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Horario de inicio e obrigatorio")
            .Must(BeValidTimeSpan).WithMessage("Horario de inicio deve ser valido");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Horario de fim e obrigatorio")
            .Must(BeValidTimeSpan).WithMessage("Horario de fim deve ser valido")
            .GreaterThan(x => x.StartTime).WithMessage("Horario de fim deve ser posterior ao horario de inicio");

        RuleFor(x => x)
            .Must(HaveMinimumDuration).WithMessage("O horario deve ter pelo menos 30 minutos de duracao")
            .Must(HaveMaximumDuration).WithMessage("O horario nao pode exceder 12 horas de duracao");
    }

    private bool BeValidTimeSpan(TimeSpan time)
    {
        return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
    }

    private bool HaveMinimumDuration(ScheduleRequest request)
    {
        var duration = request.EndTime - request.StartTime;
        return duration.TotalMinutes >= 30;
    }

    private bool HaveMaximumDuration(ScheduleRequest request)
    {
        var duration = request.EndTime - request.StartTime;
        return duration.TotalHours <= 12;
    }
}

public class BookingRequestValidator : AbstractValidator<BookingRequest>
{
    public BookingRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ID do servico e obrigatorio");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Data e obrigatoria")
            .GreaterThan(DateTime.Now).WithMessage("Data deve ser futura");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Observacoes devem ter no maximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public class AddRoleRequestValidator : AbstractValidator<AddRoleRequest>
{
    public AddRoleRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role e obrigatorio")
            .Must(role => role == "cliente" || role == "prestador")
            .WithMessage("Role deve ser 'cliente' ou 'prestador'");
    }
}