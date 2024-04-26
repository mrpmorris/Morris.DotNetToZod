#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.TestModels;

public class ClassWithRequiredString
{
	[Required(AllowEmptyStrings = true, ErrorMessage = "This is required"), EmailAddress]
	public string Name { get; set; } = null!;
}


public class UserProfile
{
	[Required(ErrorMessage = "Username is required.")]
	public string Username { get; set; }

	[StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
	public string Name { get; set; }

	[EmailAddress]
	public string Email { get; set; }

	[Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
	public int Age { get; set; }

	[Phone(ErrorMessage = "Invalid phone number format.")]
	public string Phone { get; set; }

	[Required, Url]
	public string Website { get; set; }

	[RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
	public string Address { get; set; }

	public int? OptionalInt { get; set; }
}
