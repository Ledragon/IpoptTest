namespace Ipopt
{
    public enum ReturnCode
    {
        ProblemNotInitialized = -900,
        InternalError = -199,
        InsufficientMemory = -102,
        NonIpoptExceptionThrown = -101,
        UnrecoverableException = -100,
        InvalidNumberDetected = -13,
        InvalidOption = -12,
        InvalidProblemDefinition = -11,
        NotEnoughDegreesOfFreedom = -10,
        MaximumCpuTimeExceeded = -4,
        ErrorInStepComputation = -3,
        RestorationFailed = -2,
        MaximumIterationsExceeded = -1,
        SolveSucceeded = 0,
        SolvedToAcceptableLevel = 1,
        InfeasibleProblemDetected = 2,
        SearchDirectionBecomesTooSmall = 3,
        DivergingIterates = 4,
        UserRequestedStop = 5,
        FeasiblePointFound = 6
    }
}