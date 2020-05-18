
using System;

namespace Leopard.API.Controllers
{
	/// <summary>
	/// 这是一个通用的返回结构，用于返回生成的实体的id。
	/// 返回id对于前端后续的操作来说很有方便。
	/// 例如，我发送了评论，那么就会返回评论的id。
	/// </summary>
	public class IdResponse
	{
		public Guid Id { get; set; }

		public IdResponse(Guid id)
		{
			Id = id;
		}
	}
}