﻿// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace AspNetPatchSample.Book.Data
{
  using AspNetPatchSample.Author;
  using AspNetPatchSample.Author.Data;
  using AspNetPatchSample.Data;

  /// <summary>Represents a book entity.</summary>
  public sealed class BookEntity : EntityBase, IBookEntity
  {
    /// <summary>Initializes a new instance of the <see cref="AspNetPatchSample.Book.Data.BookEntity"/> class.</summary>
    public BookEntity()
    {
      Title       = string.Empty;
      Description = string.Empty;
      BookAuthors = new List<AuthorEntity>();
    }

    /// <summary>Initializes a new instance of the <see cref="AspNetPatchSample.Book.Data.BookEntity"/> class.</summary>
    /// <param name="bookIdentity">An object that represents an identity of a book.</param>
    public BookEntity(IBookIdentity bookIdentity) : this()
    {
      BookId = bookIdentity.BookId;
    }

    /// <summary>Initializes a new instance of the <see cref="AspNetPatchSample.Book.Data.BookEntity"/> class.</summary>
    /// <param name="bookEntity">An object that represents a book entity.</param>
    public BookEntity(IBookEntity bookEntity) : this((IBookIdentity)bookEntity)
    {
      Title       = bookEntity.Title;
      Description = bookEntity.Description;
      Pages       = bookEntity.Pages;
      BookAuthors = bookEntity.Authors.Select(entity => new AuthorEntity(entity))
                                      .ToList();
    }

    /// <summary>Gets an object that represents an ID of a book.</summary>
    public Guid BookId { get => Id; set => Id = value; }

    /// <summary>Gets an object that represents a title of a book.</summary>
    public string Title { get; set; }

    /// <summary>Gets an object that represents a description of a book.</summary>
    public string Description { get; set; }

    /// <summary>Gets an object that represents a description of a book.</summary>
    public int Pages { get; set; }

    /// <summary>Gets an object that represents a collection of authors of this book.</summary>
    public ICollection<AuthorEntity> BookAuthors { get; set; }

    /// <summary>Gets an object that represents a collection of authors of this book.</summary>
    public IEnumerable<IAuthorEntity> Authors
    {
      get => BookAuthors;
      set => BookAuthors = value.Select(entity => new AuthorEntity(entity)).ToList();
    }

    protected override void Update(object newEntity, string property)
    {
      if (property != nameof(Authors))
      {
        base.Update(newEntity, property);

        return;
      }

      var newBookEntity = (IBookEntity)newEntity;

      var newAuthors = newBookEntity.Authors.Select(entity => entity.AuthorId)
                                            .ToHashSet();
      var exitingAuthors = Authors.Select(entity => entity.AuthorId)
                                  .ToHashSet();

      var deletingAuthors = BookAuthors.Where(entity => !newAuthors.Contains(entity.AuthorId))
                                       .ToList();

      foreach (var authorEntity in deletingAuthors)
      {
        BookAuthors.Remove(authorEntity);
      }

      var addingAuthors = newBookEntity.Authors.Where(entity => !exitingAuthors.Contains(entity.AuthorId))
                                               .ToList();

      foreach (var authorEntity in addingAuthors)
      {
        BookAuthors.Add(new AuthorEntity(authorEntity));
      }
    }
  }
}
