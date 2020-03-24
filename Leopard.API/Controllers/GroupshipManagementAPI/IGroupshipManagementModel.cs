using Leopard.API.Controllers.GroupshipAPI;

namespace Leopard.API.Controllers.GroupshipManagementAPI
{
	public interface IGroupshipManagementModel : IGroupModel
	{
		public string ItId { get; }
	}
}
