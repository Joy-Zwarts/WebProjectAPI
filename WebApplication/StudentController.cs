using Microsoft.AspNetCore.Mvc;

namespace Projectje;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    [HttpGet]
    public string GetString()
    {
        return "Het endpoint werkt!";
    }

    [HttpGet("student")]
    public Student GetStudent()
    {
        return new Student
        {
            Id = 1,
            Studentnummer = "25012345",
            Naam = "Jelle"
        };
    }

    [HttpGet("studentmetstatus")]
    public ActionResult<Student> GetStudentMetStatus()
    {
        var student = new Student
        {
            Id = 1,
            Studentnummer = "25012345",
            Naam = "Jelle"
        };

        if (student.Naam == "Jelle")
        {
            return NotFound();
        }

        return student;
    }

    [HttpGet("{id}")]
    public ActionResult<Student> GetStudentById(int id)
    {
        var student = new Student
        {
            Id = id,
            Studentnummer = "25012345",
            Naam = "Jelle"
        };

        return Ok(student);
    }
}