using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Departments;
using Rise.Domain.Users;
using Rise.Shared.Departments;
using Rise.Domain.Education;
using Rise.Domain.Entities;
using Rise.Domain.Infrastructure;
using Serilog;

namespace Rise.Persistence;

/// <summary>
/// Seeds the database
/// </summary>
/// <param name="dbContext"></param>
/// <param name="roleManager"></param>
/// <param name="userManager"></param>
public class DbSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    const string PasswordDefault = "A1b2C3!";
    public async Task SeedAsync()
    {
        Log.Information("Seeding Database");
        await RolesAsync();
        await DepartmentsAsync();
        await UsersAsync();
        await AssignDepartmentsAsync();
        await MenuAsync();
        await dbContext.SaveChangesAsync();
    }

    private async Task RolesAsync()
    {
        if (dbContext.Roles.Any())
            return;

        await roleManager.CreateAsync(new IdentityRole("Administrator"));
        await roleManager.CreateAsync(new IdentityRole("Secretary"));
        await roleManager.CreateAsync(new IdentityRole("Student"));
        await roleManager.CreateAsync(new IdentityRole("Employee"));
    }
    
    private async Task AssignDepartmentsAsync()
    {
        var departments = await dbContext.Departments.ToListAsync();
        var employees = await dbContext.Employees.ToListAsync();
        var students = await dbContext.Students.ToListAsync();

        var itDepartment = departments.First(d => d.Name.Contains("IT"));
        var managerEmployee = employees.First();

        itDepartment.Manager = managerEmployee;

        managerEmployee.Department = itDepartment;
    
        foreach (var student in students)
        {
            student.Department = itDepartment;
        }

        await dbContext.SaveChangesAsync();
    }
    
    private async Task DepartmentsAsync()
    {
        if (dbContext.Departments.Any())
            return;
    
        dbContext.Departments.AddRange(
            // Departementen
            new Department{ Name = "Bedrijf en Organisatie", Description = "Programma's in bedrijfskunde, financiën, marketing en organisatorische ontwikkeling", DepartmentType = DepartmentType.Department},
            new Department{ Name = "Bio- en Industriële Technologie", Description = "Studies in biotechnologie, agrowetenschappen, chemie en industriële technologie", DepartmentType = DepartmentType.Department },
            new Department{ Name = "Gezondheidszorg", Description = "Opleidingen in verpleegkunde, logopedie, audiologie en andere gezondheidsberoepen", DepartmentType = DepartmentType.Department },
            new Department{ Name = "IT en Digitale Innovatie", Description = "Opleidingen in toegepaste informatica, digitaal ontwerp en IT-beheer", DepartmentType = DepartmentType.Department },
            new Department{ Name = "Lerarenopleiding", Description = "Opleidingen voor toekomstige leraren in secundair en basisonderwijs", DepartmentType = DepartmentType.Department },
            new Department{ Name = "Omgeving", Description = "Focus op vastgoed, houttechnologie, landschapsarchitectuur en milieubeheer", DepartmentType = DepartmentType.Department },
            new Department{ Name = "Sociaal Werk", Description = "Programma's in sociaal werk, pedagogiek en gemeenschapsontwikkeling", DepartmentType = DepartmentType.Department },
            new Department{ Name = "GO5", Description = "School voor associate degrees in diverse praktische vakgebieden", DepartmentType = DepartmentType.Department },
            new Department{ Name = "KASK & Conservatorium", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Department },
        
            //TODO: Aanvullen descriptions 
            
            // Andere
            new Department{ Name = "Studentenraad", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Other },
            new Department{ Name = "Sportdienst", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Other },

            // Directies
            new Department{ Name = "Algemene directie", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie Onderwijsaangelegenheden", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie Onderzoeksaangelegenheden", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie Financiën en Personeel", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie IT", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie Infrasturctuur en Facilitair Beheer", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie Studentenvoorzieningen", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management },
            new Department{ Name = "Directie Communicatie", Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design", DepartmentType = DepartmentType.Management }
            );

        await dbContext.SaveChangesAsync();
    }
    private async Task UsersAsync()
    {
        if (dbContext.Users.Any())
            return;
    
        await dbContext.Roles.ToListAsync();
        var departments = await dbContext.Departments.ToListAsync();
        var defaultDepartment = departments.First(d => d.Name.Contains("IT")); // default, anders doet ie moeilijk
    
        var adminAccount = new IdentityUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(adminAccount, PasswordDefault);
    
        var secretaryAccount = new IdentityUser
        {
            UserName = "secretary@example.com",
            Email = "secretary@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(secretaryAccount, PasswordDefault);
    
        var studentAccount1 = new IdentityUser
        {
            UserName = "bob.thebuilder@student.hogent.be",
            Email = "bob.thebuilder@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(studentAccount1, PasswordDefault);
        var studentAccount2 = new IdentityUser
        {
            UserName = "gert.vanderstenen@student.hogent.be",
            Email = "gert.vanderstenen@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(studentAccount2, PasswordDefault);
        var managerAccount = new IdentityUser
        {
            UserName = "rudi.madalijns@hogent.be",
            Email = "rudi.madalijns@hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(managerAccount, PasswordDefault);
    
        await userManager.AddToRoleAsync(adminAccount, "Administrator");
        await userManager.AddToRoleAsync(secretaryAccount, "Secretary");
        await userManager.AddToRoleAsync(studentAccount1, "Student");
        await userManager.AddToRoleAsync(studentAccount2, "Student");
        await userManager.AddToRoleAsync(managerAccount, "Employee");
    
        var managerEmployee = new Employee 
        {
            Employeenumber = "E001",
            AccountId = managerAccount.Id,
            Email = new EmailAddress("rudi.madalijns@hogent.be"),
            Firstname = "Rudi",
            Lastname = "Madalijns",
            Department = defaultDepartment
        };
        var student1 = new Student 
        {
            StudentNumber = "S12345",
            AccountId = studentAccount1.Id,
            Email = new EmailAddress("bob.thebuilder@student.hogent.be"),
            Firstname = "Bob",
            Lastname = "The Builder",
            Department = defaultDepartment
        };
        var student2 = new Student 
        {
            StudentNumber = "S12346",
            AccountId = studentAccount2.Id,
            Email = new EmailAddress("gert.vanderstenen@student.hogent.be"),
            Firstname = "Gert",
            Lastname = "Van Der Stenen",
            Department = defaultDepartment
        };

        dbContext.Employees.Add(managerEmployee);
        dbContext.Students.AddRange(student1, student2);
        await dbContext.SaveChangesAsync();
    }
    
    private async Task MenuAsync()
    {
        if (dbContext.MenuItems.Any())
            return;

        List<MenuItem> list = new List<MenuItem>();

        var menuItem1 = new MenuItem
        {
            Name = "Spaghetti",
            Description = "Lekkere spaghetti bolognese",
            IsVeganAndHalal = true,
            Category = FoodCategory.WarmeMaaltijd
        };
        list.Add(menuItem1);

        var menuItem2 = new MenuItem
        {
            Name = "Margherita Pizza",
            Description = "Classic pizza met tomaat en mozzarella",
            IsVeganAndHalal = false,
            Category = FoodCategory.WarmeMaaltijd
        };
        list.Add(menuItem2);

        var menuItem3 = new MenuItem
        {
            Name = "Caesar Salad",
            Description = "Salade met kip, croutons en Parmezaanse kaas",
            IsVeganAndHalal = false,
            Category = FoodCategory.Groenten
        };
        list.Add(menuItem3);

        var menuItem4 = new MenuItem
        {
            Name = "Falafel Wrap",
            Description = "Wrap met falafel, hummus en groenten",
            IsVeganAndHalal = true,
            Category = FoodCategory.Groenten
        };
        list.Add(menuItem4);

        var menuItem5 = new MenuItem
        {
            Name = "Tomato Soup",
            Description = "Verse tomatensoep met kruiden",
            IsVeganAndHalal = true,
            Category = FoodCategory.Soep
        };

        list.Add(menuItem5);

        var menuItem6 = new MenuItem
        {
            Name = "Grilled Chicken",
            Description = "Gegrilde kip met aardappelen en groenten",
            IsVeganAndHalal = false,
            Category = FoodCategory.WarmeMaaltijd
        };
        list.Add(menuItem6);

        var menuItem7 = new MenuItem
        {
            Name = "Veggie Burger",
            Description = "Burger gemaakt van groenten en bonen",
            IsVeganAndHalal = true,
            Category = FoodCategory.Groenten
        };
        list.Add(menuItem7);

        var menuItem8 = new MenuItem
        {
            Name = "Fruit Salad",
            Description = "Vers gesneden fruit mix",
            IsVeganAndHalal = true,
            Category = FoodCategory.Wekelijks
        };
        list.Add(menuItem8);

        var menuItem9 = new MenuItem
        {
            Name = "Cheesecake",
            Description = "Romige cheesecake met aardbeien",
            IsVeganAndHalal = false,
            Category = FoodCategory.Dessert
        };
        list.Add(menuItem9);

        var menuItem10 = new MenuItem
        {
            Name = "Chocolate Muffin",
            Description = "Muffin met pure chocolade",
            IsVeganAndHalal = false,
            Category = FoodCategory.Dessert
        };
        list.Add(menuItem10);

        await dbContext.MenuItems.AddRangeAsync(list);

        var Openingtime1 = new Openingtime
        {
            StartTime = new TimeOnly(6, 0),
            EndTime = new TimeOnly(20, 0),
            Date = new DateOnly(2025, 10, 9)

        };
        
        var studfield1 = new StudyField
        {
            Name = "Toegepaste informatica",
        };
        
        var course1 = new Course
        {
            Name = "RISE",
            Description = "Een groepswerk waar je een proof of concept uitwerkt.",
            StudyField = studfield1

        };

        // Add deadlines seeding
        var deadline1 = new Deadline
        {
            Title = "RISE Project Proposal",
            Description = "Submit initial project proposal for RISE course",
            DueDate = new DateTime(2025, 11, 1),
            StartDate = new DateTime(2025, 10, 20)
        };

        var deadline2 = new Deadline
        {
            Title = "RISE Midterm Review",
            Description = "Present midterm progress for RISE project",
            DueDate = new DateTime(2025, 12, 15),
            StartDate = new DateTime(2025, 11, 1)
        };

        var deadline3 = new Deadline
        {
            Title = "RISE Final Submission",
            Description = "Submit final proof of concept and report",
            DueDate = new DateTime(2026, 1, 31),
            StartDate = new DateTime(2025, 12, 15)
        };
        
        // Fetch students to assign courses and deadlines
        var students = await dbContext.Students.ToListAsync();
        var student1 = students.FirstOrDefault(s => s.StudentNumber == "S12345");
        var student2 = students.FirstOrDefault(s => s.StudentNumber == "S12346");

        if (student1 != null && student2 != null)
        {
            // Assign course to students
            course1.EnrollStudent(student1);
            course1.EnrollStudent(student2);

            // Add deadlines to course
            course1.AddDeadline(deadline1);
            course1.AddDeadline(deadline2);
            course1.AddDeadline(deadline3);

            // Assign deadlines to students
            deadline1.AssignStudent(student1);
            deadline1.AssignStudent(student2);

            deadline2.AssignStudent(student1);
            deadline2.AssignStudent(student2);

            deadline3.AssignStudent(student1);
            deadline3.AssignStudent(student2);

            dbContext.Update(student1);
            dbContext.Update(student2);
        }

        // Add education entities to context
        dbContext.Studyfields.Add(studfield1);
        dbContext.Courses.Add(course1);
        dbContext.Deadlines.AddRange(deadline1, deadline2, deadline3);
        
        var classroom1 = new Classroom
        {
            Coordinates = "0.1",
            Lesson = new Lesson(),
            Building = null

        };
        
        var restoNames = new List<string>
        {
            "Ledeganck",
            "Mercator G.",
            "Bijloke",
            "Schoonmeersen B.",
            "Schoonmeersen P.",
            "Schoonmeersen D."
        };
        
        var menu1 = new Menu
        {
            StartDate = DateTime.Now
        };
        
        menu1.AddMenuToDay("Ma", list);
        menu1.AddMenuToDay("Di", list);
        menu1.AddMenuToDay("Wo", list);
        menu1.AddMenuToDay("Do", list);
        menu1.AddMenuToDay("Vr", list);
        
        var Restos = restoNames.Select((name, index) => new Resto
        {
            Menu = menu1,
            Coordinates = $"{index}, {index + 1}",
            Building = null,
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Name = $"Resto {name}"
        }).ToList();
        
        var dep1 = new Department
        {
            Name = "Digitale innovatie",
        };
        
        var building1 = new Building
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Restos = new List<Resto> { Restos[3] },
            Campus = null,
            Classrooms = new List<Classroom> { classroom1 },
            Name = "Schoonmeersen gebouw B"

        };
        
        var building2 = new Building
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Restos = new List<Resto> { Restos[0] },
            Campus = null,
            Classrooms = new List<Classroom> { classroom1 },
            Name = "Ledeganck gebouw A"

        };
        
        var building3 = new Building
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Restos = new List<Resto> { Restos[1] },
            Campus = null,
            Classrooms = new List<Classroom> { classroom1 },
            Name = "Mercator gebouw G"

        };
        
        var building4 = new Building
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Restos = new List<Resto> { Restos[2] },
            Campus = null,
            Classrooms = new List<Classroom> { classroom1 },
            Name = "Bijloke gebouw B"

        };

        var building5 = new Building
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Restos = new List<Resto> { Restos[4] },
            Campus = null,
            Classrooms = new List<Classroom> { classroom1 },
            Name = "Schoonmeersen gebouw P"

        };
        
        var building6 = new Building
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Restos = new List<Resto> { Restos[5] },
            Campus = null,
            Classrooms = new List<Classroom> { classroom1 },
            Name = "Schoonmeersen gebouw D"

        };


        var campus1 = new Campus
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Events = new List<Event> { },
            Emergencies = new List<Emergency> { },
            Departements = new List<Department> { dep1 },
            Map = "",
            Buildings = new List<Building> { building1, building5, building6 },
            Name = "Schoonmeersen"

        };

        var campus2 = new Campus
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Events = new List<Event> { },
            Emergencies = new List<Emergency> { },
            Departements = new List<Department> { dep1 },
            Map = "",
            Buildings = new List<Building> { building3 },
            Name = "Mercator"

        };

        var campus3 = new Campus
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Events = new List<Event> { },
            Emergencies = new List<Emergency> { },
            Departements = new List<Department> { dep1 },
            Map = "",
            Buildings = new List<Building> { building4 },
            Name = "Bijloke"

        };

        var campus4 = new Campus
        {
            Openingtimes = new List<Openingtime> { Openingtime1 },
            Events = new List<Event> { },
            Emergencies = new List<Emergency> { },
            Departements = new List<Department> { dep1 },
            Map = "",
            Buildings = new List<Building> { building2 },
            Name = "Ledeganck"

        };

        classroom1.Building = building1;
        building1.Campus = campus1;
        Restos[0].Building = building2;
        Restos[1].Building = building3;
        Restos[2].Building = building4;
        Restos[3].Building = building1;
        Restos[4].Building = building5;
        Restos[5].Building = building6;

        building5.Campus = campus1;
        building6.Campus = campus1;
        building1.Campus = campus1;
        building2.Campus = campus4;
        building3.Campus = campus2;
        building4.Campus = campus3;
        studfield1.Courses = new List<Course> { course1 };
        studfield1.Departement = dep1;

        await dbContext.Menus.AddAsync(menu1);
        await dbContext.Restos.AddRangeAsync(Restos);
    }
    
}