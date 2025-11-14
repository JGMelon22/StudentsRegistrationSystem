namespace StudentsRegistrationSystem.Core.Shared;

public sealed record Error(int Code, string Description)
{
    // Course Errors
    public static readonly Error CourseNotFound = new(100, "Course not found");
    public static readonly Error CourseAlreadyExists = new(101, "A course with this name already exists");
    public static readonly Error CourseInvalidName = new(102, "Course name cannot be empty");
    public static readonly Error CourseInvalidDescription = new(103, "Course description cannot be empty");

    // Student Errors
    public static readonly Error StudentNotFound = new(200, "Student not found");
    public static readonly Error StudentInvalidName = new(201, "Student name cannot be empty");
    public static readonly Error StudentInvalidEmail = new(202, "Invalid email address");
    public static readonly Error StudentUnderage = new(203, "Student must be at least 18 years old");
    public static readonly Error StudentAlreadyExists = new(204, "A student with this email already exists");

    // Enrollment Errors
    public static readonly Error EnrollmentNotFound = new(300, "Enrollment not found");
    public static readonly Error EnrollmentStudentNotFound = new(301, "Student not found for enrollment");
    public static readonly Error EnrollmentCourseNotFound = new(302, "Course not found for enrollment");
    public static readonly Error EnrollmentAlreadyEnrolled = new(303, "Student is already enrolled in this course");
    public static readonly Error EnrollmentNotEnrolled = new(304, "Student is not enrolled in this course");

    // Technical/Infrastructure Errors
    public static readonly Error DatabaseError = new(900, "A database error occurred");
    public static readonly Error ServerError = new(999, "An unexpected server error occurred");
}