namespace PsGenApi.ServiceModel;

public class ApiResponseDto
{
	public object Result { get; set; } = default!;

	public bool IsSuccess { get; set; }

	public string Message { get; set; } = string.Empty;
}

public class ApiResponseDto<T> : ApiResponseDto
{
	public new T Result { get; set; } = default!;
}

public class DeletedResponseDto : ApiResponseDto<bool>
{
}

public class RecordResponseDto<T> : ApiResponseDto<T>
{
}

public class RecordsResponseDto<T> : ApiResponseDto<IList<T>>
{
}