using Leopard.API.Filters;
using Leopard.API.ResponseConvension;
using Leopard.Domain.Model.GroupshipDealerAggregate;
using Leopard.Domain.Model.Relationships;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leopard.API.Controllers.GroupshipManagementAPI
{
	[Route("api/groupship-management")]
	[ApiController]
	[ServiceFilter(typeof(AuthenticationFilter))]
	[GroupshipManagement]
	public class GroupshipManagementController : ControllerBase
	{
		public GroupshipManagementController(GroupshipManagementPipelineContext z)
		{
			Z = z;
		}

		public GroupshipManagementPipelineContext Z { get; }


		[HttpPost("handle-request")]
		[ItInGroup(false)]
		public async Task<IActionResult> HandleRequest([FromBody]HandleRequestModel2 model)
		{
			if (model.Accept && model.Block)
				return new ApiError(MyErrorCode.ModelInvalid, "Invalid: accept==true and block==true").Wrap();

			var dealer = Z.ItsDealer;

			if (model.Accept)
			{
				dealer.AcceptRequest();
			}
			else
			{
				dealer.RefuseRequest(model.Message, model.Block);
			}

			await Z.GroupshipDealerRepository.PutAsync(dealer);

			return Ok();
		}
		public class HandleRequestModel2 : IGroupshipManagementModel
		{
			public string GroupId { get; set; }

			public bool Accept { get; set; }

			public bool Block { get; set; }

			[Required(AllowEmptyStrings = true)]
			[MaxLength(32)]
			public string Message { get; set; }

			public string ItId { get; set; }
		}



		[HttpPost("assign-admin")]
		[ItInGroup(true)]
		public async Task<IActionResult> AssignAdmin([FromBody]SimpleGroupshipManageModel model)
		{
			if (Z.ItsDealer.GroupShip.Role != GroupRole.Normal)
			{
				return new ApiError(MyErrorCode.NotNormalRole, "Target user is not normal member").Wrap();
			}

			Z.ItsDealer.SetRole(GroupRole.Admin);

			await Z.GroupshipDealerRepository.PutAsync(Z.ItsDealer);

			return Ok();
		}
		public class SimpleGroupshipManageModel : IGroupshipManagementModel
		{
			public string ItId { get; set; }
			public string GroupId { get; set; }
		}


		[HttpPost("kick")]
		[ItInGroup(true)]
		public async Task<IActionResult> Kick([FromBody]SimpleGroupshipManageModel model)
		{
			if (Z.ItsDealer.GroupShip.Role != GroupRole.Normal)
			{
				return new ApiError(MyErrorCode.PermissionDenied, "Permission denied").Wrap();
			}

			Z.ItsDealer.QuitGroup();

			await Z.GroupshipDealerRepository.PutAsync(Z.ItsDealer);

			return Ok();
		}

		[NotCommand]
		[HttpPost("query-context")]
		[Produces(typeof(QGroupshipContext))]
		public async Task<IActionResult> GetContext([FromBody]SimpleGroupshipManageModel model)
		{
			return new JsonResult(QGroupshipContext.ManagerUserView(Z.ItsDealer));
		}
	}


	public class QGroupshipContext
	{
		public ObjectId Id { get; set; }
		public ObjectId UserId { get; set; }
		public ObjectId GroupId { get; set; }
		public QGroupship Groupship { get; set; }
		public QGroupshipRequest Request { get; set; }
		public long CreatedAt { get; set; }
		public long UpdatedAt { get; set; }

		public static QGroupshipContext ManagerUserView(GroupshipDealer d)
		{
			if (d == null)
				return null;

			return new QGroupshipContext
			{
				Id = d.Id,
				UserId = d.UserId,
				GroupId = d.GroupId,
				Groupship = QGroupship.UserView(d.GroupShip),
				Request = QGroupshipRequest.UserView(d.Request),
				CreatedAt = d.CreatedAt,
				UpdatedAt = d.UpdatedAt
			};
		}
	}

	public class QGroupship
	{
		public ObjectId Id { get; set; }
		public ObjectId GroupId { get; set; }
		public ObjectId UserId { get; set; }
		public bool IsValid { get; set; }
		public GroupRole Role { get; set; }
		public long CreatedAt { get; set; }
		public long UpdatedAt { get; set; }
		public string GroupDisplayName { get; set; }

		public static QGroupship UserView(GroupShip ship)
		{
			if (ship == null)
				return null;

			var a = new QGroupship
			{
				Id = ship.Id,
				GroupId = ship.GroupId,
				UserId = ship.UserId,
				IsValid = ship.IsValid,
				Role = ship.Role,
				CreatedAt = ship.CreatedAt,
				UpdatedAt = ship.UpdatedAt,
				GroupDisplayName = ship.GroupDisplayName
			};
			return a;
		}
	}

	public class QGroupshipRequest
	{
		public ObjectId Id { get; set; }
		public QInvestigationAndAnswer[] InvestigationAndAnswers { get; set; }
		public string RefuseMessage { get; set; }
		public RelationshipRequestStatus Status { get; set; }
		public RelationshipRequestType Type { get; set; }
		public long UpdatedAt { get; set; }
		public long CreatedAt { get; set; }
		public string ValidationMessage { get; set; }

		public static QGroupshipRequest UserView(GroupshipRequest r)
		{
			if (r == null)
				return null;

			return new QGroupshipRequest
			{
				Id = r.Id,
				InvestigationAndAnswers = r.InvestigationAndAnswers?.Select(p => QInvestigationAndAnswer.View(p)).ToArray(),
				RefuseMessage = r.RefuseMessage,
				Status = r.Status,
				Type = r.Type,
				UpdatedAt = r.UpdatedAt,
				CreatedAt = r.CreatedAt,
				ValidationMessage = r.ValidationMessage
			};
		}
	}

	public class QInvestigationAndAnswer
	{
		public string Investigation { get; set; }
		public string Answer { get; set; }

		public static QInvestigationAndAnswer View(InvestigationAndAnswer ias)
		{
			if (ias == null)
				return null;

			return new QInvestigationAndAnswer
			{
				Investigation = ias.Investigation.Content,
				Answer = ias.Answer,
			};
		}
	}
}