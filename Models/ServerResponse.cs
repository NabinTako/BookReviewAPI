namespace BookReview.Models {
	public class ServerResponse<T> {

		public T? Data { get; set; }
		public bool Sucess { get; set; } = true;
		public string Message { get; set; } = "";
	}
}
