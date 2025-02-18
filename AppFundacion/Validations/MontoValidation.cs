using InputKit.Shared.Validations;

namespace AppFundacion.Validations;
public class MontoValidation : IValidation
{
    public string Message { get; set; } = "Monto debe ser numerico y no negativo";

    public bool Validate(object value)
    {
        if (value is string text)
        {
            if (int.TryParse(text, out int monto))
            {
                return monto >= 0;
            }
        }
        return false;
    }
}
