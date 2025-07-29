using CaseService.API.CaseService.Domain.Entities;
using FluentAssertions;
using System.Threading.Tasks;

public class CaseTests
{
    [Fact]
    public void Create_valid_input_returns_submitted_case()
    {
        var email = "  USER@example.com  ";   // intentionally padded / mixed-case
        var description = "Need orthopedic consult";
        var speciality = "Orthopedics";

        var @case = Case.Create(email, description, speciality,"Cardiology");

        @case.Id.Should().NotBeEmpty();                      
        @case.Email.Should().Be("USER@example.com");             // trimmed
        @case.Description.Should().Be(description);
        @case.Speciality.Should().Be(speciality);
        @case.Status.Should().Be("Submitted");                    
        @case.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5)); //Time Span should be varibale
    }

    [Theory]
    [InlineData(null, "desc", "spec", "Email is required.")]
    [InlineData("   ", "desc", "spec", "Email is required.")]
    [InlineData("bad-email", "desc", "spec", "Email format is invalid.")]
    [InlineData("a@b.c", null, "spec", "Description is required.")]
    [InlineData("a@b.c", " ", "spec", "Description is required.")]
    [InlineData("a@b.c", "desc", null, "Speciality is required.")]
    [InlineData("a@b.c", "desc", " ", "Speciality is required.")]
    public void Create_invalid_input_throws(
    string? email,
    string? description,
    string? speciality,
    string expectedMessage)
    {
        Action act = () => Case.Create(email!, description!, speciality!, "Cardiology");


        act.Should()
           .Throw<Exception>()
           .WithMessage(expectedMessage);
    }

    [Fact]
    public void MoveToInReview_from_Submitted_succeeds()
    {
        var @case = Case.Create(
            email: "user@example.com",
            description: "Need ortho consult",
            speciality: "Orthopedics",
            title: "anything"
            );

        @case.MoveToInReview();

        @case.Status.Should().Be("InReview");  
    }

    [Fact]
    public void MoveToInReview_from_non_Submitted_throws()
    {
        var @case = Case.Create(
            email: "user@example.com",
            description: "Need ortho consult",
            speciality: "Orthopedics",
            title: "anything");

        @case.MoveToInReview();  

        
        Action act = () => @case.MoveToInReview();   // attempt the transition again

        act.Should()
           .Throw<Exception>()
           .WithMessage("Can only move submitted cases to InReview.");
    }

    [Fact]
    public void Finish_from_InReview_succeeds()
    {
        var @case = Case.Create(
            email: "user@example.com",
            description: "Need ortho consult",
            speciality: "Orthopedics", title: "anything");

        @case.MoveToInReview();                

        @case.Finish();                        

      
        @case.Status.Should().Be("Finished"); 
    }
    [Fact]
    public void Finish_from_non_InReview_throws()
    {
        
        var @case = Case.Create(
            email: "user@example.com",
            description: "Need ortho consult",
            speciality: "Orthopedics", title: "anything");

        Action act = () => @case.Finish();

        act.Should()
           .Throw<Exception>()
           .WithMessage("Can only finish cases that are InReview.");
    }

    [Fact]
    public void Finish_after_already_Finished_throws()
    {
        var @case = Case.Create(
            email: "user@example.com",
            description: "Need ortho consult",
            speciality: "Orthopedics",
            title:"anything");

        @case.MoveToInReview();   
        @case.Finish();           

        Action act = () => @case.Finish();

        act.Should()
           .Throw<Exception>()
           .WithMessage("Case is already Finished.");   
    }



}
