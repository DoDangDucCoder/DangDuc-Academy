using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HDNXUdemyServices.Services
{
    public class CourseNoteService : ICourseNoteService
    {
        private readonly ProjectContext _context;

        public CourseNoteService(ProjectContext context)
        {
            _context = context;
        }

        public async Task<List<CourseNoteModel>> GetNotesByCourseAsync(long studentId, long courseId)
        {
            return await _context.CourseNotes!
                .Where(n => n.IdStudent == studentId && n.IdCourse == courseId)
                .OrderBy(n => n.Timestamp)
                .Select(n => new CourseNoteModel
                {
                    Id = n.Id,
                    IdStudent = n.IdStudent,
                    IdCourse = n.IdCourse,
                    IdContentDetail = n.IdContentDetail,
                    Timestamp = n.Timestamp,
                    Content = n.Content
                }).ToListAsync();
        }

        public async Task<CourseNoteModel> AddNoteAsync(CourseNoteModel model)
        {
            var entity = new CourseNoteEntities
            {
                IdStudent = model.IdStudent,
                IdCourse = model.IdCourse,
                IdContentDetail = model.IdContentDetail,
                Timestamp = model.Timestamp,
                Content = model.Content
            };
            _context.CourseNotes!.Add(entity);
            await _context.SaveChangesAsync();
            model.Id = entity.Id;
            return model;
        }

        public async Task<CourseNoteModel> UpdateNoteAsync(long noteId, CourseNoteModel model)
        {
            var note = await _context.CourseNotes!.FindAsync(noteId);
            if (note != null)
            {
                note.Content = model.Content;
                await _context.SaveChangesAsync();
            }
            return model;
        }

        public async Task<bool> DeleteNoteAsync(long noteId, long studentId)
        {
            var note = await _context.CourseNotes!.FirstOrDefaultAsync(n => n.Id == noteId && n.IdStudent == studentId);
            if (note != null)
            {
                _context.CourseNotes!.Remove(note);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
