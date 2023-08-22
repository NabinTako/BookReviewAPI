﻿
using BookReview.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Services.BookServices {
	public interface IBookService {

		public Task<ServerResponse<List<BookDto>>> GetAllBooks();
		public Task<ServerResponse<List<BookDto>>> AddBook(BookDto newBook);
		public Task<ServerResponse<List<BookDto>>> DeleteBook(int id);
		public Task<ServerResponse<BookDto>> GetBookById(int id);
		public Task<ServerResponse<BookDetailsDto>> GetBookDetailsById(int id);
		public Task<ServerResponse<List<BookDto>>> GetBookByTag(string tag);
	}
}
