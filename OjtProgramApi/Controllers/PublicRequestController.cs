
using Microsoft.AspNetCore.Mvc;
using OjtProgramApi.Controllers;
using OjtProgramApi.Repositories;

[ApiController]
public class PublicRequestController : BaseController<PublicRequestController>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IConfiguration _configuration;
    public PublicRequestController(IRepositoryWrapper repositoryWrapper, IConfiguration configuration)
    {
        _repositoryWrapper = repositoryWrapper;
        _configuration = configuration;
    }

    [HttpGet("heartbeat", Name = "heartbeat")]
    public ActionResult<string> HeartBeat()
    {
        var responseString = "Api is working well. Server DateTime " + DateTime.Now.ToString();
        return Ok(responseString);
    }
}
