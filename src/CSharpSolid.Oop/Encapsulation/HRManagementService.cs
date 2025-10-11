
using CSharpSolid.Oop.Encapsulation;

public class HRManagementService
{
    private readonly IList<GoodEmployee> _employees;
    private readonly ILogger<HRManagementService> _logger;
    private readonly ITimeProvider _timeProvider;

    public HRManagementService(ILogger<HRManagementService> logger, ITimeProvider timeProvider)
    {
        _employees = new List<GoodEmployee>();
        _logger = logger;
        _timeProvider = timeProvider;
    }
}