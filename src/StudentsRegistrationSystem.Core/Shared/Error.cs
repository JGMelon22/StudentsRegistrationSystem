using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsRegistrationSystem.Core.Shared;

public sealed record Error(int Code, string Description)
{

    // Course Errors
    public static readonly Error NotFound = new(100, "Course not found");
    public static readonly Error AlreadyExists = new(101, "A course with this name already exists");
    public static readonly Error InvalidName = new(102, "Course name cannot be empty");
    public static readonly Error InvalidDescription = new(103, "Course description cannot be empty");

    // Sudent Errors
    public static readonly Error NotFound = new(200, "Student not found");
    public static readonly Error InvalidName = new(201, "Student name cannot be empty");
    public static readonly Error InvalidEmail = new(202, "Invalid email address");
    public static readonly Error Underage = new(203, "Student must be at least 18 years old");
    public static readonly Error AlreadyExists = new(204, "A student with this email already exists");

    // Enrollment Errors
    public static readonly Error NotFound = new(300, "Enrollment not found");
    public static readonly Error StudentNotFound = new(301, "Student not found for enrollment");
    public static readonly Error CourseNotFound = new(302, "Course not found for enrollment");
    public static readonly Error AlreadyEnrolled = new(303, "Student is already enrolled in this course");
    public static readonly Error NotEnrolled = new(304, "Student is not enrolled in this course");
}