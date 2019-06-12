using CMS.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace CMS.DAL.Services
{
    public class DbFileService : DbUpdateService
    {
        public List<File> GetFiles(int fieldId, int docId)
        {
            using (var db = new CMSContext())
            {
                var query = from link in db.FileLinks
                            join file in db.Files on link.FileId equals file.Id
                            where !file.Deleted && link.FieldId == fieldId && link.DocId == docId
                            select new
                            {
                                file.Id,
                                file.Name,
                                file.Comment,
                                file.CreateDate,
                                file.CreatedBy
                            };

                return query.ToList().Select(i => new File()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Comment = i.Comment,
                    CreateDate = i.CreateDate,
                    CreatedBy = i.CreatedBy
                }).ToList();
            }
        }

        public File GetFile(int fileId)
        {
            using (var db = new CMSContext())
            {
                return db.Files.FirstOrDefault(f => f.Id == fileId);
            }
        }

        public List<File> AddFiles(int fieldId, int docId, IEnumerable<File> files)
        {
            List<File> result = new List<File>();
            using (var db = new CMSContext())
            {
                foreach(File file in files)
                {
                    db.Files.Add(file);
                    db.SaveChanges();

                    var fileLink = new FileLink()
                    {
                        FieldId = fieldId,
                        DocId = docId,
                        FileId = file.Id
                    };

                    db.FileLinks.Add(fileLink);
                    db.SaveChanges();

                    result.Add(file);
                }
            }
            return result;
        }

        public void UpdateFile(File file)
        {
            using (var db = new CMSContext())
            {
                var fileEntity = db.Files.FirstOrDefault(f => f.Id == file.Id);
                db.Entry(fileEntity).CurrentValues.SetValues(file);
                db.SaveChanges();
            }
        }
    }
}
