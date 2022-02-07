using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitOfWork.API.configuration;
using UnitOfWork.API.Model;

namespace UnitOfWork.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _unitOfWork.Employee.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            var item = await _unitOfWork.Employee.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.Id = Guid.NewGuid();
                await _unitOfWork.Employee.Add(employee);

                Employee employee2 = new Employee()
                {
                    Id = Guid.NewGuid(),
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = "shekhar1231@test.com" 
                };
               
                await _unitOfWork.Employee.Add(employee2);

                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetEmployee", new { employee.Id }, employee);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
 
                await _unitOfWork.Employee.Update(employee);

               

                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetEmployee", new { employee.Id }, employee);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var item = await _unitOfWork.Employee.GetByIdAsync(id);

            if (item == null)
                return BadRequest();

            await _unitOfWork.Employee.Delete(id);
            await _unitOfWork.CompleteAsync();

            return Ok(item);
        }

    }
}
