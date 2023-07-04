using PracticalFourteen.Pagination;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PracticalFourteen.Controllers
{
	public class EmployeesController : Controller
	{
		readonly PracticalFourteenEntities db = new PracticalFourteenEntities();

		[HttpGet]
		public async Task<ActionResult> Index(string name, int? page)
		{
			int pageNumber = page ?? 1; 
			int pageSize = 10; 
			var totalCount = await db.Employees.CountAsync();
			var pagedEmployees = await db.Employees.OrderBy(e => e.Id)
										   .Skip((pageNumber - 1) * pageSize)
										   .Take(pageSize)
										   .ToListAsync();

			var pagedResult = new PagedResult<Employee>(pagedEmployees, totalCount, pageNumber, pageSize);
			return View(pagedResult);
		}

		[HttpGet]
		public JsonResult GetData(string SearchString, int pageNo)
		{
			if (SearchString == null)
			{
				return Json(db.Employees.OrderBy(x => x.Id).Skip((pageNo - 1) * 10).Take(10), JsonRequestBehavior.AllowGet);
			}

			var employees = db.Employees.Where(x => x.Name.Contains(SearchString)).OrderBy(x => x.Id).Skip((pageNo - 1) * 10).Take(10);

			return Json(employees, JsonRequestBehavior.AllowGet);

		}

		[HttpGet]
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = await db.Employees.FindAsync(id);
			if (employee == null)
			{
				return HttpNotFound();
			}
			return View(employee);
		}

		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include = "Id,Name,DOB,Age")] Employee employee)
		{
			if (ModelState.IsValid)
			{
				db.Employees.Add(employee);
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			return View(employee);
		}

		[HttpGet]
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = await db.Employees.FindAsync(id);
			if (employee == null)
			{
				return HttpNotFound();
			}
			return View(employee);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include = "Id,Name,DOB,Age")] Employee employee)
		{
			if (ModelState.IsValid)
			{
				db.Entry(employee).State = EntityState.Modified;
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(employee);
		}

		[HttpGet]
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Employee employee = await db.Employees.FindAsync(id);
			if (employee == null)
			{
				return HttpNotFound();
			}
			return View(employee);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			Employee employee = await db.Employees.FindAsync(id);
			db.Employees.Remove(employee);
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
