using System.Text.Json.Serialization;

namespace BookReview.Models {
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Tag {
		Horror = 2,
		Romance = 5,
		Sci_Fi = 3
	}
}
