# Refactoring Untestable Code: From Anti-patterns to Testable Design

## Overview

This document demonstrates common untestable code patterns (anti-patterns) and their refactored, testable counterparts. The examples show how to transform hard-to-test code into maintainable, testable code using dependency injection and abstraction patterns.

## Anti-patterns and Their Solutions

### 1. Static Dependencies
**Problem**: Static methods like `DateTime.Now`, `Console.WriteLine`, `File.ReadAllText` cannot be mocked.

**Untestable Code**:
```csharp
public string CreateCourseWithCurrentTime(string courseName)
{
    var currentTime = DateTime.Now; // Cannot be mocked
    // ... rest of implementation
}
```

**Testable Solution**:
```csharp
public string CreateCourseWithCurrentTime(string courseName)
{
    var currentTime = _dateTimeProvider.Now; // Can be mocked
    // ... rest of implementation
}
```

**Key Technique**: Wrap static calls in interfaces (`IDateTimeProvider`, `IFileService`, etc.)

### 2. Direct Resource Access
**Problem**: Direct access to file system, console, or network resources makes tests dependent on external state.

**Untestable Code**:
```csharp
public async Task SaveCourseToFileAsync(Course course)
{
    await File.WriteAllTextAsync(path, content); // Requires actual file system
}
```

**Testable Solution**:
```csharp
public async Task SaveCourseToFileAsync(Course course)
{
    await _fileService.WriteAllTextAsync(path, content); // Can be mocked
}
```

**Key Technique**: Abstract external resources behind interfaces.

### 3. Non-Deterministic Behavior
**Problem**: Random numbers, GUIDs, and other non-deterministic operations make tests unreliable.

**Untestable Code**:
```csharp
public string GenerateCourseCode()
{
    var random = new Random(); // Non-deterministic
    return random.Next(1000, 9999).ToString();
}
```

**Testable Solution**:
```csharp
public string GenerateCourseCode()
{
    return _randomService.GenerateString(6); // Deterministic in tests
}
```

**Key Technique**: Inject randomness and other non-deterministic behavior.

### 4. Hard-to-Verify Side Effects
**Problem**: Operations like console output, file logging, or HTTP calls are hard to verify in tests.

**Untestable Code**:
```csharp
public void ProcessCourse(Course course)
{
    Console.WriteLine($"Processing {course.Name}"); // Hard to verify
    // ... processing logic
}
```

**Testable Solution**:
```csharp
public void ProcessCourse(Course course)
{
    _consoleService.WriteLine($"Processing {course.Name}"); // Can be verified
    // ... processing logic
}
```

**Key Technique**: Use mockable services that can be verified in tests.

### 5. Mixed Concerns
**Problem**: Business logic mixed with infrastructure concerns makes testing complex.

**Untestable Code**:
```csharp
public async Task<bool> EnrollStudentAsync(int studentId, int courseId)
{
    // Business logic mixed with HTTP calls, file I/O, console output
    using var client = new HttpClient();
    var response = await client.GetAsync($"https://api.example.com/students/{studentId}");
    
    if (!response.IsSuccessStatusCode)
    {
        File.AppendAllText("errors.txt", $"Student {studentId} not found");
        Console.WriteLine("Error occurred");
        return false;
    }
    
    // ... more mixed concerns
}
```

**Testable Solution**:
```csharp
public async Task<bool> EnrollStudentAsync(int studentId, int courseId)
{
    // Pure business logic with injected dependencies
    var response = await _httpService.GetResponseAsync($"https://api.example.com/students/{studentId}");
    
    if (!response.IsSuccessStatusCode)
    {
        await _loggerService.LogToFileAsync($"Student {studentId} not found", "errors.txt");
        _consoleService.WriteLine("Error occurred");
        return false;
    }
    
    // ... clean business logic
}
```

**Key Technique**: Separate business logic from infrastructure concerns using dependency injection.

## Refactoring Techniques

### 1. Dependency Injection
- **What**: Inject dependencies through constructor parameters instead of creating them internally
- **Why**: Makes dependencies explicit and replaceable with mocks
- **When**: Always for external dependencies and cross-cutting concerns

### 2. Interface Segregation
- **What**: Create small, focused interfaces for specific capabilities
- **Why**: Makes mocking easier and follows Single Responsibility Principle
- **When**: For any external dependency or complex operation

### 3. Abstraction Layer
- **What**: Create wrapper interfaces around static methods and sealed classes
- **Why**: Provides a mockable layer over unmockable static APIs
- **When**: For DateTime, File, Console, Environment, Random, etc.

### 4. Pure Functions
- **What**: Functions that depend only on their parameters and have no side effects
- **Why**: Easiest to test and reason about
- **When**: For business logic and calculations

### 5. Command/Query Separation
- **What**: Separate methods that change state (commands) from those that return data (queries)
- **Why**: Makes testing more predictable and code more maintainable
- **When**: For service methods and business operations

## Testing Benefits

### Before Refactoring (Untestable Code)
- Tests require actual file system
- Tests are non-deterministic
- Tests are slow due to real HTTP calls and delays
- Tests are brittle due to external dependencies
- Side effects are hard to verify
- Tests require complex setup and teardown

### After Refactoring (Testable Code)
- Tests use mocks and are isolated
- Tests are deterministic and reliable
- Tests are fast (no real I/O or network calls)
- Tests are independent of external state
- All behavior can be verified through mocks
- Tests are simple and focused

## Example Test Structure

```csharp
[Fact]
public async Task EnrollStudentAsync_WithValidData_ShouldReturnTrue()
{
    // Arrange - Set up mocks with expected behavior
    var course = new Course { Id = 1, Name = "Test Course" };
    _courseRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(course);
    _httpServiceMock.Setup(x => x.GetResponseAsync(It.IsAny<string>()))
                   .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
    _randomServiceMock.Setup(x => x.Next(100000, 999999)).Returns(123456);

    // Act - Call the method under test
    var result = await _service.EnrollStudentAsync(1, 1);

    // Assert - Verify the result and mock interactions
    Assert.True(result);
    _consoleServiceMock.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
}
```

## Best Practices

1. **Start with Interfaces**: Design interfaces first, then implement
2. **Keep Interfaces Small**: Follow Interface Segregation Principle
3. **Use Dependency Injection**: Register services in IoC container
4. **Mock External Dependencies**: Never hit real databases, files, or APIs in unit tests
5. **Verify Behavior**: Use mocks to verify that the right methods were called
6. **Separate Concerns**: Keep business logic separate from infrastructure
7. **Make Tests Deterministic**: Use mocks to control all external inputs
8. **Test Edge Cases**: Use mocks to simulate error conditions easily

## Common Mistakes to Avoid

1. **Static Dependencies**: Avoid calling static methods directly
2. **New Keyword**: Avoid using `new` for dependencies in business logic
3. **Mixed Concerns**: Don't mix business logic with infrastructure code
4. **Untestable Constructors**: Don't do work in constructors
5. **Hidden Dependencies**: Make all dependencies explicit through constructor injection
6. **Concrete Dependencies**: Always depend on abstractions, not concrete implementations

## Tools and Libraries

- **Testing Framework**: xUnit, NUnit, MSTest
- **Mocking Framework**: Moq, NSubstitute, FakeItEasy
- **IoC Container**: Built-in .NET DI, Autofac, Castle Windsor
- **HTTP Testing**: HttpClient mocking, WireMock
- **Database Testing**: Entity Framework in-memory provider, TestContainers

## Conclusion

Refactoring untestable code requires identifying dependencies and abstractions that make testing difficult, then systematically replacing them with injectable, mockable alternatives. This approach leads to more maintainable, testable, and reliable code that follows SOLID principles and clean architecture patterns.

The key is to separate what your code *does* (business logic) from *how* it does it (infrastructure concerns), making the "how" replaceable through dependency injection.