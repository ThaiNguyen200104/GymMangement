using GymManagement_API.Models.AccountModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PackageController : ControllerBase
{
	private readonly GymManagementContext _context;

	
}
