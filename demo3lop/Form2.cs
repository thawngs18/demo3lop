using DAL.Entities;
using GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demo3lop
{
    public partial class Form2 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacultys = facultyService.GetAll();
                FillFalcutyCombobox(listFacultys);
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void FillFalcutyCombobox(List<Faculty> listFacultys)
        {
            this.comboBox1.DataSource = listFacultys;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = comboBox1.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoManor(selectedFaculty.FacultyID);
                BindGird(listStudents);
            }
        }
        private void FillMajorCombobox(List<Major> listMajor)
        {
            this.comboBox2.DataSource = listMajor;
            this.comboBox2.DisplayMember = "Name";
            this.comboBox2.ValueMember = "MajorID";
        }
        private void BindGird(List<Student> students)
        {
            dataGridView1.Rows.Clear();
            foreach (Student student in students)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[1].Value = student.StudentID;
                dataGridView1.Rows[index].Cells[2].Value = student.FullName;
                if(student.Faculty != null)
                {
                    dataGridView1.Rows[index].Cells[3].Value = student.Faculty.FacultyName;
                }
                dataGridView1.Rows[index].Cells[4].Value = student.AverageScore;
                if (student.Major != null)
                {
                    dataGridView1.Rows[index].Cells[5].Value = student.Major.Name+"";
                }

            }

        }
    }
}
