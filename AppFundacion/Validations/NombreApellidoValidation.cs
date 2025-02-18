using InputKit.Shared.Validations;

namespace AppFundacion.Validations;
public class NombreApellidoValidation : IValidation
{
    public string Message { get; set; } = "El campo Nombre y Apellido no puede estar vacio";

    public bool Validate(object value)
    {
        if (value is string text)
        {
            return text != "" && text != null;
        }
        return false;
    }
}
