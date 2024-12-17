using AiComp.Application.DTOs.RequestModel;
using System.ComponentModel.DataAnnotations;

namespace AiComp.Domain.Entities
{
    public class Profile : BaseEntity
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string? Gender { get; private set; }
        public string? Occupation { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Address { get; private set; }
        public string? FullNameOfNextOfKin { get; private set; }
        public string? ContactOfNextOfKin { get; private set; }
        public string? ProfilePicture { get; set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;

        public Profile(string firstName, string lastName, string gender, string occupation, string address, string phoneNumber, string contactOfNextOfKin, string fullNameOfNextOfKin, string profilePicture)
        {
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            Occupation = occupation;
            Address = address;
            PhoneNumber = phoneNumber;
            ContactOfNextOfKin = contactOfNextOfKin;
            FullNameOfNextOfKin = fullNameOfNextOfKin;
            ProfilePicture = profilePicture;
        }

        public void SetUserObject(User user)
        {
            User = user;
            UserId = user.Id;
        }

        public void UpdateProfile(ProfileUpdateRequestModel request)
        {
            FirstName = request.FirstName ?? FirstName;
            LastName = request.LastName ?? LastName;
            Gender = request.Gender ?? Gender;
            Occupation = request.Occupation ?? Occupation;
            Address = request.Address ?? Address;
            PhoneNumber = request.PhoneNumber ?? PhoneNumber;
            ContactOfNextOfKin = request.ContactOfNextOfKin ?? ContactOfNextOfKin;
            FullNameOfNextOfKin = request.FullNameOfNextOfKin ?? FullNameOfNextOfKin;
        }

        public void UpdateProfilePicture(string request)
        {
            ProfilePicture = request ?? ProfilePicture;
        }

        public void UpdateAge(DateTime dob)
        {
            DateOfBirth = dob;
        }
    }
}
