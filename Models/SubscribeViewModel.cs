using System.ComponentModel.DataAnnotations;

namespace UnsoocialLandingPage.Models
{
	public class SubscribeViewModel
	{
		[Required(ErrorMessage = "Please enter your email address.")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; }
	}
}
