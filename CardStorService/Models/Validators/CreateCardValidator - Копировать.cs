using CardStorService.Models.Requests;
using FluentValidation;

namespace CardStorService.Models.Validators
{
    //класс валидации для CreateCardRequest, на основе интерфейса  AbstractValidator пакета,
    // в качестве параметра указываем класс с полями,которые валидируем
    //Регестрируем в program - раздел Configure FluentValidator
    //Применяем в контролере
    public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        //переопределяем конструктор
        public CreateCardRequestValidator()
        {
            RuleFor(x => x.CVV2)//указаываем свойство
                .NotNull()//правило валидации
                .Length(3);//правило валидации

            RuleFor(x => x.CardNo)
                .NotNull()
                .Length(16, 20);

            RuleFor(x => x.ExpDate)
                .NotNull();

            RuleFor(x => x.ClientId)
                .NotNull()
                .InclusiveBetween(1,100000);

            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50);


        }
    }
}

