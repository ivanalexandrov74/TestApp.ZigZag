

namespace ZigZag.Test.Dto;

public abstract class BaseResponseDto
{
    public string? errorMessage { get; set; }

    /// <summary>
    /// For a responses without result property we must set value to true or client side will threat it as failed response.
    /// </summary>
    public bool isSuccess { get; protected set; } = true;
    public long? logId { get; set; }
}
public abstract class BaseResponseDto<TResult> : BaseResponseDto where TResult : Enum
{
    public TResult result
    {
        get
        {
            return __result;
        }
        set
        {
            __result = value;

            isSuccess = Convert.ToInt32(value) == 0;
        }
    }
    private TResult __result;
}

public abstract class BaseResponseDto<TResult, TData> : BaseResponseDto<TResult> where TResult : Enum
{
    public TData resultData { get; set; }
}
