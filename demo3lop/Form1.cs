using DAL.Entities;
using GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace demo3lop
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public frmStudent()
        {
            InitializeComponent();
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            try

            {
                setGridViewStyle(dataGridView1);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.comboBox1.DataSource = listFacultys;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }
        //Hàm binding gridView từ list sinh viên
        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dataGridView1.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dataGridView1.Rows[index].Cells[4].Value = item.Major.Name + "";

                // Display image if the file path is available
                if (!string.IsNullOrEmpty(item.Avatar))
                {
                    pictureBox1.ImageLocation = item.Avatar; // Set image path
                    pictureBox1.Load(); // Load the image from the path
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }
        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                pictureBox1.Image = null;
            }
            else
            {
                string parentDirectory =
                Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, " Images ", ImageName);
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.Refresh();
            }
        }
        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.checkBox1.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            {
                try
                {
                    int studentID = int.Parse(textBox1.Text);
                    studentService.Delete(studentID); // Gọi phương thức xóa
                    MessageBox.Show("Xóa sinh viên thành công!");
                    BindGrid(studentService.GetAll()); // Làm mới DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells[0].Value?.ToString();
                textBox2.Text = row.Cells[1].Value?.ToString();
                textBox3.Text = row.Cells[3].Value?.ToString();

                // Lấy FacultyID thay vì FacultyName
                var studentID = int.Parse(textBox1.Text);
                var student = studentService.FindByID(studentID);

                if (student != null)
                {
                    comboBox1.SelectedValue = student.FacultyID; // Gán FacultyID vào ComboBox
                }

                // Gán Major nếu có
                if (row.Cells[4].Value == null || string.IsNullOrEmpty(row.Cells[4].Value.ToString()))
                {
                    checkBox1.Checked = true;
                }
                else
                {
                    checkBox1.Checked = false;
                }

                // Load image from the Avatar file path (stored in SQL)
                string avatarPath = student?.Avatar; // Assuming Avatar stores the file path
                if (!string.IsNullOrEmpty(avatarPath))
                {
                    // Ensure the image exists at the path before loading it
                    if (File.Exists(avatarPath))
                    {
                        pictureBox1.ImageLocation = avatarPath; // Set the image location
                        pictureBox1.Load(); // Load the image into the PictureBox
                    }
                    else
                    {
                        pictureBox1.Image = null; // If image not found, clear the PictureBox
                    }
                }
                else
                {
                    pictureBox1.Image = null; // If no Avatar path, clear the PictureBox
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int studentID = int.Parse(textBox1.Text);
                var student = studentService.FindByID(studentID);

                string imagePath = null;
                if (pictureBox1.Image != null)
                {
                    imagePath = pictureBox1.ImageLocation; // Store the file path
                }

                if (student != null)
                {
                    student.FullName = textBox2.Text;
                    student.AverageScore = float.Parse(textBox3.Text);
                    student.FacultyID = (int)comboBox1.SelectedValue;
                    student.Avatar = imagePath; // Save the image path

                    studentService.InsertUpdate(student); // Update the database
                    MessageBox.Show("Cập nhật sinh viên thành công!");
                }
                else
                {
                    var newStudent = new Student
                    {
                        StudentID = studentID,
                        FullName = textBox2.Text,
                        AverageScore = float.Parse(textBox3.Text),
                        FacultyID = (int)comboBox1.SelectedValue,
                        Avatar = imagePath // Save the image path
                    };

                    studentService.InsertUpdate(newStudent); // Insert into the database
                    MessageBox.Show("Thêm sinh viên thành công!");
                }

                BindGrid(studentService.GetAll()); // Refresh the DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
     
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp"; // Hạn chế loại file được chọn

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Hiển thị ảnh đã chọn trong PictureBox
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                pictureBox1.ImageLocation = openFileDialog.FileName;
            }

        }

    }
}




        
 
