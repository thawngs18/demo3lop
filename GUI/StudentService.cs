using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            Model1 context = new Model1();
            return context.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            Model1 context = new Model1();
            return context.Students.Where(p=>p.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoManor(int facultyID)
        {
            Model1 context = new Model1();
            return context.Students.Where(p=>p.MajorID == null && p.FacultyID == facultyID).ToList();
        }
        public Student FindByID(int studentId)
        {
            Model1 context = new Model1();
            return context.Students.FirstOrDefault(p=>p.StudentID == studentId);
        }
        public void InsertUpdate(Student s)
        {
            Model1 context = new Model1();
            context.Students.AddOrUpdate(s);
            context.SaveChanges();
        }

        public void Delete(int studentID)
        {
            using (var context = new Model1())
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);
                if (student != null)
                {
                    context.Students.Remove(student); // Xóa sinh viên
                    context.SaveChanges(); // Lưu thay đổi
                }
            }
        }

        public void SaveProfilePicturePath(int studentID, string imagePath)
        {
            using (Model1 context = new Model1())
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);
                if (student != null)
                {
                    student.Avatar = imagePath; // Lưu đường dẫn hình ảnh
                    context.SaveChanges();
                }
            }
        }

        public string GetProfilePicturePath(int studentID)
        {
            using (Model1 context = new Model1())
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);
                return student?.Avatar; // Trả về đường dẫn hình ảnh
            }
        }

    }
}
