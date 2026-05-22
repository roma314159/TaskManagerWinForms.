using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{
    /// <summary>
    /// Основная форма приложения TaskManager для управления задачами.
    /// </summary>
    public partial class Form1 : Form
    {
        List<TaskItem> tasks = new List<TaskItem>();

        /// <summary>
        /// Инициализация формы и настройка интерфейса.
        /// </summary>
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

        /// <summary>
        /// Добавление новой задачи.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название задачи");
                return;
            }

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

        /// <summary>
        /// Удаление выбранной задачи.
        /// </summary>
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

        /// <summary>
        /// Редактирование выбранной задачи.
        /// </summary>
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

        /// <summary>
        /// Заполнение полей при выборе задачи.
        /// </summary>
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

        /// <summary>
        /// Обновление списка задач с фильтром и сортировкой.
        /// </summary>
        private void RefreshList()
        {
            lstTasks.Items.Clear();

            List<TaskItem> list = new List<TaskItem>(tasks);

            if (cmbFilter.Text == "Выполненные")
                list.RemoveAll(t => !t.IsDone);

            if (cmbFilter.Text == "Невыполненные")
                list.RemoveAll(t => t.IsDone);

            switch (cmbSort.Text)
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

        /// <summary>
        /// Очистка полей ввода.
        /// </summary>
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

        /// <summary>
        /// Снятие выделения при клике вне элементов списка.
        /// </summary>
        private void lstTasks_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstTasks.IndexFromPoint(e.Location) == ListBox.NoMatches)
                lstTasks.ClearSelected();
        }

        /// <summary>
        /// Установка высоты элемента списка.
        /// </summary>
        private void lstTasks_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 95;
        }

        /// <summary>
        /// Отрисовка элемента списка задач.
        /// </summary>
        private void lstTasks_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            TaskItem task = (TaskItem)lstTasks.Items[e.Index];

            bool isOverdue = task.Deadline.Date < DateTime.Now.Date && !task.IsDone;

            Rectangle rect = new Rectangle(
                e.Bounds.X,
                e.Bounds.Y,
                e.Bounds.Width - 1,
                e.Bounds.Height - 1
            );

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = selected ? Color.FromArgb(80, Color.DodgerBlue) : Color.White;

            using (Brush bg = new SolidBrush(backColor))
                e.Graphics.FillRectangle(bg, rect);

            using (Pen pen = new Pen(Color.Gray, 2))
                e.Graphics.DrawRectangle(pen, rect);

            Color textColor = Color.Black;

            if (isOverdue)
                textColor = Color.Red;

            string icon = "";

            if (task.IsDone)
                icon = "✔";
            else if (isOverdue)
                icon = "✖";

            e.Graphics.DrawString(
                icon,
                new Font("Segoe UI", 16, FontStyle.Bold),
                Brushes.Black,
                rect.X + 5,
                rect.Y + 5
            );

            string title = task.Title;
            string description = task.Description;

            if (title.Length > 25)
                title = title.Substring(0, 25) + "...";

            if (description.Length > 40)
                description = description.Substring(0, 40) + "...";

            string text =
                $"Название: {title}\n" +
                $"Описание: {description}\n" +
                $"Приоритет: {task.Priority}\n" +
                $"Дата: {task.Deadline:dd.MM.yyyy}";

            using (Brush br = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(
                    text,
                    e.Font,
                    br,
                    rect.X + 35,
                    rect.Y + 8
                );
            }

            e.DrawFocusRectangle();
        }
    }
}