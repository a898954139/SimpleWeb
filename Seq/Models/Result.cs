namespace Seq.Models;

public class Result(
    bool isSuccess,
    Exception? error = null)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public Exception? Error { get; set; } = error;
}