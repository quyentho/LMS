# IStudentServiceWithRepository Interface

## Overview

The `IStudentServiceWithRepository` interface is an enhanced version of the student service that leverages the Generic Repository Pattern and Unit of Work. This interface provides all the functionality of the original `IStudentService` plus additional features that take advantage of the repository pattern.

## Interface Definition

```csharp
public interface IStudentServiceWithRepository
{
    // Methods from original IStudentService
    IEnumerable<StudentViewModel> GetStudent(int? studentId);
    IEnumerable<StudentViewModel> GetStudents();

    // Enhanced async methods with repository pattern
    Task<int> PostStudentAsync(StudentCreateViewModel studentCreateViewModel);
    Task<bool> UpdateStudentAsync(int id, StudentCreateViewModel studentUpdateViewModel);
    Task<bool> SoftDeleteStudentAsync(int id);
    Task<StudentViewModel?> GetStudentByIdAsync(int id);
    Task<IEnumerable<StudentViewModel>> SearchStudentsAsync(string searchTerm);

    // Advanced repository pattern features
    Task<bool> TransferStudentToNewSchoolAsync(int studentId, int newSchoolId);
}
```

## Key Features

### 1. **Backward Compatibility**

- Maintains all original `IStudentService` methods
- Existing code can be migrated gradually

### 2. **Async Support**

- All CRUD operations have async versions
- Better performance for I/O operations
- Non-blocking execution

### 3. **Enhanced Query Capabilities**

- `SearchStudentsAsync`: Search by first name, last name, or school name
- Efficient querying with includes for related entities
- Built-in filtering for soft-deleted records

### 4. **Transaction Support**

- `TransferStudentToNewSchoolAsync`: Demonstrates complex operations with transaction rollback
- Ensures data consistency across multiple operations

### 5. **Soft Delete Support**

- `SoftDeleteStudentAsync`: Uses the repository's built-in soft delete functionality
- Preserves data while marking records as deleted

## Usage Examples

### Dependency Injection

```csharp
// In your controller or service
public class StudentController : ControllerBase
{
    private readonly IStudentServiceWithRepository _studentService;

    public StudentController(IStudentServiceWithRepository studentService)
    {
        _studentService = studentService;
    }
}
```

### Basic CRUD Operations

```csharp
// Get student by ID (async)
var student = await _studentService.GetStudentByIdAsync(1);

// Create new student
var newStudentId = await _studentService.PostStudentAsync(new StudentCreateViewModel
{
    FirstName = "John",
    LastName = "Doe",
    SchoolId = 1,
    // ... other properties
});

// Update student
var updated = await _studentService.UpdateStudentAsync(1, updatedStudentData);

// Soft delete student
var deleted = await _studentService.SoftDeleteStudentAsync(1);
```

### Advanced Features

```csharp
// Search students
var searchResults = await _studentService.SearchStudentsAsync("John");

// Transfer student with transaction support
var transferred = await _studentService.TransferStudentToNewSchoolAsync(studentId: 1, newSchoolId: 2);
```

### Cached Operations

```csharp
// Get all students (uses memory cache)
var allStudents = _studentService.GetStudents();

// Get specific students (direct repository query)
var filteredStudents = _studentService.GetStudent(studentId: 1);
```

## Benefits Over Original IStudentService

| Feature                 | Original IStudentService | IStudentServiceWithRepository   |
| ----------------------- | ------------------------ | ------------------------------- |
| **Async Support**       | Limited                  | Full async/await support        |
| **Transaction Support** | No                       | Built-in transaction management |
| **Soft Delete**         | Manual implementation    | Built-in repository support     |
| **Search Capabilities** | Basic                    | Advanced search with includes   |
| **Error Handling**      | Basic                    | Enhanced with proper exceptions |
| **Performance**         | Direct DbContext queries | Optimized repository queries    |
| **Testability**         | Moderate                 | High (mockable repositories)    |
| **Maintainability**     | Lower                    | Higher (separation of concerns) |

## Implementation Details

The `StudentServiceWithRepository` class implements this interface using:

- **IUnitOfWork**: For repository management and transactions
- **IMapper**: For entity-to-DTO mapping
- **IMemoryCache**: For performance optimization
- **Generic Repository Pattern**: For standardized data access

## Migration Guide

### From IStudentService to IStudentServiceWithRepository

1. **Update Dependency Injection**:

   ```csharp
   // Old
   services.AddScoped<IStudentService, StudentService>();

   // New (both can coexist)
   services.AddScoped<IStudentService, StudentService>();
   services.AddScoped<IStudentServiceWithRepository, StudentServiceWithRepository>();
   ```

2. **Update Controller Dependencies**:

   ```csharp
   // Old
   public StudentController(IStudentService studentService)

   // New
   public StudentController(IStudentServiceWithRepository studentService)
   ```

3. **Update Method Calls**:

   ```csharp
   // Old
   var student = studentService.GetStudent(1).FirstOrDefault();

   // New
   var student = await studentService.GetStudentByIdAsync(1);
   ```

## Error Handling

The interface methods handle errors appropriately:

- **ArgumentException**: Thrown when referenced entities (like School) don't exist
- **Return false**: For operations that fail due to entity not found
- **Return null**: For queries that don't find matching entities
- **Transaction Rollback**: Automatic rollback on errors in complex operations

## Performance Considerations

- **Caching**: The `GetStudents()` method uses memory cache with 30-second sliding expiration
- **Lazy Loading**: Repository queries only load required data
- **Includes**: Related entities (like School) are efficiently loaded when needed
- **Async Operations**: Non-blocking I/O for better scalability

## Example Controller

See `StudentRepositoryController.cs` for a complete example of how to use all the interface methods in a Web API controller.

## Registration

The interface is registered in dependency injection:

```csharp
// In AddDependencyInjection.cs
serviceCollection.AddScoped<IStudentServiceWithRepository, StudentServiceWithRepository>();
```

This allows the interface to be injected anywhere in your application.
