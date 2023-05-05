namespace PsGenApi.ServiceModel;

public class ApiResponse
{
    public object Result { get; set; }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    public new T Result { get; set; }
}

public class DeletedResponse : ApiResponse<bool>
{
}

public class RecordResponse<T> : ApiResponse<T>
{
}

public class RecordsResponse<T> : ApiResponse<IList<T>>
{
}