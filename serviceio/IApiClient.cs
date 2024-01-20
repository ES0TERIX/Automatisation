using RestEase;

public interface IApiClient
{
  [Post("/calculmoyenne")]
  Task<HttpResponseMessage> CalculMoyenne([Body] object data);
}
