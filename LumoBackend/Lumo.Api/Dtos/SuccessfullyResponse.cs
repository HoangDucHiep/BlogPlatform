namespace Lumo.Api.Dtos;

public class SuccessfullyResponse
{
    public bool Success { get; private set; } = true;

    public object Response { get; private set; }

    public static SuccessfullyResponse Create(object response)
    {
        return new SuccessfullyResponse
        {
            Response = response
        };
    }

}
