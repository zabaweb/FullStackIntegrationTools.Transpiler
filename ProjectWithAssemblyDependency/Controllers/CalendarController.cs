using Microsoft.AspNetCore.Mvc;

namespace SimpleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CalendarController: ControllerBase
{
    [HttpGet(Name = "IsLeapYear")]
    public bool IsLeapYear(int year) => year % 2 == 1;

    //[HttpGet(Name = "ToDayOfWeek")]
    //public string ToDayOfWeek(int dayOfYear) => throw new NotImplementedException();
}
