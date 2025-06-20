# Generic Repository Pattern Implementation

This document describes the implementation of the Generic Repository Pattern in the TodoWeb DataAccess layer.

## Overview

The Generic Repository Pattern provides a standardized way to access data across different entities while reducing code duplication and improving maintainability. This implementation includes:

- **Generic Repository Interface** (`IGenericRepository<T>`)
- **Generic Repository Implementation** (`GenericRepository<T>`)
- **Unit of Work Pattern** (`IUnitOfWork`, `UnitOfWork`)
- **Base Entity Interface** (`IBaseEntity`)

## Structure

```
TodoWeb.DataAccess/
├── Entities/
│   ├── IBaseEntity.cs          # Base interface for all entities
│   └── [Entity classes...]     # All entities now implement IBaseEntity
├── Repositories/
│   ├── IGenericRepository.cs   # Generic repository interface
│   ├── GenericRepository.cs    # Generic repository implementation
│   ├── IUnitOfWork.cs          # Unit of Work interface
│   └── UnitOfWork.cs           # Unit of Work implementation
```

## Key Features

### 1. IBaseEntity Interface

All entities implement this interface to ensure they have an `Id` property:

```csharp
public interface IBaseEntity
{
    int Id { get; set; }
}
```

### 2. Generic Repository Interface

Provides comprehensive CRUD operations:

- **Get Operations**: `GetById`, `GetAll`, `Find`, `FirstOrDefault` (both async and sync)
- **Query Operations**: `Query` with support for includes and predicates
- **Count Operations**: `Count`, `CountAsync`
- **Exists Operations**: `Exists`, `ExistsAsync`
- **Add Operations**: `Add`, `AddRange` (both async and sync)
- **Update Operations**: `Update`, `UpdateRange`
- **Remove Operations**: `Remove`, `RemoveRange`, `RemoveById`
- **Soft Delete Operations**: `SoftDelete`, `SoftDeleteById` (for entities implementing `IDelete`)

### 3. Unit of Work Pattern

Manages repositories and provides transaction support:

- **Repository Management**: Generic `Repository<T>()` method and specific repository properties
- **Transaction Support**: `BeginTransaction`, `CommitTransaction`, `RollbackTransaction`
- **Save Operations**: `SaveChanges`, `SaveChangesAsync`

## Usage Examples

### Basic Repository Usage

```csharp
public class StudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Student?> GetStudentAsync(int id)
    {
        return await _unitOfWork.StudentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
    {
        return await _unitOfWork.StudentRepository.FindAsync(s => s.Status != Status.Deleted);
    }
}
```

### Repository with Includes

```csharp
public async Task<Student?> GetStudentWithSchoolAsync(int id)
{
    return await _unitOfWork.StudentRepository.GetByIdAsync(id, s => s.School);
}

public async Task<IEnumerable<Student>> GetStudentsWithSchoolsAsync()
{
    return await _unitOfWork.StudentRepository.GetAllAsync(s => s.School);
}
```

### Complex Queries

```csharp
public IEnumerable<Student> GetStudentsByAge(int minAge, int maxAge)
{
    return _unitOfWork.StudentRepository.Query(
        s => s.Status != Status.Deleted && s.Age >= minAge && s.Age <= maxAge,
        s => s.School
    ).ToList();
}
```

### Transaction Usage

```csharp
public async Task<bool> TransferStudentAsync(int studentId, int newSchoolId)
{
    try
    {
        await _unitOfWork.BeginTransactionAsync();

        var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
        student.SchoolId = newSchoolId;
        _unitOfWork.StudentRepository.Update(student);

        // Additional operations...

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
        return true;
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

### Soft Delete Usage

```csharp
public async Task<bool> DeleteStudentAsync(int id)
{
    var student = await _unitOfWork.StudentRepository.GetByIdAsync(id);
    if (student == null) return false;

    _unitOfWork.StudentRepository.SoftDeleteById(id);
    await _unitOfWork.SaveChangesAsync();
    return true;
}
```

## Dependency Injection Registration

In `AddDependencyInjection.cs`:

```csharp
public static void AddService(this IServiceCollection serviceCollection)
{
    // Repository Pattern Registration
    serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

    // Other service registrations...
}
```

## Migration from Direct DbContext Usage

### Before (Direct DbContext)

```csharp
public class StudentService
{
    private readonly IApplicationDbContext _context;

    public async Task<Student?> GetStudentAsync(int id)
    {
        return await _context.Students.FindAsync(id);
    }

    public async Task<int> CreateStudentAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student.Id;
    }
}
```

### After (Repository Pattern)

```csharp
public class StudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Student?> GetStudentAsync(int id)
    {
        return await _unitOfWork.StudentRepository.GetByIdAsync(id);
    }

    public async Task<int> CreateStudentAsync(Student student)
    {
        await _unitOfWork.StudentRepository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return student.Id;
    }
}
```

## Benefits

1. **Code Reusability**: Common CRUD operations are implemented once
2. **Consistency**: Standardized data access patterns across all entities
3. **Testability**: Easy to mock repositories for unit testing
4. **Maintainability**: Changes to data access logic can be centralized
5. **Transaction Support**: Built-in transaction management
6. **Flexibility**: Support for both sync and async operations
7. **Soft Delete Support**: Built-in soft delete functionality for applicable entities

## Available Repository Properties

The Unit of Work provides direct access to repositories for commonly used entities:

- `ToDoRepository`
- `StudentRepository`
- `CourseRepository`
- `CourseStudentRepository`
- `SchoolRepository`
- `GradeRepository`
- `QuestionRepository`
- `ExamRepository`
- `ExamQuestionRepository`
- `ExamSubmissionRepository`
- `ExamSubmissionDetailRepository`
- `UserRepository`
- `RefreshTokenRepository`
- `AuditLogRepository`

You can also access any repository generically using `Repository<T>()` method.

## Best Practices

1. **Always use Unit of Work**: Inject `IUnitOfWork` instead of individual repositories
2. **Use transactions for complex operations**: Wrap multiple repository operations in transactions
3. **Leverage async methods**: Use async methods for better performance
4. **Include related data efficiently**: Use the includes parameter to fetch related entities
5. **Handle soft deletes**: Always check for soft-deleted entities in your business logic
6. **Cache when appropriate**: Implement caching at the service level, not repository level

## Example Files

- `ToDoServiceWithRepository.cs` - Shows updated ToDo service using repository pattern
- `StudentServiceWithRepository.cs` - Shows migrated Student service with advanced repository usage patterns
