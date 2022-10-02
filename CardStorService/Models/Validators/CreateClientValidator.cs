using CardStorService.Models.Requests;
using FluentValidation;

namespace CardStorService.Models.Validators
{
    //класс валидации для CreateClientRequest, на основе интерфейса  AbstractValidator пакета,
    // в качестве параметра указываем класс с полями,которые валидируем
    //Регестрируем в program - раздел Configure FluentValidator
    //Применяем в контролере
    public class CreateClientValidator : AbstractValidator<CreateClientRequest>
    {
        //переопределяем конструктор
        public CreateClientValidator()
        {
            RuleFor(x => x.FirstName)//указаываем свойство
                .NotNull()//правило валидации
                .Length(1,255);//правило валидации

            RuleFor(x => x.Patronymic)
                .NotNull()
                .Length(1, 255);

            RuleFor(x => x.Surname)
                .NotNull()
                .Length(1, 255);


        }
    }
}

