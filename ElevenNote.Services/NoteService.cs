using ElevenNote.Models;
using ElevenNote.Data;
using ElevenNote.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userId;

        public NoteService(Guid userId)
        {
            _userId = userId;
        }

        /// <summary>
        /// Get all notes for the current user
        /// </summary>
        /// <returns>The currecnt user's notes</returns>

        public IEnumerable<NoteListItemModel> GetNotes()
        {

            using (var context = new ElevenNoteDbContext())
            {
                var query =
                    context
                        .Notes
                        .Where(e => e.OwnerId == _userId)
                        .Select(
                            e => new NoteListItemModel
                                {
                                    NoteId = e.NoteId,
                                    Title = e.Title,
                                    CreatedUtc = e.CreatedUtc
                                }
                            );

                return query.ToArray();
            }
        }

        /// <summary>
        /// create a new note for current user
        /// </summary>
        /// <param name="model">The model to base the new note upon</param>
        /// <returns>A boolean indicating whether creating a note was successful</returns>

        public bool CreateNote(NoteCreateModel model)
        {
            var entity =
                new NoteEntity
                {
                    OwnerId = _userId,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedUtc = DateTimeOffset.UtcNow
                };
            using (var context = new ElevenNoteDbContext())
            {
                context.Notes.Add(entity);

                return context.SaveChanges() == 1;
            }
        }

        /// <summary>
        /// Gets a particular note for a current user
        /// </summary>
        /// <param name="noteId">The id for the note to retreive</param>
        /// <returns>the specified note</returns>

        public NoteDetailModel GetNoteById(int noteId)
        {
            using (var ctx = new ElevenNoteDbContext())
            {
                var entity =
                    ctx
                        .Notes
                        .Single(e => e.NoteId == noteId && e.OwnerId == _userId);

                return
                    new NoteDetailModel
                    {
                        NoteId = entity.NoteId,
                        Title = entity.Title,
                        Content = entity.Content,
                        CreatedUtc = entity.CreatedUtc,
                        ModifiedUtc = entity.ModifiedUtc
                    };

            }
        }

        public bool UpdateNote(NoteEditModel model)
        {
            using (var context = new ElevenNoteDbContext())
            {
                var entity =
                    context
                        .Notes
                        .Single(e => e.NoteId == model.NoteId && e.OwnerId == _userId);

                entity.Title = model.Title;
                entity.Content = model.Content;
                entity.ModifiedUtc = DateTimeOffset.UtcNow;

                return context.SaveChanges() == 1;
            }
        }

        public bool DeleteNote(int noteId)
        {
            using (var context = new ElevenNoteDbContext())
            {
                var entity =
                    context
                        .Notes
                        .Single(e => e.NoteId == noteId && e.OwnerId == _userId);
                context.Notes.Remove(entity);

                return context.SaveChanges() == 1;
            }
        }
    }
}
