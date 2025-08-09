using Microsoft.EntityFrameworkCore;
using TodoMinimal.Application.Common;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Infrastructure.Persistence.Repositories
{
    public class NoteRepository(AppDbContext dbContext) : INoteRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task AddNote(Note note, CancellationToken cancellationToken)
        {
            _ = await _dbContext.AddAsync(note, cancellationToken);
        }

        public async Task<Note?> GetNote(int noteId, CancellationToken cancellationToken)
        {
            return await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
        }

        public async Task<PagedResult<Note>> GetNotesForUser(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<Note> query = _dbContext.Notes
                // .AsNoTracking()
                .Where(x => x.UserId == userId);

            List<Note> notes = await query
                .OrderBy(n => n.CreatedAt)
                .Skip((pageNumber - 1) * Math.Max(pageSize, 1))
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            int count = await query.CountAsync(cancellationToken);

            return new PagedResult<Note>(notes, pageNumber, pageSize, count);
        }

        public async Task<PagedResult<Note>> GetAllNotes(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            List<Note> notes = await _dbContext.Notes
                .AsNoTracking()
                .OrderBy(static n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            int count = await _dbContext.Notes.CountAsync(cancellationToken);

            return new PagedResult<Note>(notes, pageNumber, pageSize, count);
        }

        public Task RemoveNote(Note note)
        {
            _ = _dbContext.Remove(note);
            return Task.CompletedTask;
        }
    }
}