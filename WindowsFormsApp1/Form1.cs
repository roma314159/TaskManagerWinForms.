using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        List<TaskItem> tasks = new List<TaskItem>();

        public Form1()
        {
            InitializeComponent();

            lstTasks.DrawMode = DrawMode.OwnerDrawVariable;
            lstTasks.MeasureItem += lstTasks_MeasureItem;
            lstTasks.DrawItem += lstTasks_DrawItem;
            lstTasks.MouseDown += lstTasks_MouseDown;

            lstTasks.SelectedIndexChanged += lstTasks_SelectedIndexChanged;

            lstTasks.SelectionMode = SelectionMode.One;

            cmbFilter.Items.Clear();
            cmbFilter.Items.Add("Все");
            cmbFilter.Items.Add("Выполненные");
            cmbFilter.Items.Add("Невыполненные");
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += cmbFilter_SelectedIndexChanged;

            cmbSort.Items.Clear();
            cmbSort.Items.Add("Без сортировки");
            cmbSort.Items.Add("По дате ↑");
            cmbSort.Items.Add("По дате ↓");
            cmbSort.Items.Add("По названию A→Z");
            cmbSort.Items.Add("По названию Z→A");
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += cmbSort_SelectedIndexChanged;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TaskItem task = new TaskItem
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                Deadline = dtpDeadLine.Value,
                Priority = cmbPriority.Text,
                IsDone = chkDone.Checked
            };

            tasks.Add(task);
            RefreshList();
            ClearFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана задача для выполнения действия");
                return;
            }

            TaskItem task = (TaskItem)lstTasks.SelectedItem;
            tasks.Remove(task);

            RefreshList();
            ClearFields();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана задача для выполнения действия");
                return;
            }

            TaskItem task = (TaskItem)lstTasks.SelectedItem;

            task.Title = txtTitle.Text;
            task.Description = txtDescription.Text;
            task.Deadline = dtpDeadLine.Value;
            task.Priority = cmbPriority.Text;
            task.IsDone = chkDone.Checked;

            RefreshList();
            ClearFields();
        }

        private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem == null)
                return;

            TaskItem task = (TaskItem)lstTasks.SelectedItem;

            txtTitle.Text = task.Title;
            txtDescription.Text = task.Description;
            dtpDeadLine.Value = task.Deadline;
            cmbPriority.Text = task.Priority;
            chkDone.Checked = task.IsDone;
        }

        private void RefreshList()
        {
            lstTasks.Items.Clear();

            List<TaskItem> list = new List<TaskItem>(tasks);

            string filter = cmbFilter.Text;

            if (filter == "Выполненные")
                list.RemoveAll(t => !t.IsDone);

            if (filter == "Невыполненные")
                list.RemoveAll(t => t.IsDone);

            string sort = cmbSort.Text;

            switch (sort)
            {
                case "По дате ↑":
                    list.Sort((a, b) => a.Deadline.CompareTo(b.Deadline));
                    break;

                case "По дате ↓":
                    list.Sort((a, b) => b.Deadline.CompareTo(a.Deadline));
                    break;

                case "По названию A→Z":
                    list.Sort((a, b) => a.Title.CompareTo(b.Title));
                    break;

                case "По названию Z→A":
                    list.Sort((a, b) => b.Title.CompareTo(a.Title));
                    break;
            }

            foreach (TaskItem t in list)
                lstTasks.Items.Add(t);
        }

        private void ClearFields()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            chkDone.Checked = false;
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void lstTasks_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstTasks.IndexFromPoint(e.Location) == ListBox.NoMatches)
                lstTasks.ClearSelected();
        }

        private void lstTasks_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 95;
        }

        private void lstTasks_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            TaskItem task = (TaskItem)lstTasks.Items[e.Index];

            Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = Color.White;

            if (task.IsDone)
                backColor = Color.LightGreen;
            else if (task.Deadline < DateTime.Now)
                backColor = Color.LightCoral;

            if (selected)
                backColor = Color.FromArgb(80, Color.DodgerBlue);

            using (Brush b = new SolidBrush(backColor))
                e.Graphics.FillRectangle(b, rect);

            Color border =
                task.IsDone ? Color.Green :
                task.Deadline < DateTime.Now ? Color.Red :
                Color.Gray;

            using (Pen p = new Pen(border, 2))
                e.Graphics.DrawRectangle(p, rect);

            string text =
                $"Название: {task.Title}\n" +
                $"Описание: {task.Description}\n" +
                $"Приоритет: {task.Priority}\n" +
                $"Дата: {task.Deadline:dd.MM.yyyy}";

            e.Graphics.DrawString(text, e.Font, Brushes.Black, rect.X + 6, rect.Y + 6);

            e.DrawFocusRectangle();
        }
    }
}